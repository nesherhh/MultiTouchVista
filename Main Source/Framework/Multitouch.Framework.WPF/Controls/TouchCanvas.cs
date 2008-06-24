using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using Multitouch.Framework.WPF.Input;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Controls
{
	public class TouchCanvas : InkCanvas
	{
		IDictionary<int, StylusPointCollection> points;
		Proc<InkCanvas, InkCanvasStrokeCollectedEventArgs, bool> raiseGestureOrStrokeCollectedMethod;

		public TouchCanvas()
		{
			points = new Dictionary<int, StylusPointCollection>();
			raiseGestureOrStrokeCollectedMethod = Dynamic<InkCanvas>.Instance.Procedure.Explicit<InkCanvasStrokeCollectedEventArgs, bool>.CreateDelegate("RaiseGestureOrStrokeCollected");

			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);

			StylusPlugIns.Add(new TestPlugin());

			EditingMode = InkCanvasEditingMode.InkAndGesture;
			SetEnabledGestures(new[]{ApplicationGesture.ScratchOut});
			Gesture += TouchCanvas_Gesture;
		}

		public class TestPlugin : StylusPlugIn
		{
			protected override void OnStylusDown(RawStylusInput rawStylusInput)
			{
				base.OnStylusDown(rawStylusInput);
			}

			protected override void OnStylusMove(RawStylusInput rawStylusInput)
			{
				base.OnStylusMove(rawStylusInput);
			}

			protected override void OnStylusUp(RawStylusInput rawStylusInput)
			{
				base.OnStylusUp(rawStylusInput);
			}

			protected override void OnStylusDownProcessed(object callbackData, bool targetVerified)
			{
				base.OnStylusDownProcessed(callbackData, targetVerified);
			}

			protected override void OnStylusMoveProcessed(object callbackData, bool targetVerified)
			{
				base.OnStylusMoveProcessed(callbackData, targetVerified);
			}

			protected override void OnStylusUpProcessed(object callbackData, bool targetVerified)
			{
				base.OnStylusUpProcessed(callbackData, targetVerified);
			}
		}

		void TouchCanvas_Gesture(object sender, InkCanvasGestureEventArgs e)
		{
			GestureRecognitionResult recognitionResult = e.GetGestureRecognitionResults().FirstOrDefault();
			if (recognitionResult != null && recognitionResult.RecognitionConfidence == RecognitionConfidence.Strong && recognitionResult.ApplicationGesture == ApplicationGesture.ScratchOut)
				Strokes.Clear();
		}

		protected virtual void OnNewContact(object sender, NewContactEventArgs e)
		{
			StylusPointCollection pointCollection = new StylusPointCollection();
			points.Add(e.Contact.Id, pointCollection);

			Point position = e.GetPosition(this);
			pointCollection.Add(new StylusPoint(position.X, position.Y));
		}

		protected virtual void OnContactMoved(object sender, ContactEventArgs e)
		{
			Point position = e.GetPosition(this);
			if(points.ContainsKey(e.Contact.Id))
				points[e.Contact.Id].Add(new StylusPoint(position.X, position.Y));
		}

		protected virtual void OnContactRemoved(object sender, ContactEventArgs e)
		{
			Point position = e.GetPosition(this);
			StylusPointCollection pointCollection = points[e.Contact.Id];
			pointCollection.Add(new StylusPoint(position.X, position.Y));
			points.Remove(e.Contact.Id);

			Stroke stroke = new Stroke(pointCollection);
            raiseGestureOrStrokeCollectedMethod(this, new InkCanvasStrokeCollectedEventArgs(stroke), true);
		}
	}
}