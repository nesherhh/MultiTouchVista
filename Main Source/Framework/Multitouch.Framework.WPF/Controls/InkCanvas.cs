using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Multitouch.Framework.WPF.Input;
using WpfInkCanvas = System.Windows.Controls.InkCanvas;

namespace Multitouch.Framework.WPF.Controls
{
	/// <summary>
	/// <see cref="System.Windows.Controls.InkCanvas"/>
	/// </summary>
	public class InkCanvas : FrameworkElement
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void InkCanvasStrokeErasingEventHandler(object sender, InkCanvasStrokeErasingEventArgs e);

		public static readonly DependencyProperty BackgroundProperty;
		public static readonly DependencyProperty DefaultDrawingAttributesProperty;
		public static readonly DependencyProperty EditingModeProperty;
		public static readonly DependencyProperty StrokesProperty;
		public static readonly RoutedEvent StrokeErasedEvent;
		public static readonly RoutedEvent StrokeCollectedEvent;
		public static readonly RoutedEvent EditingModeChangedEvent;

		/// <summary>
		/// 
		/// </summary>
		public event DrawingAttributesReplacedEventHandler DefaultDrawingAttributesReplaced;
		/// <summary>
		/// 
		/// </summary>
		public event InkCanvasStrokeErasingEventHandler StrokeErasing;
		/// <summary>
		/// 
		/// </summary>
		public event InkCanvasStrokesReplacedEventHandler StrokesReplaced;

