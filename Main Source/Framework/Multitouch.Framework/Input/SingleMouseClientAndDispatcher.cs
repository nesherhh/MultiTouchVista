using System;
using System.Diagnostics;
using System.Windows;
using ManagedWinapi.Hooks;
using ManagedWinapi.Windows;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	class SingleMouseClientAndDispatcher : IApplicationInterface, IApplicationInterfaceCallback, IDisposable
	{
		readonly CommunicationLogic logic;
		LowLevelMouseHook mouseHook;
		int id;

		public SingleMouseClientAndDispatcher(CommunicationLogic logic)
		{
			this.logic = logic;
			mouseHook = new LowLevelMouseHook();
			mouseHook.MessageIntercepted += mouseHook_MessageIntercepted;
			mouseHook.StartHook();
			id = 0;
		}

		void mouseHook_MessageIntercepted(LowLevelMessage evt, ref bool handled)
		{
			handled = false;

			LowLevelMouseMessage message = evt as LowLevelMouseMessage;
			if (message != null)
			{
				FrameData data = new FrameData();
				data.Timestamp = Stopwatch.GetTimestamp();
				data.Images = new Service.ImageData[0];

				ContactData contact = new ContactData();
				contact.MajorAxis = 10;
				contact.MinorAxis = 10;
				contact.Orientation = 0;
				contact.Position = new Point(message.Point.X, message.Point.Y);

				SystemWindow window = SystemWindow.FromPointEx(message.Point.X, message.Point.Y, true, true);
				contact.Hwnd = window.HWnd;

				switch ((MouseEventFlagValues)message.MouseEventFlags)
				{
					case MouseEventFlagValues.LEFTDOWN:
						id++;
						if (id == int.MaxValue)
							id = 0;
						contact.State = Service.ContactState.New;
						break;
					case MouseEventFlagValues.LEFTUP:
						contact.State = Service.ContactState.Removed;
						break;
					case MouseEventFlagValues.MOVE:
						contact.State = Service.ContactState.Moved;
						break;
					default:
						return;
				}
				contact.Id = id;
				contact.Bounds = new Rect(contact.Position.X - (contact.MajorAxis / 2), contact.Position.Y - (contact.MinorAxis / 2), contact.MajorAxis, contact.MinorAxis);
				data.Contacts = new[] { contact };
				
				Frame(data);
			}
		}

		[Flags]
		private enum MouseEventFlagValues
		{
			LEFTDOWN = 0x00000002,
			LEFTUP = 0x00000004,
			MIDDLEDOWN = 0x00000020,
			MIDDLEUP = 0x00000040,
			MOVE = 0x00000001,
			RIGHTDOWN = 0x00000008,
			RIGHTUP = 0x00000010,
			WHEEL = 0x00000800,
			HWHEEL = 0x00001000
		}

		public void CreateSession()
		{}

		public void RemoveSession()
		{}

		public void AddWindowHandleToSession(IntPtr windowHandle)
		{}

		public void RemoveWindowHandleFromSession(IntPtr windowHandle)
		{}

		public void SendEmptyFrames(bool value)
		{}

		public bool SendImageType(Service.ImageType imageType, bool value)
		{
			return false;
		}

		public void Frame(FrameData data)
		{
			logic.DispatchFrame(data);			
		}

		public void Dispose()
		{
			if(mouseHook != null)
				mouseHook.Dispose();
		}
	}
}
