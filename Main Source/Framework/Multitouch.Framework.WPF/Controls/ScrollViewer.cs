using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using AdvanceMath;
using Multitouch.Framework.WPF.Input;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;

namespace Multitouch.Framework.WPF.Controls
{
	public class ScrollViewer : System.Windows.Controls.ScrollViewer
	{
		const int BORDER = 1;
		Point startPoint;
		Point startOffset;
		PhysicsEngine engine;
		PhysicsTimer timer;
		Body body;
		FixedHingeJoint joint;
		int? firstContactId;

		public static readonly DependencyProperty LinearDumpingProperty = DependencyProperty.Register("LinearDumping", typeof(double),
			typeof(ScrollViewer), new UIPropertyMetadata(0.98d));

		public ScrollViewer()
		{
			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactRemoved);	

			engine = new PhysicsEngine();
			engine.BroadPhase = new SweepAndPruneDetector();
			engine.Solver = new SequentialImpulsesSolver();
			timer = new PhysicsTimer(PhysicsTimerCallback, 0.01);
		}

		public double LinearDumping
		{
			get { return (double)GetValue(LinearDumpingProperty); }
			set { SetValue(LinearDumpingProperty, value); }
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			if (!firstContactId.HasValue)
			{
				firstContactId = e.Contact.Id;

				startPoint = e.GetPosition(this);
				startOffset.X = HorizontalOffset;
				startOffset.Y = VerticalOffset;

				Vector2D contactPoint = startPoint.ToVector2D();
				joint = new FixedHingeJoint(body, contactPoint, new Lifespan());
				engine.AddJoint(joint);
			}
		}

		void OnContactMoved(object sender, ContactEventArgs e)
		{
			if (e.Contact.Id == firstContactId)
			{
				Point currentPoint = e.GetPosition(this);

				Vector2D currentPosition = -body.State.Position.Linear;
				if (-BORDER <= currentPosition.X && currentPosition.X <= ScrollableWidth + BORDER || -BORDER <= currentPosition.Y && currentPosition.Y <= ScrollableHeight + BORDER)
					joint.Anchor = currentPoint.ToVector2D();
			}
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			if (e.Contact.Id == firstContactId)
			{
				joint.Lifetime.IsExpired = true;
				joint = null;
				firstContactId = null;
			}
		}

		void PhysicsTimerCallback(double dt)
		{
			engine.Update(dt);

			if (body != null)
			{

				Vector2D position = -body.State.Position.Linear;
				Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
				                                                     {
				                                                     	ScrollToHorizontalOffset(position.X);
				                                                     	ScrollToVerticalOffset(position.Y);
				                                                     }));
				if (position.X < -BORDER)
				{
					body.State.Position.Linear.X = BORDER;
					body.State.Velocity.Linear.X = 0;
					body.ApplyImpulse(new Vector2D(-10, 0));
				}
				if (position.X > ScrollableWidth + BORDER)
				{
					body.State.Position.Linear.X = -(ScrollableWidth + BORDER);
					body.State.Velocity.Linear.X = 0;
					body.ApplyImpulse(new Vector2D(10, 0));
				}
				if (position.Y < -BORDER)
				{
					body.State.Position.Linear.Y = BORDER;
					body.State.Velocity.Linear.Y = 0;
					body.ApplyImpulse(new Vector2D(0, -10));
				}
				if (position.Y > ScrollableHeight + BORDER)
				{
					body.State.Position.Linear.Y = -(ScrollableHeight + BORDER);
					body.State.Velocity.Linear.Y = 0;
					body.ApplyImpulse(new Vector2D(0, 10));
				}
			}
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			timer.IsRunning = false;

			if (body != null)
				body.Lifetime.IsExpired = true;

			base.OnContentChanged(oldContent, newContent);

			PhysicsState state = new PhysicsState(new ALVector2D(0, 0, 0));
			Shape shape = new PolygonShape(PolygonShape.CreateRectangle(5, 5), BORDER);
			MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, BORDER);
			body = new Body(state, shape, mass, new Coefficients(0, BORDER), new Lifespan());
			body.LinearDamping = LinearDumping;
			body.Mass.MomentOfInertia = double.PositiveInfinity;
			body.Tag = newContent;
			engine.AddBody(body);

			if (!DesignerProperties.GetIsInDesignMode(this))
				timer.IsRunning = true;
		}
	}
}
