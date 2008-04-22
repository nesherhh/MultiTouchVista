using System;
using System.Linq;
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

		public RawDevicesManager(InputProvider inputProvider)
		{
			this.inputProvider = inputProvider;
			devices = new DeviceCollection();
			contacts = new ContactCollection();

			foreach (RawDevice mouseDevice in RawDevice.GetRawDevices().Where(d => d.RawType == RawType.Mouse))
				devices.Add(new DeviceState(mouseDevice));

			Thread inputThread = new Thread(InputWorker);
			inputThread.IsBackground = true;
			inputThread.SetApartmentState(ApartmentState.STA);
			inputThread.Name = "MultipleMice thread";
			inputThread.Start();

			this.inputProvider.IsRunning = true;
		}

		void InputWorker()
		{
			RawDevice.RegisterRawDevices(0x01, 0x02, InputMode.BackgroundMode | InputMode.SuppressMessages);
			RawDevice.RawInput += RawDevice_RawInput;
			Application.Run();
		}

		public void Dispose()
		{
			RawDevice.UnregisterRawDevices(0x01, 0x02);
			RawDevice.RawInput -= RawDevice_RawInput;
			Application.Exit();
			inputProvider.IsRunning = false;
		}

		void RawDevice_RawInput(object sender, RawInputEventArgs e)
		{
			if (devices.Contains(e.Handle))
			{
				DeviceState state = devices[e.Handle];
				MouseData data = (MouseData)e.GetRawData();

				state.X += data.X;
				state.Y += data.Y;
				state.ButtonState = data.ButtonState;

				MouseContact contact = null;
				if (state.ButtonState == MouseButtonState.LeftDown)
				{
					contact = new MouseContact(state);
					contacts.Add(contact);
				}
				else if ((state.ButtonState == MouseButtonState.None || state.ButtonState == MouseButtonState.LeftUp) && contacts.Contains(e.Handle))
				{
					contact = contacts[e.Handle];
					contact.Update(state);
				}
				if (contact != null)
				{
					MouseContactChangedEventArgs args = new MouseContactChangedEventArgs(contact);
					inputProvider.OnContactChanged(args);
				}
				if (state.ButtonState == MouseButtonState.LeftUp)
					contacts.Remove(e.Handle);
			}
		}
	}
}