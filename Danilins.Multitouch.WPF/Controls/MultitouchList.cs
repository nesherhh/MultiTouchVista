using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using AdvanceMath;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Controls.Adorners;
using Danilins.Multitouch.Framework;
using Danilins.Multitouch.PhysicsLibrary;
using Physics2DDotNet;
using Physics2DDotNet.Joints;

namespace Danilins.Multitouch.Controls
{
	public class MultitouchList : ItemsControl
	{
		int maxZ;

		private PhysicsController physicsController;
		private Dictionary<int, Joint> contactJoints;
		private ContactsAdorner contactsAdorner;

		static MultitouchList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultitouchList), new FrameworkPropertyMetadata(typeof(MultitouchList)));
		}

		public MultitouchList()
		{
			AddHandler(MultitouchScreen.ContactDownEvent, (ContactEventHandler)OnContactDown);
			AddHandler(MultitouchScreen.ContactUpEvent, (ContactEventHandler)OnContactUp);
			AddHandler(MultitouchScreen.ContactMoveEvent, (ContactEventHandler)OnContactMove);
			AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactLeave);

			maxZ = 1;

			RandomizePosition = true;

			contactJoints = new Dictionary<int, Joint>();
			Loaded += MultitouchListBox_Loaded;
		}

		public override void OnApplyTemplate()
		{
			physicsController = GetTemplateChild("Physics") as PhysicsController;
			base.OnApplyTemplate();
		}

		public bool RandomizePosition
		{
			get;set;
		}

		void MultitouchListBox_Loaded(object sender, RoutedEventArgs e)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
			contactsAdorner = new ContactsAdorner(this);
			adornerLayer.Add(contactsAdorner);
		}

		private void UpdateAdorner()
		{
			//if (contactsAdorner != null)
			//    contactsAdorner.Refresh();
		}

		private void OnContactLeave(object sender, ContactEventArgs e)
		{
			OnContactUp(sender, e);
		}

		private void OnContactUp(object sender, ContactEventArgs e)
		{
			int id = e.Contact.Id;
			Point localPoint = e.GetPosition(this);
			HitTestResult test = VisualTreeHelper.HitTest(this, localPoint);
			if (test != null)
			{
				MultitouchListItem item = ContainerFromElement(test.VisualHit) as MultitouchListItem;
				if (item != null)
				{
					MakeUnselected(item);
					e.Handled = true;
				}
			}
			Joint joint;
			if (contactJoints.TryGetValue(id, out joint))
			{
				CustomFixedHingeJoint fixedHinge = joint as CustomFixedHingeJoint;
				if (fixedHinge != null)
				{
					fixedHinge.Lifetime.IsExpired = true;
					contactJoints.Remove(id);
				}
			}
			UpdateAdorner();
		}

		private void OnContactMove(object sender, ContactEventArgs e)
		{
			ContactInfo contact = e.Contact;
			Point localPoint = e.GetPosition(this);
			HitTestResult hitTest = VisualTreeHelper.HitTest(this, localPoint);
			MultitouchListItem item = ContainerFromElement(hitTest.VisualHit) as MultitouchListItem;
			if (item != null)
			{
				Joint joint;
				if (contactJoints.TryGetValue(contact.Id, out joint))
				{
					CustomFixedHingeJoint fixedHinge = joint as CustomFixedHingeJoint;
					if (fixedHinge != null)
					{
						Vector2D point = fixedHinge.Anchor;
						point.X = localPoint.X;
						point.Y = localPoint.Y;
						fixedHinge.Anchor = point;
					}
				}
				e.Handled = true;
			}
			UpdateAdorner();
		}

		private void OnContactDown(object sender, ContactEventArgs e)
		{
			ContactInfo contact = e.Contact;
			ContactInfoModel model = new ContactInfoModel(contact);
			Point localPoint = e.GetPosition(this);
			HitTestResult test = VisualTreeHelper.HitTest(this, localPoint);
			MultitouchListItem item = ContainerFromElement(test.VisualHit) as MultitouchListItem;
			if (item != null)
			{
				MakeSelected(item);
				e.Handled = true;

				if (physicsController != null)
				{
					Body body = physicsController.GetBody(item);
					if (body != null)
					{
						Vector2D point = new Vector2D(localPoint.X, localPoint.Y);
						if (!body.Shape.BroadPhaseDetectionOnly && body.Shape.CanGetIntersection)
						{
							Vector2D temp = body.Matrices.ToBody * point;
							IntersectionInfo info;
							if (body.Shape.TryGetIntersection(temp, out info))
							{
								CustomFixedHingeJoint joint = new CustomFixedHingeJoint(body, point, new Lifespan());
								physicsController.Engine.AddJoint(joint);
								contactJoints[model.Id] = joint;
							}
						}
					}
				}
			}
			UpdateAdorner();
		}

		private void MakeSelected(MultitouchListItem item)
		{
			maxZ++;
			if (maxZ >= int.MaxValue)
			{
				foreach (DependencyObject o in Items)
				{
					UIElement element = (UIElement)ContainerFromElement(o);
					Canvas.SetZIndex(element, 0);
				}
				maxZ = 1;
			}

			Canvas.SetZIndex(item, maxZ);
			item.Contacts++;
		}

		private void MakeUnselected(MultitouchListItem item)
		{
			item.Contacts--;
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new MultitouchListItem { RandomizePosition = RandomizePosition };
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is MultitouchListItem;
		}
	}
}