using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AdvanceMath;
using Danilins.Multitouch.Common;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;

namespace Danilins.Multitouch.PhysicsLibrary
{
	public class PhysicsController : Decorator
	{
		PhysicsEngine engine;
		PhysicsTimer timer;
		Dictionary<FrameworkElement, Body> itemToBody;

		#region Attached Properties
		public static readonly DependencyProperty IsCollidableProperty = DependencyProperty.RegisterAttached("IsCollidable", typeof(bool),
			typeof(PhysicsController), new UIPropertyMetadata(true, IsCollidableChanged));

		public static readonly DependencyProperty EnablePhysicsProperty = DependencyProperty.RegisterAttached("EnablePhysics", typeof(bool),
			typeof(PhysicsController), new UIPropertyMetadata(false, EnablePhysicsChanged));

		public static bool GetIsCollidable(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsCollidableProperty);
		}

		public static void SetIsCollidable(DependencyObject obj, bool value)
		{
			obj.SetValue(IsCollidableProperty, value);
		}

		public static bool GetEnablePhysics(DependencyObject obj)
		{
			return (bool)obj.GetValue(EnablePhysicsProperty);
		}

		public static void SetEnablePhysics(DependencyObject obj, bool value)
		{
			obj.SetValue(EnablePhysicsProperty, value);
		}

		private static void IsCollidableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PhysicsController physicsController = FindParentPhysicsController(d);
			if (physicsController != null)
			{
				foreach (Body body in physicsController.Engine.Bodies)
				{
					if (body.Tag != null && body.Tag.Equals(d))
						body.IsCollidable = (bool)e.NewValue;
				}
			}
		}

		private static void EnablePhysicsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement element = d as FrameworkElement;
			if (element == null)
				throw new ArgumentException("Only FrameworkElement, and it descendants, are supported");

			PhysicsController controller = FindParentPhysicsController(element);
			if (controller != null)
			{
				if ((bool)e.NewValue)
					element.SizeChanged += controller.Element_SizeChanged;
				else
				{
					element.SizeChanged -= controller.Element_SizeChanged;
					Body body;
					if(controller.itemToBody.TryGetValue(element, out body))
					{
						body.Lifetime.IsExpired = true;
						controller.itemToBody.Remove(element);
					}
				}
			}
		}

		public static PhysicsController FindParentPhysicsController(DependencyObject element)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(element);
			if (parent is PhysicsController)
				return parent as PhysicsController;
			else if (parent != null)
				return FindParentPhysicsController(parent);
			else
				return null;
		} 
		#endregion

		public PhysicsController()
		{
			itemToBody = new Dictionary<FrameworkElement, Body>();
			engine = new PhysicsEngine();
			engine.BroadPhase = new SweepAndPruneDetector();
			engine.Solver = new SequentialImpulsesSolver();
			timer = new PhysicsTimer(PhysicsTimerCallback, 0.01);
			if(!ViewUtility.IsDesignTime)
				timer.IsRunning = true;
		}

		private void PhysicsTimerCallback(double dt)
		{
			engine.Update(dt);
			UpdateChildren();
		}

		private void UpdateChildren()
		{
			foreach (Body body in engine.Bodies)
			{
				FrameworkElement element = body.Tag as FrameworkElement;
				if (element == null)
					continue;

				Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
				                                                     	{
				                                                     		double newLeft = body.State.Position.Linear.X - (element.ActualWidth / 2);
				                                                     		double newTop = body.State.Position.Linear.Y - (element.ActualHeight / 2);
				                                                     		if (Canvas.GetLeft(element) != newLeft)
				                                                     			Canvas.SetLeft(element, newLeft);
				                                                     		if(Canvas.GetTop(element) != newTop)
				                                                     			Canvas.SetTop(element, newTop);

				                                                     		double offsetX = element.ActualWidth / 2;
				                                                     		double offsetY = element.ActualHeight / 2;
				                                                     		RotateTransform transform = element.RenderTransform as RotateTransform;
				                                                     		if (transform == null)
				                                                     		{
				                                                     			transform = new RotateTransform();
				                                                     			element.RenderTransform = transform;
				                                                     		}
				                                                     		double angleInDegrees = MathHelper.ToDegrees(body.State.Position.Angular);
				                                                     		if(transform.Angle != angleInDegrees)
				                                                     			transform.Angle = angleInDegrees;
				                                                     		if(transform.CenterX != offsetX)
				                                                     			transform.CenterX = offsetX;
				                                                     		if(transform.CenterY != offsetY)
				                                                     			transform.CenterY = offsetY;
				                                                     	});
			}
		}

		public PhysicsEngine Engine
		{
			get { return engine; }
		}

		private void Element_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			FrameworkElement element = sender as FrameworkElement;
			if (element != null)
			{
				Body body;
				if (itemToBody.TryGetValue(element, out body))
					UpdateSize(body, e);
				else
					CreateNewBody(element);
			}
		}

		private void UpdateSize(Body body, SizeChangedEventArgs e)
		{
			double xScale = e.NewSize.Width / e.PreviousSize.Width;
			double yScale = e.NewSize.Height / e.PreviousSize.Height;
			body.Transformation *= Matrix2x3.FromScale(new Vector2D(xScale, yScale));
		}

		void CreateNewBody(FrameworkElement element)
		{
			double angle = 0;
			if (element.RenderTransform is RotateTransform)
				angle = ((RotateTransform)element.RenderTransform).Angle;
			PhysicsState state = new PhysicsState(new ALVector2D(angle, Canvas.GetLeft(element) + (element.ActualWidth / 2),
			                                                     Canvas.GetTop(element) + (element.ActualHeight / 2)));

			Shape shape = new PolygonShape(PolygonShape.CreateRectangle(element.ActualHeight, element.ActualWidth), 2);
			MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, 1);
			Body body = new CustomBody(state, shape, mass, new Coefficients(0, 1), new Lifespan());
			body.LinearDamping = 0.95;
			body.AngularDamping = 0.95;
			body.IsCollidable = GetIsCollidable(element);
			body.Tag = element;

			PhysicsController controller = FindParentPhysicsController(element);
			if (controller != null)
				controller.engine.AddBody(body);
			itemToBody.Add(element, body);
		}

		public Body GetBody(FrameworkElement item)
		{
			Body body;
			if (itemToBody.TryGetValue(item, out body))
				return body;
			return null;
		}
	}
}