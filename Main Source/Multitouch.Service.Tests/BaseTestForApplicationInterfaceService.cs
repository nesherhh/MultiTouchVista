using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multitouch.Contracts;
using Multitouch.Service.Logic;

namespace Multitouch.Service.Tests
{
	public class BaseTestForApplicationInterfaceService
	{
		AsyncOperation operation;
		protected InputProviderManager_Accessor InputProviderManager { get; private set; }

		[TestInitialize]
		public void TestInit()
		{
			ManualResetEvent waitHandle = new ManualResetEvent(false);
			ThreadPool.QueueUserWorkItem(o =>
			{
				operation = AsyncOperationManager.CreateOperation(null);
				InputProviderManager = new InputProviderManager_Accessor(new Provider());
				waitHandle.Set();
				Application.Run();
			});
			waitHandle.WaitOne();
		}

		[TestCleanup]
		public void TestClean()
		{
			operation.SynchronizationContext.Send(o => InputProviderManager.Dispose(), null);
			Application.Exit();
		}

		class Provider : IProvider
		{
			public void Start()
			{ }

			public void Stop()
			{ }

			public bool IsRunning
			{
				get { return true; }
			}

			public bool HasConfiguration
			{
				get { return false; }
			}

			public UIElement GetConfiguration()
			{
				throw new NotImplementedException();
			}

			public bool SendImageType(ImageType imageType, bool value)
			{
				return false;
			}

			public bool SendEmptyFrames { get; set; }

			public event EventHandler<NewFrameEventArgs> NewFrame;
		}
	}
}
