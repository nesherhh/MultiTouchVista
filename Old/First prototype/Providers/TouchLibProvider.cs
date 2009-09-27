using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Providers;
using TouchlibWrapper;

namespace Danilins.Multitouch.Providers
{
	[InputProvider(ID, "TouchLib", false)]
	public class TouchLibProvider : InputProvider
	{
		CsTI instance;
		const string ID = "{232668E3-BFDB-4866-8871-5178AF8EB333}";

		Queue<ContactInfo> contacts;
		ReaderWriterLockSlim rwLock;
		Thread runThread;

		public TouchLibProvider()
		{
			rwLock = new ReaderWriterLockSlim();
			contacts = new Queue<ContactInfo>();
		}

		public override void Dispose()
		{
			Stop();
		}

		protected override ContactInfo[] GetContactsCore()
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				ContactInfo[] result = new ContactInfo[contacts.Count];
				Array.Copy(contacts.ToArray(), result, contacts.Count);

				rwLock.EnterWriteLock();
				contacts.Clear();
				rwLock.ExitWriteLock();
				return result;
			}
			finally
			{
				rwLock.ExitUpgradeableReadLock();
			}
		}

		public override Guid Id
		{
			get { return new Guid(ID); }
		}

		public override string Name
		{
			get { return "TouchLib"; }
		}

		public override void Start()
		{
			instance = CsTI.Instance();
			instance.fingerDown += OnFingerDown;
			instance.fingerUp += OnFingerUp;
			instance.fingerUpdate += OnFingerUpdate;

			runThread = new Thread(StartInstance);
			runThread.IsBackground = true;
			runThread.Start();
		}

		void StartInstance(object state)
		{
			isRunning = true;
			instance.startScreen();
		}

		public override void Stop()
		{
			if (instance != null)
			{
				runThread.Abort();
				isRunning = false;
				instance.fingerDown -= OnFingerDown;
				instance.fingerUp -= OnFingerUp;
				instance.fingerUpdate -= OnFingerUpdate;
				instance = null;
			}
		}

		void OnFingerDown(int id, int tagID, float X, float Y, float angle, float area, float height, float width, float dX, float dY)
		{
			rwLock.EnterWriteLock();
			try
			{
				ContactInfo contactInfo = new ContactInfo(new Rect(X, Y, Math.Abs(width), Math.Abs(height)));
				contactInfo.State = ContactState.Down;
				contactInfo.Id = id;
				contactInfo.Delta = new Vector(dX, dY);
				contacts.Enqueue(contactInfo);
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		void OnFingerUp(int id, int tagID, float X, float Y, float angle, float area, float height, float width, float dX, float dY)
		{
			rwLock.EnterWriteLock();
			try
			{
				ContactInfo contactInfo = new ContactInfo(new Rect(X, Y, Math.Abs(width), Math.Abs(height)));
				contactInfo.State = ContactState.Up;
				contactInfo.Id = id;
				contactInfo.Delta = new Vector(dX, dY);
				contacts.Enqueue(contactInfo);
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		void OnFingerUpdate(int id, int tagID, float X, float Y, float angle, float area, float height, float width, float dX, float dY)
		{
			rwLock.EnterWriteLock();
			try
			{
				ContactInfo contactInfo = new ContactInfo(new Rect(X, Y, Math.Abs(width), Math.Abs(height)));
				contactInfo.State = ContactState.Move;
				contactInfo.Id = id;
				contactInfo.Delta = new Vector(dX, dY);
				contacts.Enqueue(contactInfo);
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}
	}
}