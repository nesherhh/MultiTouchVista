using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
	public class ApplicationInterfaceService : IApplicationInterface
	{
		IApplicationInterfaceCallback callback;
		IntPtr hWnd;
		Action<IContactData> contactChangedHandler;
		Action<IFrameData> frameHandler;

		static Dictionary<IntPtr, Receiver> receivers;
		Dictionary<ImageType, int> imagesToSend;

		static ApplicationInterfaceService()
		{
			receivers = new Dictionary<IntPtr, Receiver>();
		}

		public ApplicationInterfaceService()
		{
			imagesToSend = new Dictionary<ImageType, int>();
			foreach (ImageType value in Enum.GetValues(typeof(ImageType)))
				imagesToSend.Add(value, 0);
		}

		public void Subscribe(IntPtr windowHandle)
		{
			hWnd = windowHandle;
			callback = OperationContext.Current.GetCallbackChannel<IApplicationInterfaceCallback>();
			contactChangedHandler = OnContactChanged;
			frameHandler = OnFrame;

			receivers.Add(hWnd, new Receiver(contactChangedHandler));
		}

		public void Unsubscribe()
		{
			receivers.Remove(hWnd);
		}

		public void ReceiveFrames(bool value)
		{
			Receiver receiver;
			receivers.TryGetValue(hWnd, out receiver);

			if (receiver != null)
			{
				if (value)
					receiver.FrameHandler = frameHandler;
				else
					receiver.FrameHandler = null;
			}
		}

		public bool SendImageType(ImageType imageType, bool value)
		{
			if(value)
				imagesToSend[imageType]++;
			else
				imagesToSend[imageType]--;

			if (imagesToSend[imageType] > 0)
				InputProviderManager.Instance.Provider.SendImageType(imageType, true);
			else
				InputProviderManager.Instance.Provider.SendImageType(imageType, false);
			
			return true;
		}

		void OnContactChanged(IContactData contact)
		{
			callback.ContactChanged(contact.Id, contact.X, contact.Y, contact.Width, contact.Height, contact.Angle, contact.Bounds, contact.State);
		}

		void OnFrame(IFrameData frame)
		{
			FrameData data = new FrameData();
			data.SetImages(frame.Image.Where(i => imagesToSend[i.Type] > 0));
			callback.Frame(data);
		}

		public static void DispatchInput(InputDataEventArgs e)
		{
			switch (e.Type)
			{
				case InputType.Contact:
					HandleContact((IContactData)e.Data);
					break;
				case InputType.Frame:
					HandleFrame((IFrameData)e.Data);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		static void HandleFrame(IFrameData frame)
		{
			foreach (KeyValuePair<IntPtr, Receiver> keyValuePair in receivers.Where(pair => pair.Value.FrameHandler != null).ToList())
			{
				try
				{
					keyValuePair.Value.FrameHandler(frame);
				}
				catch (Exception)
				{
					receivers.Remove(keyValuePair.Key);
				}
			}
		}

		static void HandleContact(IContactData contact)
		{
			IntPtr hWnd = NativeMethods.WindowFromPoint(new NativeMethods.POINT((int)contact.X, (int)contact.Y));
			if (hWnd == IntPtr.Zero)
				DeliverContact(contact);
			else
				DeliverContact(hWnd, contact);
		}

		static void DeliverContact(IntPtr hWnd, IContactData contact)
		{
			Receiver receiver;
			if (receivers.TryGetValue(hWnd, out receiver))
			{
				try
				{
					receiver.ContactHandler(contact);
				}
				catch (Exception)
				{
					receivers.Remove(hWnd);
				}
			}
		}

		static void DeliverContact(IContactData contact)
		{
			List<KeyValuePair<IntPtr, Receiver>> handlers = receivers.ToList();
			foreach (KeyValuePair<IntPtr, Receiver> pair in handlers)
			{
				try
				{
					pair.Value.ContactHandler(contact);
				}
				catch (Exception)
				{
					receivers.Remove(pair.Key);
				}
			}
		}
	}
}