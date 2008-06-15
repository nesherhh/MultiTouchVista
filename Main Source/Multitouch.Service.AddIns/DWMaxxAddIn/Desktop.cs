using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using AdvanceMath;
using DWMaxxAddIn.Native;
using dwmaxxLib;
using ManagedWinapi.Hooks;
using ManagedWinapi.Windows;
using Multitouch.Contracts;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;
using Point=System.Drawing.Point;

namespace DWMaxxAddIn
{
	class Desktop : IDisposable
	{
		DWMInjectorClass injectorClass;
		DWMEx dwm;

		Dictionary<Window, Body> windowToBody;
		Dictionary<int, FixedHingeJoint> contactJoints;
		PhysicsEngine engine;
		PhysicsTimer timer;
		WindowManager windowManager;
		object lockPhysicsLoop = new object();
		LowLevelMouseHook hook;

		public Desktop()
		{
			windowToBody = new Dictionary<Window, Body>();
			contactJoints = new Dictionary<int, FixedHingeJoint>();
			engine = new PhysicsEngine();
			engine.BroadPhase = new SweepAndPruneDetector();
			engine.Solver = new SequentialImpulsesSolver();
			timer = new PhysicsTimer(PhysicsTimerCallback, 0.01);
			timer.IsRunning = true;

			injectorClass = new DWMInjectorClass();
			if (!injectorClass.IsInjected)
				injectorClass.Inject();
			injectorClass.GetDWMExObject(out dwm);
			Debug.Assert(dwm != null, "DWM is NULL");

			windowManager = new WindowManager();
			windowManager.WindowCreated += windowManager_WindowCreated;
			windowManager.WindowDestroyed += windowManager_WindowDestroyed;
			windowManager.Register(MessageEvents.Handle);

			ThreadPool.QueueUserWorkItem(delegate
										 {
											 hook = new LowLevelMouseHook(MouseHook);
											 hook.StartHook();
											 Application.Run();
										 });
		}

		void MouseHook(int msg, POINT pt, int mouseData, int flags, int time, IntPtr dwExtraInfo, ref bool handled)
		{
			if (flags != -1)
				handled = true;
		}

		void windowManager_WindowDestroyed(Window window)
		{
			RemoveBody(window);
		}

		void windowManager_WindowCreated(Window window)
		{
			CreateBody(window);
		}

		void RemoveBody(Window window)
		{
			lock (lockPhysicsLoop)
			{
				Body body;
				if (windowToBody.TryGetValue(window, out body))
				{
					body.Lifetime.IsExpired = true;
					windowToBody.Remove(window);
				}
			}
		}

		void CreateBody(Window window)
		{
			lock (lockPhysicsLoop)
			{
				double angle = window.Angle;
				Rectangle position = window.Position;
				if (position.Width > 0 && position.Height > 0)
				{
					PhysicsState state = new PhysicsState(new ALVector2D(angle, position.Left + position.Width / 2, position.Top + position.Height / 2));
					Shape shape = new PolygonShape(PolygonShape.CreateRectangle(position.Height, position.Width), 2);
					MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, 1);
					Body body = new Body(state, shape, mass, new Coefficients(0, 1), new Lifespan());
					body.LinearDamping = 0.95;
					body.AngularDamping = 0.95;
					body.IsCollidable = false;
					body.Tag = window;
					engine.AddBody(body);
					windowToBody.Add(window, body);

					try
					{
						dwm.SetWindowMatrix(window.HWnd.ToInt32(), 0f, 0f, 1f, 1f, 0f);
					}
					catch (Exception e)
					{
						HandleDwmCrash(e);
					}
				}
			}
		}

		void HandleDwmCrash(Exception e)
		{
			Trace.TraceError(e.Message);
			Trace.TraceInformation("Killing DWM");
			Process name = Process.GetProcessesByName("dwm").Where(p => p.SessionId.Equals(Process.GetCurrentProcess().SessionId)).SingleOrDefault();
			name.Kill();
			MessageBox.Show("DWM Crashed");

			Environment.Exit(1);
		}

		void PhysicsTimerCallback(double dt)
		{
			lock (lockPhysicsLoop)
			{
				engine.Update(dt);
				UpdateWindows();
			}
		}

