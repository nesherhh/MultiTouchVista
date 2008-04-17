using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Danilins.Multitouch.Controls.Adorners
{
	public class ContactsAdorner : Adorner
	{
		public ContactsAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
			IsHitTestVisible = false;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			IMultitouchable element = AdornedElement as IMultitouchable;
			if (element != null)
			{
				SolidColorBrush renderBrush = new SolidColorBrush(Colors.Red);
				renderBrush.Opacity = 0.2;
				Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
				double renderRadius = 5.0;

				foreach (ContactInfoModel model in element.Contacts)
				{
					// Draw a circle at each corner.
					drawingContext.DrawEllipse(renderBrush, renderPen, model.Center, renderRadius, renderRadius);
					DrawText(drawingContext,
						string.Format("Id: {0}\r\nX: {1}\r\nY: {2}\r\nDelta: {3}\r\nDisplacement: {4}\r\nTarget: {5}", model.Id,
						model.Center.X.ToString("N3"),
						model.Center.Y.ToString("N3"),
						model.Delta,
						model.Displacement,
						model.Target != null ? model.Target.GetHashCode().ToString() : "None"),
						model.Center);
				}
			}
		}

		private void DrawText(DrawingContext drawingContext, string text, Point origin)
		{
			FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
															new Typeface("Tahoma"), 14, Brushes.Blue);
			drawingContext.DrawText(formattedText, origin);
		}

		public void Refresh()
		{
			InvalidateVisual();
		}
	}
}