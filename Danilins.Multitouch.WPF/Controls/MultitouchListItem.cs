using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Controls
{
	public class MultitouchListItem : ContentControl
	{
		public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(MultitouchListItem),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(MultitouchListItem));
		public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(MultitouchListItem));

		private int localContacts;

		static MultitouchListItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultitouchListItem), new FrameworkPropertyMetadata(typeof(MultitouchListItem)));
		}

		public MultitouchListItem()
		{
			localContacts = 0;
		}

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			FrameworkElement parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
			if (parent != null)
				if (parent.ActualHeight > 0 || parent.ActualWidth > 0)
					SetRandomPosition(parent);
				else
					parent.SizeChanged += parent_SizeChanged;
			
			base.OnVisualParentChanged(oldParent);
		}

		private void parent_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			FrameworkElement parent = (FrameworkElement)sender;
			parent.SizeChanged -= parent_SizeChanged;

			SetRandomPosition(parent);
		}

		private void SetRandomPosition(FrameworkElement parent)
		{
			if (RandomizePosition)
			{
				double newLeft = Utils.NextRandom() * parent.ActualWidth;
				double newTop = Utils.NextRandom() * parent.ActualHeight;

				if (newLeft < ActualWidth)
					newLeft = ActualWidth;
				if (newLeft > parent.ActualWidth - ActualWidth)
					newLeft = parent.ActualWidth - ActualWidth;

				if (newTop < ActualHeight)
					newTop = ActualHeight;
				if (newTop > parent.ActualHeight - ActualHeight)
					newTop = parent.ActualHeight - ActualHeight;

				Canvas.SetLeft(this, newLeft);
				Canvas.SetTop(this, newTop);
			}
			else
			{
				if (double.IsNaN(Canvas.GetLeft(this)) || double.IsNaN(Canvas.GetTop(this)))
				{
					Canvas.SetLeft(this, parent.ActualWidth / 2 - ActualWidth / 2);
					Canvas.SetTop(this, parent.ActualHeight / 2 - ActualHeight / 2);
				}
			}
		}

		internal int Contacts
		{
			get { return localContacts; }
			set
			{
				localContacts = value;
				IsSelected = localContacts > 0;
			}
		}

		[Bindable(true), Category("Appearance")]
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public bool RandomizePosition { get; set; }
	}
}