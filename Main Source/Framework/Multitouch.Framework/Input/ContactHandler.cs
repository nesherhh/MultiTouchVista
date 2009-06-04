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
		readonly CommunicationLogic logic;

		/// <summary>
		/// Indicates that a frame from camera is received
		/// </summary>
		public event EventHandler<FrameEventArgs> Frame;
		/// <summary>
		/// A new contact is on surface
		/// </summary>
		public event EventHandler<ContactEventArgs> NewContact;
		/// <summary>
		/// Contact has been moved
		/// </summary>
		public event EventHandler<ContactEventArgs> ContactMoved;
		/// <summary>
		/// Contact has been removed
		/// </summary>
		public event EventHandler<ContactEventArgs> ContactRemoved;

		/// <summary>
		/// Window handle.
		/// </summary>
		/// <value>The handle.</value>
		public IntPtr Handle { get; private set; }

		/// <summary>
		/// x,y will be relative to this window handle
		/// </summary>
		public IntPtr RelativeTo { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContactHandler"/> class.
		/// </summary>
		/// <param name="handle">Window handle for which contacts should be received</param>
		/// <remarks>
		/// If /// <paramref name="handle"/> is <c>IntPtr.Zero</c> contacts for all windows will be received
		/// </remarks>
		public ContactHandler(IntPtr handle)
			: this(handle, handle)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContactHandler"/> class.
		/// </summary>
		/// <param name="handle">Window handle for which contacts should be received</param>
		/// <param name="relativeTo">Contacts coordinates will be relative to this window handle</param>
		/// <remarks>
		/// If <paramref name="handle"/> is <c>IntPtr.Zero</c> contacts for all windows will be received
		/// Use <c>IntPtr.Zero</c> in <paramref name="relativeTo"/> to get x,y in screen coordinates.
		/// </remarks>
		public ContactHandler(IntPtr handle, IntPtr relativeTo)
		{
			Handle = handle;
			logic = CommunicationLogic.Instance;
			logic.RegisterHandler(this);
			RelativeTo = relativeTo;
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
			if(disposing)
				logic.UnregisterHandler(this);
		}

		/// <summary>
		/// Sets a value indicating whether you want to receive image of a specified type.
		/// </summary>
		/// <param name="imageType">Type of the image.</param>
		/// <param name="value">if set to <c>true</c> images will be received.</param>
		/// <returns>Returns a value indicating whether image of specified type will be really received</returns>
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
				handler(this, new ContactEventArgs(contact, RelativeTo, timestamp));
		}

		internal void HandleFrame(FrameData frame)
		{
			EventHandler<FrameEventArgs> eventHandler = Frame;
			if(eventHandler != null)
				eventHandler(this, new FrameEventArgs(frame, RelativeTo));
		}
	}
}
