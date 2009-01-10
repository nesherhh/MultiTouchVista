using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MultipleMice.Native;

namespace MultipleMice
{
	class RawDevicesManager : IDisposable
	{
		InputProvider inputProvider;
		DeviceCollection devices;
		ContactCollection contacts;
		int screenWidth;
		int screenHeight;
		double mouseSpeed;
		Rectangle virtualScreen;

		//const ushort PEN_X_MAX = 9650;
		//const ushort PEN_Y_MAX = 7188;
		const ushort PEN_X_MAX = 24500;
		const ushort PEN_Y_MAX = 18500;

		public RawDevicesManager(InputProvider inputProvider)
		{
			virtualScreen = SystemInformation.VirtualScreen;

			this.inputProvider = inputProvider;
			mouseSpeed = SystemInformation.MouseSpeed * 0.15;
			screenWidth = Screen.PrimaryScreen.Bounds.Width;
			screenHeight = Screen.PrimaryScreen.Bounds.Height;

			devices = new DeviceCollection();
			contacts = new ContactCollection();

			IEnumerable<RawDevice> rawDevices = from device in RawDevice.GetRawDevices()
												where (device.RawType == RawType.Device &&
														device.GetRawInfo().UsagePage == HID_USAGE_PAGE_DIGITIZER &&
														device.GetRawInfo().Usage == HID_USAGE_DIGITIZER_PEN) ||
														device.RawType == RawType.Mouse
												select device;


			foreach (RawDevice mouseDevice in rawDevices)
				devices.Add(new DeviceStatus(mouseDevice));

			Thread inputThread = new Thread(InputWorker);
			inputThread.IsBackground = true;
			inputThread.SetApartmentState(ApartmentState.STA);
			inputThread.Name = "MultipleMice thread";
			inputThread.Start();

			this.inputProvider.IsRunning = true;
		}

		void InputWorker()
		{
			RawDevice.RegisterRawDevices(0x01, 0x02, InputMode.BackgroundMode);
			RawDevice.RegisterRawDevices(HID_USAGE_PAGE_DIGITIZER, HID_USAGE_DIGITIZER_PEN, InputMode.BackgroundMode);
			RawDevice.RawInput += RawDevice_RawInput;
			Application.Run();
		}

		public const ushort HID_USAGE_PAGE_DIGITIZER = 0x0D;
		public const ushort HID_USAGE_DIGITIZER_DIGITIZER = 1;
		public const ushort HID_USAGE_DIGITIZER_PEN = 2;
		public const ushort HID_USAGE_DIGITIZER_LIGHTPEN = 3;
		public const ushort HID_USAGE_DIGITIZER_TOUCHSCREEN = 4;


		public void Dispose()
		{
			RawDevice.UnregisterRawDevices(0x01, 0x02);
			RawDevice.UnregisterRawDevices(HID_USAGE_PAGE_DIGITIZER, HID_USAGE_DIGITIZER_PEN);
			RawDevice.RawInput -= RawDevice_RawInput;
			Application.Exit();
			inputProvider.IsRunning = false;
		}

		void RawDevice_RawInput(object sender, RawInputEventArgs e)
		{
			if (devices.Contains(e.Handle))
			{
				DeviceStatus state = devices[e.Handle];
				MouseData mouseData = e.GetRawData() as MouseData;
				if (mouseData != null)
					UpdateMouse(mouseData, state);
				else
				{
					DeviceData deviceData = e.GetRawData() as DeviceData;
					if (deviceData != null)
						UpdatePen(deviceData, state);
				}

				if(state.ButtonState == DeviceState.None)
					return;

				MouseContact contact = null;
				if (state.ButtonState == DeviceState.Down)
				{
					contact = new MouseContact(state);
					//Debug.WriteLine("Down: " + contact);
					contacts.Add(contact);
				}
				else if ((state.ButtonState == DeviceState.Move || state.ButtonState == DeviceState.Up) && contacts.Contains(e.Handle))
				{
					contact = contacts[e.Handle];
					//Debug.WriteLine("Move: " + contact);
					contact.Update(state);
				}
				if (contact != null)
					inputProvider.EnqueueContact(contact);
				if (state.ButtonState == DeviceState.Up)
				{
					contact = contacts[e.Handle];
					contact.Update(state);
					//Debug.WriteLine("Up: " + contact);
					contacts.Remove(e.Handle);
				}
			}
		}

		void UpdatePen(DeviceData deviceData, DeviceStatus state)
		{
			int count;
			int size;
			IntPtr ptr = deviceData.GetDataPtr(out size, out count);
			PEN_DATA pen_DATA = (PEN_DATA)Marshal.PtrToStructure(ptr, typeof(PEN_DATA));

			int x = pen_DATA.X * virtualScreen.Width / PEN_X_MAX;
			int y = pen_DATA.Y * virtualScreen.Height / PEN_Y_MAX;

			state.Location = new Point(x, y);

			if((pen_DATA.Status & PenStatus.PenTipDown) == PenStatus.PenTipDown && state.ButtonState == DeviceState.None)
				state.ButtonState = DeviceState.Down;
			else if ((pen_DATA.Status & PenStatus.PenTipDown) == PenStatus.PenTipDown && (state.ButtonState == DeviceState.Down || state.ButtonState == DeviceState.Move))
				state.ButtonState = DeviceState.Move;
			else if ((pen_DATA.Status & PenStatus.InRange) == PenStatus.InRange && (state.ButtonState == DeviceState.Move || state.ButtonState == DeviceState.Down))
				state.ButtonState = DeviceState.Up;
			else
				state.ButtonState = DeviceState.None;
		}

		void UpdateMouse(MouseData mouseData, DeviceStatus state)
		{
			Point location = state.Location;
			location.X += (int)(mouseData.X * mouseSpeed);
			location.Y += (int)(mouseData.Y * mouseSpeed);

			if (location.X <= 0)
				location.X = 0;
			if (location.Y <= 0)
				location.Y = 0;
			if (location.X >= screenWidth)
				location.X = screenWidth;
			if (location.Y >= screenHeight)
				location.Y = screenHeight;

			state.Location = location;

			switch (mouseData.ButtonState)
			{
				case MouseButtonState.LeftDown:
					state.ButtonState = DeviceState.Down;
					break;
				case MouseButtonState.LeftUp:
					state.ButtonState = DeviceState.Up;
					break;
				case MouseButtonState.None:
					state.ButtonState = DeviceState.Move;
					break;
			}
		}
	}
}