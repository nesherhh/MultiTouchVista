using System;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// 
	/// </summary>
	public class ContactHandler : IDisposable
	{
		bool receiveEmptyFrames;
		CommunicationLogic logic;

		public event EventHandler<FrameEventArgs> Frame;
		public event EventHandler<ContactEventArgs> NewContact;
		public event EventHandler<ContactEventArgs> ContactMoved;
		public event EventHandler<ContactEventArgs> ContactRemoved;

		/// <summary>
		/// Window handle.
		/// </summary>
		/// <value>The handle.</value>
		public IntPtr Handle { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContactHandler"/> class.
		/// </summary>
		/// <param name="handle">The handle.</param>
		public ContactHandler(IntPtr handle)
		{
			Handle = handle;
			logic = CommunicationLogic.Instance;
			logic.RegisterHandler(this);
		}

		/// <summary>
		/// Gets or sets a value indicating whether Frame event will be raise if no contact events are present.
		/// </summary>
		public bool ReceiveEmptyFrames
		{
			get { return receiveEmptyFrames; }
			set
			{
				if (receiveEmptyFrames != value)
				{
					receiveEmptyFrames = value;
					logic.ReceiveEmptyFrames(value);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			logic.UnregisterHandler(this);
		}

		/// <summary>
		/// Sets a value indicating whether you want to receive image of a specified type.
		/// </summary>
		/// <param name="imageType">Type of the image.</param>
		/// <param name="value">if set to <c>true</c> images will be received.</param>
		/// <returns>Returns a value indicating whether image of specified type will be realy received</returns>
		public bool ReceiveImageType(ImageType imageType, bool value)
		{
			Service.ImageType type;
			switch (imageType)
			{
				case ImageType.Normalized:
					type = Service.ImageType.Normalized;
					break;
				case ImageType.Binarized:
					type = Service.ImageType.Binarized;
					break;
				default:
					throw new ArgumentOutOfRangeException("imageType");
			}
			return logic.SendImageType(type, value);
		}

		internal void HandleContact(ContactData contact, long timestamp)
		{
			EventHandler<ContactEventArgs> handler;
			switch(contact.State)
			{
				case Service.ContactState.New:
					handler = NewContact;
					break;
				case Service.ContactState.Removed:
					handler = ContactRemoved;
					break;
				case Service.ContactState.Moved:
					handler = ContactMoved;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			if(handler != null)
				handler(this, new ContactEventArgs(contact, timestamp));
		}

		internal void HandleFrame(FrameData frame)
		{
			EventHandler<FrameEventArgs> eventHandler = Frame;
			if(eventHandler != null)
				eventHandler(this, new FrameEventArgs(frame));
		}
	}
}