		/// <summary>
		/// 
		/// </summary>
		public event RoutedEventHandler EditingModeChanged
		{
			add { AddHandler(EditingModeChangedEvent, value); }
			remove { RemoveHandler(EditingModeChangedEvent, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public event InkCanvasStrokeCollectedEventHandler StrokeCollected
		{
			add { AddHandler(StrokeCollectedEvent, value); }
			remove { RemoveHandler(StrokeCollectedEvent, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public event RoutedEventHandler StrokeErased
		{
			add { AddHandler(StrokeErasedEvent, value); }
			remove { RemoveHandler(StrokeErasedEvent, value); }
		}

		private readonly System.Windows.Controls.InkCanvas inkCanvas;
		private readonly object key;

		/// <summary>
		/// 
		/// </summary>
		public DrawingAttributes DefaultDrawingAttributes
		{
			get { return (DrawingAttributes)GetValue(DefaultDrawingAttributesProperty); }
			set { SetValue(DefaultDrawingAttributesProperty, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public InkCanvasEditingMode EditingMode
		{
			get { return (InkCanvasEditingMode)GetValue(EditingModeProperty); }
			set { SetValue(EditingModeProperty, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public StrokeCollection Strokes
		{
			get { return (StrokeCollection)GetValue(StrokesProperty); }
			set { SetValue(StrokesProperty, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public StylusShape EraserShape
		{
			get { return inkCanvas.EraserShape; }
			set { inkCanvas.EraserShape = value; }
		}

		static InkCanvas()
		{
			BackgroundProperty = WpfInkCanvas.BackgroundProperty.AddOwner(typeof(InkCanvas));
			DefaultDrawingAttributesProperty = WpfInkCanvas.DefaultDrawingAttributesProperty.AddOwner(typeof(InkCanvas));
			StrokesProperty = WpfInkCanvas.StrokesProperty.AddOwner(typeof(InkCanvas));
			StrokeErasedEvent = WpfInkCanvas.StrokeErasedEvent.AddOwner(typeof(InkCanvas));
			StrokeCollectedEvent = WpfInkCanvas.StrokeCollectedEvent.AddOwner(typeof(InkCanvas));
			EditingModeChangedEvent = EventManager.RegisterRoutedEvent("EditingModeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InkCanvas));
			EditingModeProperty = DependencyProperty.Register("EditingMode", typeof(InkCanvasEditingMode), typeof(InkCanvas),
				new FrameworkPropertyMetadata(InkCanvasEditingMode.Ink, new PropertyChangedCallback(OnEditingModeChanged)), ValidateEditingMode);
		}

		/// <summary>
		/// 
		/// </summary>
		public InkCanvas()
		{
			MultitouchScreen.AddNewContactHandler(this, OnNewContact);
			MultitouchScreen.AddContactLeaveHandler(this, OnContactLeave);
			MultitouchScreen.AddContactMovedHandler(this, OnContactMove);

			inkCanvas = new System.Windows.Controls.InkCanvas();
			key = new object();
			DefaultDrawingAttributes = new DrawingAttributes();
			Strokes = new StrokeCollection();

			BindToInkCanvas(BackgroundProperty);
			BindToInkCanvas(DefaultDrawingAttributesProperty);
			BindToInkCanvas(EditingModeProperty);
			BindToInkCanvas(StrokesProperty);

			inkCanvas.DefaultDrawingAttributesReplaced += OnDefaultDrawingAttributesReplaced;
			inkCanvas.StrokeErasing += OnStrokeErasing;
			inkCanvas.StrokesReplaced += OnStrokesReplaced;
			inkCanvas.StrokeCollected += OnCollected;
		}

		protected virtual void OnCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
		{
			OnStrokeCollected(e);
		}

		protected virtual void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
		{
			RaiseEvent(e);
		}

		protected virtual void OnStrokesReplaced(object sender, InkCanvasStrokesReplacedEventArgs e)
		{
			InkCanvasStrokesReplacedEventHandler handler = StrokesReplaced;
			if (handler != null)
				handler(this, e);
		}

		protected virtual void OnStrokeErasing(object sender, System.Windows.Controls.InkCanvasStrokeErasingEventArgs e)
		{
			InkCanvasStrokeErasingEventArgs e2 = new InkCanvasStrokeErasingEventArgs(e.Stroke);
			OnStrokeErasing(e2);
		}

		private void OnStrokeErasing(InkCanvasStrokeErasingEventArgs e)
		{
			InkCanvasStrokeErasingEventHandler handler = StrokeErasing;
			if (handler != null)
				handler(this, e);
		}

		protected virtual void OnDefaultDrawingAttributesReplaced(object sender, DrawingAttributesReplacedEventArgs e)
		{
			DrawingAttributesReplacedEventHandler handler = DefaultDrawingAttributesReplaced;
			if (handler != null)
				handler(this, e);
		}

		private void BindToInkCanvas(DependencyProperty property)
		{
			Binding binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(property.Name);
			inkCanvas.SetBinding(property, binding);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			inkCanvas.Arrange(new Rect(finalSize));
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			inkCanvas.Measure(availableSize);
			return inkCanvas.DesiredSize;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			AddVisualChild(inkCanvas);
		}

		protected override Visual GetVisualChild(int index)
		{
			return inkCanvas;
		}

		protected override int VisualChildrenCount
		{
			get { return 1; }
		}

		protected virtual void OnContactLeave(object sender, ContactEventArgs e)
		{
			if (e.Contact.Captured == this)
			{
				switch (EditingMode)
				{
					case InkCanvasEditingMode.Ink:
						EndStroke(e.Contact);
						break;
					default:
						break;
				}

				e.Handled = true;
				e.Contact.Capture(null);
			}
		}

		protected virtual void OnContactMove(object sender, ContactEventArgs e)
		{
			if (e.Contact.Captured == this)
			{
				switch (EditingMode)
				{
					case InkCanvasEditingMode.EraseByStroke:
						RemovePartialStroke(e.Contact);
						EraseStroke(e.Contact);
						break;
					case InkCanvasEditingMode.EraseByPoint:
						RemovePartialStroke(e.Contact);
						ErasePoint(e.Contact);
						break;
					case InkCanvasEditingMode.Ink:
						AddPointsToStroke(e.Contact);
						break;
					default:
						break;
				}

				e.Handled = true;
			}
		}

		private void RemovePartialStroke(Contact contact)
		{
			Stroke stroke = (Stroke) contact.GetUserData(key);
			if(stroke != null)
			{
				inkCanvas.Strokes.Remove(stroke);
				contact.SetUserData(key, null);
			}
		}

		protected virtual void OnNewContact(object sender, NewContactEventArgs e)
		{
			switch (EditingMode)
			{
				case InkCanvasEditingMode.EraseByStroke:
					EraseStroke(e.Contact);
					break;
				case InkCanvasEditingMode.EraseByPoint:
					ErasePoint(e.Contact);
					break;
				case InkCanvasEditingMode.Ink:
					StartStroke(e.Contact);
					break;
				default:
					break;
			}
			e.Contact.Capture(this);
			e.Handled = true;
		}

		private void ErasePoint(Contact contact)
		{
			Point position = contact.GetPosition(this);
			Point[] points = new[] { position };
			foreach (Stroke stroke in inkCanvas.Strokes.HitTest(points, EraserShape))
			{
				InkCanvasStrokeErasingEventArgs e = new InkCanvasStrokeErasingEventArgs(stroke);
				OnStrokeErasing(e);
				if (!e.Cancel)
				{
					StrokeCollection eraseResult = stroke.GetEraseResult(points, EraserShape);
					inkCanvas.Strokes.Replace(stroke, eraseResult);
					RoutedEventArgs e2 = new RoutedEventArgs(StrokeErasedEvent, this);
					RaiseEvent(e2);
				}
			}
		}

		private void EraseStroke(Contact contact)
		{
			Point position = contact.GetPosition(this);
			foreach (Stroke stroke in inkCanvas.Strokes.HitTest(new[] {position}, EraserShape))
			{
				InkCanvasStrokeErasingEventArgs e = new InkCanvasStrokeErasingEventArgs(stroke);
				OnStrokeErasing(e);
				if (!e.Cancel)
				{
					inkCanvas.Strokes.Remove(stroke);
					RoutedEventArgs e2 = new RoutedEventArgs(StrokeErasedEvent, this);
					RaiseEvent(e2);
				}
			}
		}

		protected virtual void StartStroke(Contact contact)
		{
			StylusPointCollection stylusPoints = new StylusPointCollection();
			Point position = contact.GetPosition(this);
			stylusPoints.Add(new StylusPoint(position.X, position.Y, 0.5f));
			Stroke stroke = new Stroke(stylusPoints, DefaultDrawingAttributes);
			inkCanvas.Strokes.Add(stroke);
			contact.SetUserData(key, stroke);
		}

		protected virtual void AddPointsToStroke(Contact contact)
		{
			Stroke stroke = (Stroke)contact.GetUserData(key);
			if (stroke == null)
				StartStroke(contact);
			else
			{
				StylusPointCollection stylusPoints = stroke.StylusPoints;
				if (stylusPoints != null)
				{
					Point position = contact.GetPosition(this);
					stylusPoints.Add(new StylusPoint(position.X, position.Y, 0.5f));
				}
			}
		}

		protected virtual void EndStroke(Contact contact)
		{
			Stroke stroke = (Stroke)contact.GetUserData(key);
			if (stroke != null)
			{
				InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(stroke);
				OnStrokeCollected(args);
			}
		}

		private static void OnEditingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InkCanvas canvas = d as InkCanvas;
			if (canvas != null)
			{
				canvas.inkCanvas.EditingMode = (InkCanvasEditingMode) e.NewValue;
				canvas.OnEditingModeChanged(new RoutedEventArgs(EditingModeChangedEvent, canvas));
			}
		}

		protected virtual void OnEditingModeChanged(RoutedEventArgs e)
		{
			RaiseEvent(e);
		}

		private static bool ValidateEditingMode(object value)
		{
			InkCanvasEditingMode mode = (InkCanvasEditingMode)value;
			return mode == InkCanvasEditingMode.EraseByPoint
				   || mode == InkCanvasEditingMode.EraseByStroke
				   || mode == InkCanvasEditingMode.Ink
				   || mode == InkCanvasEditingMode.None;
		}
	}
}