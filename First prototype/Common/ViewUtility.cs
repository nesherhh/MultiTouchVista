using System;
using System.ComponentModel;
using System.Windows;

namespace Danilins.Multitouch.Common
{
	public class ViewUtility : DependencyObject
	{
		static ViewUtility instance;

		private static ViewUtility Instance
		{
			get
			{
				if (instance == null)
					instance = new ViewUtility();
				return instance;
			}
		}

		public static bool IsDesignTime
		{
			get { return DesignerProperties.GetIsInDesignMode(Instance); }
		}
	}
}