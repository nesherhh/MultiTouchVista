using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.Logic.Providers;

namespace Danilins.Multitouch.Logic
{
	public class InputProviderLogic : Common.Logics.Logic
	{
		private IInputProvider currentProvider;
		private PreviewPixelHandler previewInputHandler;
		private PreviewPixelHandler previewOutputHandler;

		public bool ShowPreview { get; set; }

		public InputProviderLogic(IServiceProvider parent)
			: base(parent)
		{
			SelectedProviderId = GetLogic<ConfigurationLogic>().Section.InputProvider;
		}

		public Guid? SelectedProviderId
		{
			get
			{
				if(currentProvider != null)
					return currentProvider.Id;
				return null;
			}
			set
			{
				if (IsRunning)
					throw new MultitouchException(string.Format("InputProvider ('{0}') already running.", currentProvider.Id));
				if (currentProvider != null)
					currentProvider.Dispose();

				currentProvider = InputProviderFactory.Create(value.Value);

				GetLogic<ConfigurationLogic>().Section.InputProvider = value.Value;
			}
		}

		public PreviewPixelHandler PreviewInputHandler
		{
			get { return previewInputHandler; }
			set { previewInputHandler = value; }
		}

		public PreviewPixelHandler PreviewOutputHandler
		{
			get { return previewOutputHandler; }
			set { previewOutputHandler = value; }
		}

		public IInputProvider CurrentProvider
		{
			get { return currentProvider; }
		}

		public bool IsRunning
		{
			get { return currentProvider != null && currentProvider.IsRunning; }
		}

		public void Start()
		{
			if (currentProvider == null)
				throw new MultitouchException("No input provider selected");
			if (!currentProvider.IsRunning)
			{
				currentProvider.SetPreviewInputHandler(PreviewInputHandler);
				currentProvider.SetPreviewOutputHandler(PreviewOutputHandler);
				currentProvider.ShowPreview = ShowPreview;
				currentProvider.Start();
			}
		}

		public void Stop()
		{
			if (currentProvider != null && currentProvider.IsRunning)
			{
				currentProvider.Stop();
				currentProvider.SetPreviewInputHandler(null);
				currentProvider.SetPreviewOutputHandler(null);
			}
		}

		public InputProviderCollection AvailableProviders
		{
			get
			{
				InputProviderCollection result = new InputProviderCollection();

				var list = from type in LogicUtility.ProvidersAssembly.GetTypes()
						   let attr = (InputProviderAttribute[])type.GetCustomAttributes(typeof(InputProviderAttribute), false)
						   where attr.Length > 0
						   orderby attr[0].Name
						   select new { attr[0].Id, attr[0].Name, type, attr[0].HasConfiguration };

				foreach (var item in list)
					result.Add(new InputProviderInfo(item.Id, item.Name, item.type, item.HasConfiguration));
				return result;
			}
		}
	}
}