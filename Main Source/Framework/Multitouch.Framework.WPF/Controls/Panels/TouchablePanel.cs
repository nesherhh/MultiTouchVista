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
using Physics2DDotNet.Ignorers;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls
{
    /// <summary>
    /// A Panel that can Move, Rotate and Scale it's child items and respons to Multitouch events.
    /// </summary>
    public class TouchablePanel : RandomCanvas
    {
        const int ROTATE_TRANSFORM_INDEX_IN_GROUP = 1;
        const int SCALE_TRANSFORM_INDEX_IN_GROUP = 0;

        class ScaleState
        {
            public double Scale { get; set; }
            public Point Center { get; set; }

            public ScaleState()
            {
                Scale = 1;
            }
        }

    	readonly PhysicsEngine engine;
    	readonly PhysicsTimer timer;
    	readonly ObjectIgnorer ignorer;
    	readonly Dictionary<FrameworkElement, Body> elementToBody;
    	readonly Dictionary<FrameworkElement, ScaleState> elementToScale;
    	readonly List<FrameworkElement> shouldCreateBody;
    	readonly List<FrameworkElement> shouldRemoveBody;
    	readonly Dictionary<int, FixedHingeJoint> contactJoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchablePanel"/> class.
        /// </summary>
        public TouchablePanel()
        {
			elementToBody = new Dictionary<FrameworkElement, Body>();
            shouldCreateBody = new List<FrameworkElement>();
            shouldRemoveBody = new List<FrameworkElement>();
            contactJoints = new Dictionary<int, FixedHingeJoint>();
            elementToScale = new Dictionary<FrameworkElement, ScaleState>();
            ignorer = new ObjectIgnorer();

            engine = new PhysicsEngine();
            engine.BroadPhase = new SweepAndPruneDetector();
            engine.Solver = new SequentialImpulsesSolver();
            timer = new PhysicsTimer(PhysicsTimerCallback, 0.01);

            Loaded += TouchablePanel_Loaded;
        }

        private void CreateWalls()
        {
            var wallCoff = new Coefficients(0.8f, 0.95f);
            var wallLife = new Lifespan();

            var flrState = new PhysicsState(new ALVector2D((float)0.0, ((float)ActualWidth) * ((float)0.5), (float)ActualHeight + 100.0));
            var flrShape = new PolygonShape(VertexHelper.CreateRectangle(ActualWidth, 200), 2);
            var bdyFloor = new Body(flrState, flrShape, float.PositiveInfinity, wallCoff, wallLife);

            var ceiState = new PhysicsState(new ALVector2D((float)0.0, ((float)ActualWidth) * ((float)0.5), -100.0));
            var ceiShape = new PolygonShape(VertexHelper.CreateRectangle(ActualWidth, 200), 2);
            var bdyCeiling = new Body(ceiState, ceiShape, float.PositiveInfinity, wallCoff, wallLife);

            var lwlState = new PhysicsState(new ALVector2D((float)0.0, -100.0, ((float)ActualHeight) * ((float)0.5)));
            var lwlShape = new PolygonShape(VertexHelper.CreateRectangle(200, ActualHeight), 2);
            var bdyLeftWall = new Body(lwlState, lwlShape, float.PositiveInfinity, wallCoff, wallLife);

            var rwlState = new PhysicsState(new ALVector2D((float)0.0, (float)ActualWidth + 100.0, ((float)ActualHeight) * ((float)0.5)));
            var rwlShape = new PolygonShape(VertexHelper.CreateRectangle(200, ActualHeight), 2);
            var bdyRightWall = new Body(rwlState, rwlShape, float.PositiveInfinity, wallCoff, wallLife);

            engine.AddBody(bdyFloor);
            engine.AddBody(bdyCeiling);
            engine.AddBody(bdyLeftWall);
            engine.AddBody(bdyRightWall);
        }

        void TouchablePanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                timer.IsRunning = true;

            if (EnableWalls)
                CreateWalls();
        }

		void OnContactMoved(object sender, ContactEventArgs e)
		{
			Point position = e.GetPosition(this);

			FixedHingeJoint joint;
			if (contactJoints.TryGetValue(e.Contact.Id, out joint))
			{
				joint.Anchor = position.ToVector2D();

				//scale
				Body body = joint.Bodies.First();
				FrameworkElement frameworkElement = body.Tag as FrameworkElement;
				if (frameworkElement != null && GetIsScalable(frameworkElement))
				{
					ScaleState state;
					if (elementToScale.TryGetValue(frameworkElement, out state))
					{
						IEnumerable<Contact> contacts = MultitouchScreen.GetContactsCaptured((IInputElement)e.Source);
						double previousDistance = 0;
						double currentDistance = 0;
						int divisor = 0;
						Contact[] contactsArray = contacts.ToArray();

						Point center = new Point(frameworkElement.ActualWidth / 2, frameworkElement.ActualHeight / 2);

						for (int i = 0; i < contactsArray.Length; i++)
						{
							for (int j = i + 1; j < contactsArray.Length; j++)
							{
								Point currFirst = contactsArray[j].GetPosition(this);
								Point currSecond = contactsArray[i].GetPosition(this);
								Vector vector = frameworkElement.PointFromScreen(currFirst) - frameworkElement.PointFromScreen(currSecond);
								currentDistance += vector.Length;

								Point prevFirst = contactsArray[j].GetPoints(this).FirstOrDefault();
								if (default(Point) == prevFirst)
									prevFirst = currFirst;
								Point prevSecond = contactsArray[i].GetPoints(this).FirstOrDefault();
								if (default(Point) == prevSecond)
									prevSecond = currSecond;
								Vector previousVector = frameworkElement.PointFromScreen(prevFirst) - frameworkElement.PointFromScreen(prevSecond);
								previousDistance += previousVector.Length;
								divisor++;
							}
						}
						if (divisor == 0)
							divisor = 1;

						previousDistance /= divisor;
						currentDistance /= divisor;

						double delta = currentDistance / previousDistance;
						if (double.IsNaN(delta))
							delta = 1;

						var newScale = state.Scale * delta;
						if (newScale > MaxScale)
							delta = MaxScale / state.Scale;
						else if (newScale < MinScale)
							delta = MinScale / state.Scale;

						state.Scale *= delta;
						state.Center = center;
						body.Transformation *= Matrix2x3.FromScale(new Vector2D(delta, delta));
					}
				}
			}
		}

    	static void OnNewContact(object sender, NewContactEventArgs e)
        {
			if(e.Contact.Captured != e.Source)
        		e.Contact.Capture((IInputElement)e.Source);
        }

		void OnGotContactCapture(object sender, ContactEventArgs e)
		{
			FrameworkElement element = (FrameworkElement)e.Source;
			FrameworkElement container = ItemsControl.ContainerFromElement(null, element) as FrameworkElement;
			if (container != null)
				element = container;

			Body body;
			if (elementToBody.TryGetValue(element, out body))
			{
				Point position = e.GetPosition(this);
				Vector2D contactPoint = position.ToVector2D();
				if (body.Shape.CanGetIntersection)
				{
					Vector2D temp = body.Matrices.ToBody * contactPoint;
					IntersectionInfo intersectionInfo;
					if (body.Shape.TryGetIntersection(temp, out intersectionInfo))
					{
						FixedHingeJoint joint = new FixedHingeJoint(body, contactPoint, new Lifespan());
						joint.Softness = 0.3;
						engine.AddJoint(joint);
						contactJoints[e.Contact.Id] = joint;
					}
				}
			}
			SetZTop(element);
		}

    	static void OnContactRemoved(object sender, ContactEventArgs e)
        {
        	if (e.Contact.Captured == e.Source)
        		e.Contact.Capture(null);
        }

		void OnLostContactCapture(object sender, ContactEventArgs e)
		{
			FixedHingeJoint removedJoint;
            if (contactJoints.TryGetValue(e.Contact.Id, out removedJoint))
            {
                removedJoint.Lifetime.IsExpired = true;
                contactJoints.Remove(e.Contact.Id);
            }
        }

        void SetZTop(UIElement element)
        {
            int zIndex = 0;
            foreach (FrameworkElement child in InternalChildren)
                zIndex = Math.Max(zIndex, GetZIndex(child));
            if (zIndex == int.MaxValue)
            {
                foreach (FrameworkElement child in InternalChildren)
                    SetZIndex(child, 0);
                zIndex = 0;
            }
            zIndex++;
            SetZIndex(element, zIndex);
        }

        /// <summary>
        /// Invoked when the <see cref="T:System.Windows.Media.VisualCollection"/> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="T:System.Windows.Media.Visual"/> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="T:System.Windows.Media.Visual"/> that was removed from the collection.</param>
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

        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.Canvas"/> element.
        /// </summary>
        /// <param name="arrangeSize">The size that this <see cref="T:System.Windows.Controls.Canvas"/> element should use to arrange its child elements.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.Canvas"/> element and its descendants.
        /// </returns>
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

        /// <summary> 
        /// Updates the child positions.
        /// </summary>
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
                    RotateTransform rotateTransform = (RotateTransform)transform.Children[ROTATE_TRANSFORM_INDEX_IN_GROUP];
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
                        ScaleTransform scaleTransform = (ScaleTransform)transform.Children[SCALE_TRANSFORM_INDEX_IN_GROUP];
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

            //if an element already has rotatetransform, get it's angle.
            RotateTransform rotateTransform = frameworkElement.RenderTransform as RotateTransform;
            if (rotateTransform != null)
                angle = MathHelper.ToRadians(rotateTransform.Angle);

            PhysicsState state = new PhysicsState(new ALVector2D(angle, GetLeft(frameworkElement) + (frameworkElement.ActualWidth / 2),
                                                                 GetTop(frameworkElement) + (frameworkElement.ActualHeight / 2)));
            IShape shape = new PolygonShape(VertexHelper.CreateRectangle(frameworkElement.ActualWidth, frameworkElement.ActualHeight), 2);
            MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, 1);
            Body body = new Body(state, shape, mass, new Coefficients(0.4, 0.95), new Lifespan());
            body.LinearDamping = LinearDamping;
            body.AngularDamping = AngularDamping;
            if (EnableWalls)
            {
                body.IsCollidable = true;
                body.CollisionIgnorer = ignorer;
            }
            else
                body.IsCollidable = false;
            body.Tag = frameworkElement;
            engine.AddBody(body);
            elementToBody.Add(frameworkElement, body);
            elementToScale.Add(frameworkElement, new ScaleState());

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform());
            transform.Children.Add(new RotateTransform());
            frameworkElement.RenderTransform = transform;
            SetZTop(frameworkElement);
		
			SubscribeEventsToChild(frameworkElement);
        }

    	void RemoveBody(FrameworkElement frameworkElement)
    	{
    		Body body;
    		if (elementToBody.TryGetValue(frameworkElement, out body))
    		{
    			UnsubscribeEventsFromChild(frameworkElement);

    			body.Lifetime.IsExpired = true;
    			elementToBody.Remove(frameworkElement);
    			elementToScale.Remove(frameworkElement);
    		}
    	}

    	void UnsubscribeEventsFromChild(DependencyObject frameworkElement)
    	{
    		MultitouchScreen.RemoveGotContactCaptureHandler(frameworkElement, OnGotContactCapture);
    		MultitouchScreen.RemoveLostContactCaptureHandler(frameworkElement, OnLostContactCapture);
			MultitouchScreen.RemoveNewContactHandler(frameworkElement, OnNewContact);
			MultitouchScreen.RemoveContactMovedHandler(frameworkElement, OnContactMoved);
			MultitouchScreen.RemoveContactRemovedHandler(frameworkElement, OnContactRemoved);
    	}

    	void SubscribeEventsToChild(DependencyObject frameworkElement)
    	{
    		MultitouchScreen.AddGotContactCaptureHandler(frameworkElement, OnGotContactCapture);
    		MultitouchScreen.AddLostContactCaptureHandler(frameworkElement, OnLostContactCapture);
    		MultitouchScreen.AddNewContactHandler(frameworkElement, OnNewContact);
    		MultitouchScreen.AddContactMovedHandler(frameworkElement, OnContactMoved);
    		MultitouchScreen.AddContactRemovedHandler(frameworkElement, OnContactRemoved);
    	}

    	#region MinScale

        /// <summary>
        /// MinScale Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(TouchablePanel),
                new FrameworkPropertyMetadata(0.5));

        /// <summary>
        /// Gets or sets the MinScale property.  This dependency property 
        /// indicates the minimum scaling factor for this panel's children.
        /// </summary>
        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        #endregion

        #region MaxScale

        /// <summary>
        /// MaxScale Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(TouchablePanel),
                new FrameworkPropertyMetadata(3.0));

        /// <summary>
        /// Gets or sets the MaxScale property.  This dependency property 
        /// indicates the maximum scaling factor for this panel's children.
        /// </summary>
        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        #endregion

        #region Attached - IsScalable

        /// <summary>
        /// IsScalable Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsScalableProperty =
            DependencyProperty.RegisterAttached("IsScalable", typeof(bool), typeof(TouchablePanel),
                new UIPropertyMetadata(true));

        /// <summary>
        /// Gets the IsScalable property.  This dependency property 
        /// indicates if the element is scalable.
        /// </summary>
        public static bool GetIsScalable(DependencyObject d)
        {
            return (bool)d.GetValue(IsScalableProperty);
        }

        /// <summary>
        /// Sets the IsScalable property.  This dependency property 
        /// indicates if the element is scalable.
        /// </summary>
        public static void SetIsScalable(DependencyObject d, bool value)
        {
            d.SetValue(IsScalableProperty, value);
        }

        #endregion

        #region LinearDamping

        /// <summary>
        /// LinearDamping Dependency Property
        /// </summary>
        public static readonly DependencyProperty LinearDampingProperty =
            DependencyProperty.Register("LinearDamping", typeof(double), typeof(TouchablePanel),
                new UIPropertyMetadata(0.95d));

        /// <summary>
        /// Gets or sets the LinearDamping property.  This dependency property 
        /// indicates the linear damping of the children of this panel.
        /// </summary>
        public double LinearDamping
        {
            get { return (double)GetValue(LinearDampingProperty); }
            set { SetValue(LinearDampingProperty, value); }
        }

        #endregion

        #region AngularDamping

        /// <summary>
        /// AngularDamping Dependency Property
        /// </summary>
        public static readonly DependencyProperty AngularDampingProperty =
            DependencyProperty.Register("AngularDamping", typeof(double), typeof(TouchablePanel),
                new UIPropertyMetadata(0.95d));

        /// <summary>
        /// Gets or sets the AngularDamping property.  This dependency property 
        /// indicates ....
        /// </summary>
        public double AngularDamping
        {
            get { return (double)GetValue(AngularDampingProperty); }
            set { SetValue(AngularDampingProperty, value); }
        }

        #endregion

        #region EnableWalls

        /// <summary>
        /// EnableWalls Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableWallsProperty =
            DependencyProperty.Register("EnableWalls", typeof(bool), typeof(TouchablePanel),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the EnableWalls property.  This dependency property 
        /// indicates if the elements bounce in the walls of the Panel.
        /// </summary>
        public bool EnableWalls
        {
            get { return (bool)GetValue(EnableWallsProperty); }
            set { SetValue(EnableWallsProperty, value); }
        }

        #endregion


    }
}