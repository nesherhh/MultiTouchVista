using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Logic.LegacySupport;
using dwmaxxLib;
using ManagedWinapi.Windows;
using Point=System.Windows.Point;

namespace Danilins.Multitouch.Logic.DWM
{
	class WindowManager
	{
		class State
		{
			readonly SystemWindow systemWindow;
			int contactCount;
			
			public State(SystemWindow window, int contactId)
			{
				FirstContactId = contactId;
				systemWindow = window;
				Angle = 0;
				Scale = 1;
				LastScale = 1;
			}

			public int FirstContactId { get; set; }
			public int ContactCount
			{
				get { return contactCount; }
				set
				{
					contactCount = value;
					if (contactCount <= 0)
					{
						contactCount = 0;
						FirstContactId = -1;
					}
				}
			}
			public Point LastPoint { get; set; }
			public double Angle { get; set; }
            public double Scale { get; set; }
            public double LastScale { get; set; }
            public double LastDistance { get; set; }

			public Matrix Matrix
			{
				get
				{
					RECT rectangle = systemWindow.Rectangle;
					Point center = new Point((rectangle.Width) / 2.0,
						(rectangle.Height + SystemInformation.CaptionHeight) / 2.0);

					Matrix m = new Matrix();
					m.Translate(-center.X, -center.Y);
					m.Rotate(Angle);
					m.Translate(center.X, center.Y);
					m.Scale(Scale, Scale);
					return m;
				}
			}

			public Matrix GetMatrix(SystemWindow window)
			{
				RECT rectangle = window.Rectangle;
				Point center = new Point((rectangle.Width) / 2.0,
					(rectangle.Height - SystemInformation.CaptionHeight) / 2.0);

				Matrix m = new Matrix();
				m.Translate(-center.X, -center.Y);
				m.Rotate(Angle);
				m.Translate(center.X, center.Y);
				m.Scale(Scale, Scale);
				return m;
			}
		}

		DWMInjector injector;
		DWMEx dwm;
		Dictionary<SystemWindow, State> windowTable;
		Queue<Action> tasks;

		public WindowManager()
		{
			windowTable = new Dictionary<SystemWindow, State>();
			tasks = new Queue<Action>();

			injector = new DWMInjector();
			if (!injector.IsInjected)
				injector.Inject();

			injector.GetDWMExObject(out dwm);
			Debug.Assert(dwm != null, "DWM is NULL");
		}

		public ContactInfo[] Process(ContactInfo[] contacts)
		{
			List<ContactInfo> processed = new List<ContactInfo>();
			foreach (var contact in contacts)
			{
				bool moved = false;
				switch (contact.State)
				{
					case ContactState.None:
						break;
					case ContactState.Down:
						HandleDown(contact);
						break;
					case ContactState.Up:
						HandleUp(contact);
						break;
					case ContactState.Move:
						moved = HandleMove(contact, contacts);
						break;
					default:
						break;
				}

				if (!moved)
				{
					State windowState;
					Point transformedScreenCoordinates;
					SystemWindow window = GetWindowFromContact(contact, out windowState, out transformedScreenCoordinates);
					if (window != null)
					{
						//Trace.WriteLine(window.ClassName);
						//Point subWindowCoordinates = ScreenToClient(subWindow, contact.Center);
						//Matrix matrix = windowState.GetMatrix(subWindow);
						//matrix.Invert();
						//Point transform = matrix.Transform(subWindowCoordinates);

						//using (WindowDeviceContext context = subWindow.GetDeviceContext(true))
						//{
						//    using (Graphics graphics = context.CreateGraphics())
						//    {
						//        graphics.FillEllipse(System.Drawing.Brushes.Red, (float)(transform.X - 5),
						//                             (float)(transform.Y - 5), 10, 10);
						//    }
						//}

						//Trace.WriteLine(string.Format("processing window: {0} at {1}", subWindow.ClassName, transform));
						//processed.Add(CreateNewContact(contact, ClientToScreen(subWindow, transform)));
						processed.Add(CreateNewContact(contact, transformedScreenCoordinates));
					}
				}
			}
			return processed.ToArray();
		}

		SystemWindow GetWindowFromContact(ContactInfo contact, out State windowState, out Point transformedScreenCoordinates)
		{
			SystemWindow window = null;
			transformedScreenCoordinates = new Point();
			windowState = null;

			List<SystemWindow> toDelete = new List<SystemWindow>();
			foreach (var valuePair in windowTable)
			{
				SystemWindow currentWindow = valuePair.Key;
				if (currentWindow.Process.Id.Equals(0))
				{
					toDelete.Add(currentWindow);
					continue;
				}

				Point windowCoordinates = ScreenToClient(currentWindow, contact.Center);

				windowState = valuePair.Value;
				Matrix m = windowState.GetMatrix(currentWindow);
				m.Invert();

				Point transformedPoint = m.Transform(windowCoordinates);
				transformedScreenCoordinates = ClientToScreen(currentWindow, transformedPoint);

				SystemWindow windowFromPoint = SystemWindow.FromPointEx((int)transformedScreenCoordinates.X,
				                                                        (int)transformedScreenCoordinates.Y, true, true);
				if (windowFromPoint == currentWindow)
				{
					window = windowFromPoint;
					break;
				}
			}
			foreach (SystemWindow systemWindow in toDelete)
				windowTable.Remove(systemWindow);

			string msg;
			if (window == null)
				msg = "window not found";
			else
				msg = string.Format("WIN: {0}", window.ClassName);
			Trace.WriteLine(string.Format("{0}", msg));
			return window;
		}

