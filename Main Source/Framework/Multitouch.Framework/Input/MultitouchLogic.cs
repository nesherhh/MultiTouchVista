using System;
using System.ComponentModel;
using System.Windows;

namespace Multitouch.Framework.Input
{
	class MultitouchLogic : DependencyObject
	{
		static MultitouchLogic current;

		ServiceCommunicator serviceCommunicator;

		MultitouchLogic()
		{
			if (!InDesignTime)
				serviceCommunicator = new ServiceCommunicator(this);
		}

		public static MultitouchLogic Current
		{
			get
			{
				if(current == null)
					current = new MultitouchLogic();
				return current;
			}
		}

		bool InDesignTime
		{
			get { return DesignerProperties.GetIsInDesignMode(this); }
		}
	}
}