		void UpdateWindows()
		{
			foreach (Body body in engine.Bodies)
			{
				Window window = body.Tag as Window;
				if (window != null)
				{
					Vector2D linearPosition = body.State.Position.Linear;
					Rectangle rectangle = window.Position;
					int left = (int)(linearPosition.X - rectangle.Width / 2.0);
					int top = (int)(linearPosition.Y - rectangle.Height / 2.0);
					if (rectangle.Left != left && rectangle.Top != top)
						window.Position = new Rectangle(new Point(left, top), rectangle.Size);

					double degrees = MathHelper.ToDegrees(body.State.Position.Angular);
					if (window.Angle != degrees)
					{
						window.Angle = degrees;
						Matrix m = window.Matrix;

						if (NativeMethods.IsWindow(window.HWnd))
						{
							try
							{
								dwm.SetWindowMatrix2(window.HWnd.ToInt32(), (float)m.M11, (float)m.M12, (float)m.M21, (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
							}
							catch (Exception e)
							{
								HandleDwmCrash(e);
							}
						}
					}
				}
			}
		}

		public void OnNewContact(IntPtr windowHandle, IContact contact)
		{
			Window window = GetWindow(windowHandle);
			if (window != null)
			{
				window.ContactsCount++;

				Body body;
				if (windowToBody.TryGetValue(window, out body))
				{
					NativeMethods.POINT point = new NativeMethods.POINT((int)contact.X, (int)contact.Y);

					Vector2D contactPoint = point.ToVector2D();

					if (!body.Shape.BroadPhaseDetectionOnly && body.Shape.CanGetIntersection)
					{
						Vector2D temp = body.Matrices.ToBody * contactPoint;
						IntersectionInfo intersectionInfo;
						if (body.Shape.TryGetIntersection(temp, out intersectionInfo))
						{
							FixedHingeJoint joint = new FixedHingeJoint(body, contactPoint, new Lifespan());
							engine.AddJoint(joint);
							contactJoints[contact.Id] = joint;
						}
					}
				}
			}
		}

		Window GetWindow(IntPtr windowHandle)
		{
			return windowToBody.Keys.Where(w => w.HWnd.Equals(windowHandle)).SingleOrDefault();
		}

		public void OnContactRemoved(IntPtr windowHandle, IContact contact)
		{
			Window window = GetWindow(windowHandle);
			if (window != null)
			{
				window.ContactsCount--;

				FixedHingeJoint joint;
				if (contactJoints.TryGetValue(contact.Id, out joint))
				{
					joint.Lifetime.IsExpired = true;
					contactJoints.Remove(contact.Id);
				}
			}
		}

		public void OnContactMoved(IntPtr windowHandle, IContact contact)
		{
			Window window = GetWindow(windowHandle);
			if (window != null)
			{
				FixedHingeJoint joint;
				if (contactJoints.TryGetValue(contact.Id, out joint))
					joint.Anchor = new Vector2D(contact.X, contact.Y);
			}
		}

		public void Dispose()
		{
			if (hook != null)
				hook.Unhook();
			if (windowManager != null)
				windowManager = null;
		}

		public IntPtr GetWindowAt(Point screenCoordinates, out Point transformedScreenCoordinates)
		{
			IntPtr result = IntPtr.Zero;
			transformedScreenCoordinates = screenCoordinates;
			foreach (Window currentWindow in windowToBody.Keys)
			{
				Matrix matrix = currentWindow.GetMatrix(currentWindow);
				matrix.Invert();

				Point windowCoordinates = NativeMethods.ScreenToClient(currentWindow, screenCoordinates);
				System.Windows.Point transformedPoint = matrix.Transform(windowCoordinates.ToPoint());
				transformedScreenCoordinates = NativeMethods.ClientToScreen(currentWindow, transformedPoint.ToPoint());

				SystemWindow windowAtPoint = SystemWindow.FromPointEx(transformedScreenCoordinates.X, transformedScreenCoordinates.Y, true, true);
				if (windowAtPoint != null && windowAtPoint.HWnd.Equals(currentWindow.HWnd))
				{
					result = windowAtPoint.HWnd;
					break;
				}
			}

			return result;
		}
	}
}