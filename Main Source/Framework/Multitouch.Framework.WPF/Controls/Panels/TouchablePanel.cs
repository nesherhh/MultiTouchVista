using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
		class ScaleState
		{
			public double Scale { get; set; }
			public Point Center { get; set; }

			public ScaleState()
			{
				Scale = 1;
			}
		}

		PhysicsEngine engine;
		PhysicsTimer timer;
		Dictionary<FrameworkElement, Body> elementToBody;
		Dictionary<FrameworkElement, ScaleState> elementToScale;
		List<FrameworkElement> shouldCreateBody;
		List<FrameworkElement> shouldRemoveBody;
		Dictionary<int, FixedHingeJoint> contactJoints;

		public TouchablePanel()
		{
			elementToBody = new Dictionary<FrameworkElement, Body>();
			shouldCreateBody = new List<FrameworkElement>();
			shouldRemoveBody = new List<FrameworkElement>();
			contactJoints = new Dictionary<int, FixedHingeJoint>();
			elementToScale = new Dictionary<FrameworkElement, ScaleState>();

			engine = new PhysicsEngine();
			engine.BroadPhase = new SweepAndPruneDetector();
			engine.Solver = new SequentialImpulsesSolver();
			timer = new PhysicsTimer(PhysicsTimerCallback, 0.01);

			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactRemoved);

			Loaded += TouchablePanel_Loaded;
		}

		void TouchablePanel_Loaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
				timer.IsRunning = true;
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
				if (element != null)
				{
					FixedHingeJoint joint;
					if (contactJoints.TryGetValue(e.Contact.Id, out joint))
					{
						joint.Anchor = position.ToVector2D();


						// scale
						Body body = joint.Bodies.First();
						FrameworkElement frameworkElement = body.Tag as FrameworkElement;
						if (frameworkElement != null)
						{

							ScaleState state;
							if (elementToScale.TryGetValue(frameworkElement, out state))
							{
								IDictionary<int, Contact> contacts = e.GetContacts(element);
								double previousDistance = 0;
								double currentDistance = 0;
								int divisor = 0;
								Point center = new Point(frameworkElement.ActualWidth / 2, frameworkElement.ActualHeight / 2);
								Contact[] contactsArray = contacts.Values.ToArray();
								for (int i = 0; i < contactsArray.Length; i++)
								{
									for (int j = i + 1; j < contactsArray.Length; j++)
									{
										Vector vector = frameworkElement.PointFromScreen(contactsArray[j].Position) - frameworkElement.PointFromScreen(contactsArray[i].Position);
										currentDistance += vector.Length;
										center += vector;

										Vector previousVector = frameworkElement.PointFromScreen(contactsArray[j].PreviousPosition) - frameworkElement.PointFromScreen(contactsArray[i].PreviousPosition);
										previousDistance += previousVector.Length;
										divisor++;
									}
								}
								if (divisor == 0)
									divisor = 1;

								previousDistance /= divisor;
								currentDistance /= divisor;
								center.X /= divisor;
								center.Y /= divisor;

								double delta = currentDistance / previousDistance;
								if (double.IsNaN(delta))
									delta = 1;
								state.Scale *= delta;
								state.Center = center;
								body.Transformation *= Matrix2x3.FromScale(new Vector2D(delta, delta));
							}
						}
					}
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
						if(body.Shape.CanGetIntersection)
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

		void PhysicsTimerCallback(double dt, double trueDt)
		{
			engine.Update(dt, trueDt);
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
				TransformGroup transform = child.RenderTransform as TransformGroup;
				if (transform != null)
				{
					RotateTransform rotateTransform = (RotateTransform)transform.Children[1];
					double angleInDegrees = MathHelper.ToDegrees(body.State.Position.Angular);
					if (rotateTransform.Angle != angleInDegrees)
						rotateTransform.Angle = angleInDegrees;
					if (rotateTransform.CenterX != offsetX)
						rotateTransform.CenterX = offsetX;
					if (rotateTransform.CenterY != offsetY)
						rotateTransform.CenterY = offsetY;

					ScaleState state;
					if (elementToScale.TryGetValue(child, out state))
					{
						ScaleTransform scaleTransform = (ScaleTransform)transform.Children[0];
						scaleTransform.ScaleX = scaleTransform.ScaleY = state.Scale;
						scaleTransform.CenterX = state.Center.X;
						scaleTransform.CenterY = state.Center.Y;
					}
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
			IShape shape = new PolygonShape(VertexHelper.CreateRectangle(frameworkElement.ActualHeight, frameworkElement.ActualWidth), 2);
			MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, 1);
			Body body = new Body(state, shape, mass, new Coefficients(0, 1), new Lifespan());
			body.LinearDamping = 0.95;
			body.AngularDamping = 0.95;
			body.IsCollidable = false;
			body.Tag = frameworkElement;
			engine.AddBody(body);
			elementToBody.Add(frameworkElement, body);
			elementToScale.Add(frameworkElement, new ScaleState());

			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform());
			transform.Children.Add(new RotateTransform());
			frameworkElement.RenderTransform = transform;
		}

		void RemoveBody(FrameworkElement frameworkElement)
		{
			Body body;
			if(elementToBody.TryGetValue(frameworkElement, out body))
			{
				body.Lifetime.IsExpired = true;
				elementToBody.Remove(frameworkElement);
				elementToScale.Remove(frameworkElement);
			}
		}
	}
}