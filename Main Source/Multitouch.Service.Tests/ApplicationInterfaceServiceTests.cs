using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multitouch.Contracts;
using Multitouch.Service.Logic.ExternalInterfaces;
using Multitouch.Service.Tests.Services;
using IApplicationInterface=Multitouch.Service.Tests.Services.IApplicationInterface;
using IApplicationInterfaceCallback=Multitouch.Service.Tests.Services.IApplicationInterfaceCallback;

namespace Multitouch.Service.Tests
{
	/// <summary>
	/// Summary description for ApplicationInterfaceServiceTests
	/// </summary>
	[TestClass, Ignore] //because of race conditions runs only with debugger.
	public class ApplicationInterfaceServiceTests : BaseTestForApplicationInterfaceService
	{
		int frameCounter;
		
		[TestMethod]
		public void AddZeroHandler()
		{
			using(ApplicationInterfaceClient client = CreateSession(new Callback(this)))
			{
				Thread.Sleep(TimeSpan.FromSeconds(10));
				client.AddWindowHandleToSession(IntPtr.Zero);

				Assert.AreEqual(0, frameCounter);

				InputProviderManager.applicationService.DispatchFrame(CreateNewFrameArguments(new Point(1, 1)));

				Thread.Sleep(TimeSpan.FromSeconds(20));

				Assert.AreEqual(1, frameCounter);

				client.RemoveSession();
			}
		}

		[TestMethod]
		public void AddNonZeroHandler()
		{
			frameCounter = 0;
			using(ApplicationInterfaceClient client = CreateSession(new Callback(this)))
			{
				Thread.Sleep(TimeSpan.FromSeconds(10));

				Process devEnvProcess = Process.GetProcessesByName("devenv").FirstOrDefault();
				Assert.IsNotNull(devEnvProcess);

				client.AddWindowHandleToSession(devEnvProcess.MainWindowHandle);

				Assert.AreEqual(0, frameCounter);

				InputProviderManager.applicationService.DispatchFrame(CreateNewFrameArguments(new Point(1, 1)));

				Thread.Sleep(TimeSpan.FromSeconds(20));

				Assert.AreEqual(1, frameCounter);

				client.RemoveSession();
			}
		}

		[TestMethod]
		public void AddZeroAndNonZero()
		{
			frameCounter = 0;
			using (ApplicationInterfaceClient devEnvClient = CreateSession(new Callback(this)))
			{
				Thread.Sleep(TimeSpan.FromSeconds(10));

				Process devEnvProcess = Process.GetProcessesByName("devenv").FirstOrDefault();
				Assert.IsNotNull(devEnvProcess);

				devEnvClient.AddWindowHandleToSession(devEnvProcess.MainWindowHandle);

				using (ApplicationInterfaceClient client = CreateSession(new Callback(this)))
				{
					Thread.Sleep(TimeSpan.FromSeconds(10));
					client.AddWindowHandleToSession(IntPtr.Zero);

					Assert.AreEqual(0, frameCounter);

					InputProviderManager.applicationService.DispatchFrame(CreateNewFrameArguments(new Point(1, 1)));

					Thread.Sleep(TimeSpan.FromSeconds(20));

					Assert.AreEqual(2, frameCounter);

					client.RemoveSession();
				}

				devEnvClient.RemoveSession();
			}
		}

		[TestMethod]
		public void AddZeroAndZero()
		{
			frameCounter = 0;
			using (ApplicationInterfaceClient client1 = CreateSession(new Callback(this)))
			{
				Thread.Sleep(TimeSpan.FromSeconds(10));

				client1.AddWindowHandleToSession(IntPtr.Zero);
				client1.SendEmptyFrames(true);

				using (ApplicationInterfaceClient client2 = CreateSession(new Callback(this)))
				{
					Thread.Sleep(TimeSpan.FromSeconds(10));
					client2.AddWindowHandleToSession(IntPtr.Zero);
					client2.SendEmptyFrames(true);
					Assert.AreEqual(0, frameCounter);

					InputProviderManager.applicationService.DispatchFrame(CreateNewFrameArguments(new Point(1, 1)));

					Thread.Sleep(TimeSpan.FromSeconds(20));

					Assert.AreEqual(2, frameCounter);

					client2.RemoveSession();
				}

				client1.RemoveSession();
			}
		}

		static NewFrameEventArgs CreateNewFrameArguments(Point position)
		{
			List<Contact> contacts = new List<Contact>();
			contacts.Add(new Contact(0, ContactState.Moved, position, 10, 10));
			return new NewFrameEventArgs(Stopwatch.GetTimestamp(), contacts, Enumerable.Empty<Image>());
		}

		internal static ApplicationInterfaceClient CreateSession(IApplicationInterfaceCallback callback)
		{
			Uri serviceAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			EndpointAddress remoteAddress = new EndpointAddress(serviceAddress);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			namedPipeBinding.MaxReceivedMessageSize = int.MaxValue;
			namedPipeBinding.MaxBufferSize = int.MaxValue;
			namedPipeBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;

			ApplicationInterfaceClient session = new ApplicationInterfaceClient(new InstanceContext(callback), namedPipeBinding, remoteAddress);
			session.CreateSession();
			return session;
		}

		internal void RemoveSession(IApplicationInterface service)
		{
			service.RemoveSession();
			IDisposable client = service as IDisposable;
			if (client != null)
				client.Dispose();
		}

		class Callback : IApplicationInterfaceCallback
		{
			readonly ApplicationInterfaceServiceTests parent;

			public Callback(ApplicationInterfaceServiceTests parent)
			{
				this.parent = parent;
			}

			public void Frame(FrameData data)
			{
				parent.frameCounter++;
			}
		}
	}
}
