using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Automation;
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
		public Point ScaleCenter { get; set; }
		internal WindowContacts Contacts { get; private set; }

		bool hasMenu;
		AutomationElement automationElement;

		public Window(IntPtr hWnd)
			: base(hWnd)
		{
			Angle = 0;
			Scale = 1;
			hasMenu = false;

			Contacts = new WindowContacts(this);

			automationElement = AutomationElement.FromHandle(hWnd);
			PropertyCondition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuBar);
			AutomationElement first = automationElement.FindFirst(TreeScope.Descendants, condition);
			if (first != null && !first.Current.Name.Equals("System Menu Bar"))
				hasMenu = true;
		}

		public Matrix Matrix
		{
			get
			{
				Rectangle rectangle = Rectangle;
				Point rotationCenter;
				if (hasMenu)
					rotationCenter = new Point((rectangle.Width + SystemInformation.FrameBorderSize.Width) / 2.0, (rectangle.Height + SystemInformation.CaptionHeight + SystemInformation.MenuHeight + SystemInformation.FrameBorderSize.Height) / 2.0);
				else
					rotationCenter = new Point((rectangle.Width + SystemInformation.FrameBorderSize.Width) / 2.0, (rectangle.Height + SystemInformation.CaptionHeight + SystemInformation.FrameBorderSize.Height) / 2.0);

				Matrix result = new Matrix();
				result.Translate(-rotationCenter.X, -rotationCenter.Y);
				result.Rotate(Angle);
				result.Translate(rotationCenter.X, rotationCenter.Y);
				result.Translate(-ScaleCenter.X, -ScaleCenter.Y);
				result.Scale(Scale, Scale);
				result.Translate(ScaleCenter.X, ScaleCenter.Y);
				return result;
			}
		}

		public Matrix GetMatrix(Window window)
		{
			Rectangle rectangle = window.Rectangle;
			Point rotationCenter;
			if (window.hasMenu)
				rotationCenter = new Point((rectangle.Width - SystemInformation.FrameBorderSize.Width) / 2.0, (rectangle.Height - SystemInformation.CaptionHeight - SystemInformation.MenuHeight - SystemInformation.FrameBorderSize.Height) / 2.0);
			else
				rotationCenter = new Point((rectangle.Width - SystemInformation.FrameBorderSize.Width) / 2.0, (rectangle.Height - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height) / 2.0);

			Matrix result = new Matrix();
			result.Translate(-rotationCenter.X, -rotationCenter.Y);
			result.Rotate(Angle);
			result.Translate(rotationCenter.X, rotationCenter.Y);
			result.Translate(-ScaleCenter.X, -ScaleCenter.Y);
			result.Scale(Scale, Scale);
			result.Translate(ScaleCenter.X, ScaleCenter.Y);
			return result;
		}
	}
}