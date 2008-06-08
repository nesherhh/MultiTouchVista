using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using ManagedWinapi.Windows;
using Point=System.Windows.Point;

namespace DWMaxxAddIn
{
	[DebuggerDisplay("Title: {Title}")]
	public class Window : SystemWindow
	{
		public double Angle { get; set; }
		public double Scale { get; set; }
		public int ContactsCount { get; set; }
		
		public Window(IntPtr HWnd)
			: base(HWnd)
		{
			Angle = 0;
			Scale = 1;
		}

		public Matrix Matrix
		{
			get
			{
				Rectangle rectangle = Rectangle;
				Point center = new Point((rectangle.Width + SystemInformation.FrameBorderSize.Width) / 2.0, (rectangle.Height + (SystemInformation.CaptionHeight * 2) + SystemInformation.FrameBorderSize.Height) / 2.0);

				Matrix result = new Matrix();
				result.Translate(-center.X, -center.Y);
				result.Rotate(Angle);
				result.Translate(center.X, center.Y);
				result.Scale(Scale, Scale);
				return result;
			}
		}

		public Matrix GetMatrix(SystemWindow window)
		{
			Rectangle rectangle = window.Rectangle;
			Point center = new Point((rectangle.Width - SystemInformation.FrameBorderSize.Width) / 2.0, (rectangle.Height - (SystemInformation.CaptionHeight * 2) - SystemInformation.FrameBorderSize.Height) / 2.0);

			Matrix result = new Matrix();
			result.Translate(-center.X, -center.Y);
			result.Rotate(Angle);
			result.Translate(center.X, center.Y);
			result.Scale(Scale, Scale);
			return result;
		}
	}
}