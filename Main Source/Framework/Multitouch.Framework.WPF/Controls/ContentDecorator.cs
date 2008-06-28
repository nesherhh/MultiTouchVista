using System;
using System.Windows;
using System.Windows.Controls;

namespace Multitouch.Framework.WPF.Controls
{
	internal class ContentDecorator : Decorator
	{
		readonly ScrollViewer owner;
		internal const double BORDER = 50;

		public ContentDecorator(ScrollViewer owner)
		{
			this.owner = owner;

			HorizontalAlignment = HorizontalAlignment.Center;
			VerticalAlignment = VerticalAlignment.Center;
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			UIElement child = Child;
			if(child != null)
			{
				Rect rect = new Rect(arrangeSize);
				if(OwnerHasValidMeasures())
				{
					if (owner.ActualHeight < arrangeSize.Height)
					{
						rect.Height -= BORDER * 2;
						rect.Y += BORDER;
					}
					if (owner.ActualWidth < arrangeSize.Width)
					{
						rect.Width -= BORDER * 2;
						rect.X += BORDER;
					}
				}
				child.Arrange(rect);
			}
			return arrangeSize;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			UIElement child = Child;
			if (child != null)
			{
				child.Measure(constraint);

				Size result = child.DesiredSize;
				if(OwnerHasValidMeasures())
				{
					if (owner.ActualHeight < result.Height)
						result.Height += BORDER * 2;
					if (owner.ActualWidth < result.Width)
						result.Width += BORDER * 2;
				}
				return result;
			}
			return new Size();
		}

		bool OwnerHasValidMeasures()
		{
			return !double.IsNaN(owner.ActualHeight) && !double.IsNaN(owner.ActualWidth);
		}
	}
}