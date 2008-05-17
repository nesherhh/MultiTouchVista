using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	delegate void RawMultitouchGestureReportHandler(object sender, RawMultitouchGestureReport e);

	class GestureManager
	{
		GestureRecognizer gestureRecognizer;
		Dictionary<int, StylusPointCollection> contactsHistory;

		internal static readonly RoutedEvent RawGestureInputEvent = EventManager.RegisterRoutedEvent("RawGestureInput", RoutingStrategy.Bubble,
			typeof(RawMultitouchGestureReportHandler), typeof(GestureManager));


		public GestureManager()
		{
			contactsHistory = new Dictionary<int, StylusPointCollection>();
		}

		GestureRecognizer GestureRecognizer
		{
			get
			{
				if (gestureRecognizer == null)
					gestureRecognizer = new GestureRecognizer();
				return gestureRecognizer;
			}
		}

		public bool IsGestureEnabled
		{
			get { return MultitouchScreen.IsGesturesEnabled && GestureRecognizer.IsRecognizerAvailable; }
		}

		public void PreProcessInput(PreProcessInputEventArgs e)
		{
			RawMultitouchReport report = (RawMultitouchReport)e.StagingItem.Input;
			Contact contact = report.Contact;

			StylusPointCollection pointCollection;
			if (contact.State == ContactState.New)
			{
				pointCollection = new StylusPointCollection();
				contactsHistory.Add(contact.Id, pointCollection);
			}
			else
				pointCollection = contactsHistory[contact.Id];

			pointCollection.Add(new StylusPoint(contact.X, contact.Y));
		}

		public void PostProcessInput(ProcessInputEventArgs e)
		{
			if (e.StagingItem.Input.RoutedEvent == MultitouchLogic.RawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null && !report.Handled)
				{
					Contact contact = report.Contact;
					if (contact.State == ContactState.Removed)
					{
						StrokeCollection strokeCollection = new StrokeCollection { new Stroke(contactsHistory[contact.Id]) };
						GestureRecognitionResult result = GestureRecognizer.Recognize(strokeCollection).FirstOrDefault();
						if (result != null && result.RecognitionConfidence == RecognitionConfidence.Strong)
						{
							RawMultitouchGestureReport gestureReport = new RawMultitouchGestureReport(report, result.ApplicationGesture);
							gestureReport.RoutedEvent = RawGestureInputEvent;
							e.InputManager.ProcessInput(gestureReport);
						}
					}
				}
			}
			else if (e.StagingItem.Input.RoutedEvent == RawGestureInputEvent)
				PromoteRawToPreview((RawMultitouchGestureReport)e.StagingItem.Input, e);
			else if (e.StagingItem.Input.RoutedEvent == MultitouchScreen.PreviewGestureEvent)
				PromotePreviewToMain(e);
		}

		void PromoteRawToPreview(RawMultitouchGestureReport report, ProcessInputEventArgs e)
		{
			GestureEventArgs args = new GestureEventArgs(report, Environment.TickCount);
			args.RoutedEvent = MultitouchScreen.PreviewGestureEvent;
			e.PushInput(args, e.StagingItem);
		}

		void PromotePreviewToMain(ProcessInputEventArgs e)
		{
			if (!e.StagingItem.Input.Handled)
			{
				GestureEventArgs previewArgs = (GestureEventArgs)e.StagingItem.Input;
				GestureEventArgs args = new GestureEventArgs(previewArgs, previewArgs.Timestamp);
				args.RoutedEvent = MultitouchScreen.GestureEvent;
				e.PushInput(args, e.StagingItem);
			}
		}

		public void EnableGestures(ApplicationGesture[] gestures)
		{
			if(IsGestureEnabled || gestures != null)
				gestureRecognizer.SetEnabledGestures(gestures);
		}
	}
}