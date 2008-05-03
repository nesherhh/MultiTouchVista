using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AdvanceMath;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls
{
	public class TouchablePanel : RandomCanvas
	{
		PhysicsEngine engine;
		PhysicsTimer timer;
		Dictionary<FrameworkElement, Body> elementToBody;
		List<FrameworkElement> shouldCreateBody;
		List<FrameworkElement> shouldRemoveBody;
		Dictionary<int, FixedHingeJoint> contactJoints;

		public TouchablePanel()
		{
			elementToBody = new Dictionary<FrameworkElement, Body>();
			shouldCreateBody = new List<FrameworkElement>();
			shouldRemoveBody = new List<FrameworkElement>();
			contactJoints = new Dictionary<int, FixedHingeJoint>();

			engine = new PhysicsEngine();
			engine.BroadPhase = new SweepAndPruneDetector();
			engine.Solver = new SequentialImpulsesSolver();
			timer = new PhysicsTimer(PhysicsTimerCallback, 0.01);
			if (!DesignerProperties.GetIsInDesignMode(this))
				timer.IsRunning = true;

			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactRemoved);
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			FixedHingeJoint joint;
			if(contactJoints.TryGetValue(e.Contact.Id, out joint))
			{
				joint.Lifetime.IsExpired = true;
				contactJoints.Remove(e.Contact.Id);
			}
		}

		void OnContactMoved(object sender, ContactEventArgs e)
		{
			Point position = e.GetPosition(this);
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, position);
			if(hitTestResult != null)
			{
				FrameworkElement element = hitTestResult.VisualHit as FrameworkElement;
				if(element != null)
				{
					FixedHingeJoint joint;
					if (contactJoints.TryGetValue(e.Contact.Id, out joint))
						joint.Anchor = position.ToVector2D();
				}
			}
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			Point position = e.GetPosition(this);
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, position);
			if(hitTestResult != null)
			{
				FrameworkElement element = hitTestResult.VisualHit as FrameworkElement;
				if(element != null)
				{
					FrameworkElement container = ItemsControl.ContainerFromElement(null, element) as FrameworkElement;
					if(container != null)
						element = container;

					Body body;
					if(elementToBody.TryGetValue(element, out body))
					{
						Vector2D contactPoint = position.ToVector2D();
						if(!body.Shape.BroadPhaseDetectionOnly && body.Shape.CanGetIntersection)
						{
							Vector2D temp = body.Matrices.ToBody * contactPoint;
							IntersectionInfo intersectionInfo;
							if(body.Shape.TryGetIntersection(temp, out intersectionInfo))
							{
								FixedHingeJoint joint = new FixedHingeJoint(body, contactPoint, new Lifespan());
								engine.AddJoint(joint);
								contactJoints[e.Contact.Id] = joint;
							}
						}
					}

					SetZTop(element);
				}
			}
		}

		void SetZTop(UIElement element)
		{
			int zIndex = 0;
			foreach (FrameworkElement child in InternalChildren)
				zIndex = Math.Max(zIndex, GetZIndex(child));
			if(zIndex == int.MaxValue)
			{
				foreach (FrameworkElement child in InternalChildren)
					SetZIndex(child, 0);
				zIndex = 0;
			}
			zIndex++;
			SetZIndex(element, zIndex);
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			FrameworkElement added = visualAdded as FrameworkElement;
			if (added != null)
				shouldCreateBody.Add(added);
			FrameworkElement removed = visualRemoved as FrameworkElement;
			if (removed != null)
				shouldRemoveBody.Add(removed);
			
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}

		void PhysicsTimerCallback(double dt)
		{
			engine.Update(dt);
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)UpdateChildren);
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			Size arrangeOverride = base.ArrangeOverride(arrangeSize);

			foreach (FrameworkElement element in shouldCreateBody)
				CreateBody(element);
			shouldCreateBody.Clear();
			
			foreach (FrameworkElement element in shouldRemoveBody)
				RemoveBody(element);
			shouldRemoveBody.Clear();
			
			return arrangeOverride;
		}

		protected virtual void UpdateChildren()
		{
			foreach (Body body in engine.Bodies)
			{
				FrameworkElement child = body.Tag as FrameworkElement;
				if (child == null)
					continue;

				Vector2D linearPosition = body.State.Position.Linear;
				double left = linearPosition.X - (child.ActualWidth / 2);
				double top = linearPosition.Y - (child.ActualHeight / 2);
				if (GetLeft(child) != left)
					SetLeft(child, left);
				if (GetTop(child) != top)
					SetTop(child, top);

				double offsetX = child.ActualWidth / 2;
				double offsetY = child.ActualHeight / 2;
				RotateTransform transform = child.RenderTransform as RotateTransform;
				if(transform != null)
				{
					double angleInDegrees = MathHelper.ToDegrees(body.State.Position.Angular);
					if(transform.Angle != angleInDegrees)
						transform.Angle = angleInDegrees;
					if(transform.CenterX != offsetX)
						transform.CenterX = offsetX;
					if (transform.CenterY != offsetY)
						transform.CenterY = offsetY;
				}
			}
		}

		void CreateBody(FrameworkElement frameworkElement)
		{
			double angle = 0;
			RotateTransform rotateTransform = frameworkElement.RenderTransform as RotateTransform;
			if(rotateTransform != null)
				angle = rotateTransform.Angle;
			PhysicsState state = new PhysicsState(new ALVector2D(angle, GetLeft(frameworkElement) + (frameworkElement.ActualWidth / 2),
			                                                     GetTop(frameworkElement) + (frameworkElement.ActualHeight / 2)));
			Shape shape = new PolygonShape(PolygonShape.CreateRectangle(frameworkElement.ActualHeight, frameworkElement.ActualWidth), 2);
			MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, 1);
			Body body = new Body(state, shape, mass, new Coefficients(0, 1), new Lifespan());
			body.LinearDamping = 0.95;
			body.AngularDamping = 0.95;
			body.IsCollidable = false;
			body.Tag = frameworkElement;
			engine.AddBody(body);
			elementToBody.Add(frameworkElement, body);

			if (frameworkElement.RenderTransform == null || !(frameworkElement.RenderTransform is RotateTransform))
				frameworkElement.RenderTransform = new RotateTransform();
		}

		void RemoveBody(FrameworkElement frameworkElement)
		{
			Body body;
			if(elementToBody.TryGetValue(frameworkElement, out body))
			{
				body.Lifetime.IsExpired = true;
				elementToBody.Remove(frameworkElement);
			}
		}
	}
}