		ContactInfo CreateNewContact(ContactInfo contact, Point coordinates)
		{
			ContactInfo c = new ContactInfo(new Rect(coordinates.X - 5, coordinates.Y - 5, 10, 10));
			c.Delta = contact.Delta;
			c.Displacement = contact.Displacement;
			c.Id = contact.Id;
			c.PredictedPos = contact.PredictedPos;
			c.State = contact.State;
			return c;
		}

		bool HandleMove(ContactInfo contact, ContactInfo[] allContacts)
		{
			State windowState;
			Point transformedCoordinates;
			
			SystemWindow window = GetWindowFromContact(contact, out windowState, out transformedCoordinates);
			if (window == null)
				window = SystemWindow.FromPointEx((int)contact.Center.X, (int)contact.Center.Y, true, true);
			if (window != null)
			{
				if (windowTable.ContainsKey(window))
				{
					State state = windowTable[window];
					if (state.ContactCount > 1 && window.WindowState != FormWindowState.Maximized)
					{
						if (state.FirstContactId == contact.Id)
						{
							//Rotate
							Point nullPoint = new Point(
								window.Rectangle.Left + (SystemInformation.HorizontalResizeBorderThickness + window.Rectangle.Width) / 2.0,
								window.Rectangle.Top + (SystemInformation.VerticalResizeBorderThickness + window.Rectangle.Height) / 2.0);
							Point currentPoint = contact.Center;

							Vector vector1 = state.LastPoint - nullPoint;
							Vector vector2 = currentPoint - nullPoint;
							double between = Vector.AngleBetween(vector1, vector2);
							state.Angle += between;

							state.LastPoint = currentPoint;
						}
						//rotate end

						//Scale
/*						IEnumerable<ContactInfo> movedContacts = allContacts.Where(x => x.State == ContactState.Move);
						ContactInfo[] array = movedContacts.ToArray();
						double origDist = 0.0;
						double curDist = 0.0;
						int divisor = 0;
						for (int i = 0; i < array.Length; i++)
						{
							for (int j = i + 1; j < array.Length; j++)
							{
								Vector vector = array[j].Center - array[i].Center;
								Vector previousVector = array[j].PreviousCenter - array[i].PreviousCenter;
								origDist += vector.Length;
								curDist += previousVector.Length;
								divisor++;
							}
						}

						origDist /= divisor;
						curDist /= divisor;

						if (origDist == 0)
							origDist = 1;

						state.LastScale += curDist / origDist - state.LastDistance;
						if (Math.Abs(state.LastScale) > 0.001)
							state.Scale = 1 * state.LastScale;
						else
							state.LastScale = 1;

						state.LastDistance = curDist / origDist;
*/						//scale end

						Matrix m = state.Matrix;
						if (!NativeMethods.InSendMessage())
						{
							dwm.SetWindowMatrix2(window.HWnd.ToInt32(), (float)m.M11, (float)m.M12, (float)m.M21,
							                     (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
						}
						else
						{
							lock (tasks)
							{
								tasks.Enqueue(() =>
								              {
								              	if (window != null)
								              	{
								              		dwm.SetWindowMatrix2(window.HWnd.ToInt32(), (float)m.M11, (float)m.M12, (float)m.M21,
								              		                     (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
								              	}
								              });
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		void HandleUp(ContactInfo contact)
		{
			State windowState;
			Point transformedCoordinates;
			
			SystemWindow window = GetWindowFromContact(contact, out windowState, out transformedCoordinates);
			if(window == null)
				window = SystemWindow.FromPointEx((int)contact.Center.X, (int)contact.Center.Y, true, true);
			if (window != null)
			{
				if (windowTable.ContainsKey(window))
				{
					State state = windowTable[window];
					state.ContactCount--;
					if (state.FirstContactId == contact.Id)
						state.FirstContactId = -1;
				}
			}
		}

		void HandleDown(ContactInfo contact)
		{
			State windowState;
			Point transformedCoordinates;
			
			SystemWindow window = GetWindowFromContact(contact, out windowState, out transformedCoordinates);
			if(window == null)
				window = SystemWindow.FromPointEx((int)contact.Center.X, (int)contact.Center.Y, true, true);
			if(window != null)
			{
				State state;
				if (windowTable.ContainsKey(window))
					state = windowTable[window];
				else
				{
					state = new State(window, contact.Id);
					state.LastPoint = contact.Center;
					windowTable.Add(window, state);
				}
				if(state.FirstContactId == -1)
					state.FirstContactId = contact.Id;
				state.ContactCount++;
			}
		}

		Point ScreenToClient(SystemWindow systemWindow, Point screenPoint)
		{
			POINT point = screenPoint.ToPOINT();

			int result = NativeMethods.MapWindowPoints(IntPtr.Zero, systemWindow.HWnd, ref point, 1);
			//if (result == 0)
			//    throw new Exception("could not get client coordinates");
			return point.ToPoint();
		}

		Point ClientToScreen(SystemWindow systemWindow, Point clientPoint)
		{
			POINT point = clientPoint.ToPOINT();

			int result = NativeMethods.MapWindowPoints(systemWindow.HWnd, IntPtr.Zero, ref point, 1);
			//if (result == 0)
			//    throw new Exception("could not get client coordinates");
			return point.ToPoint();
		}
	}
}
