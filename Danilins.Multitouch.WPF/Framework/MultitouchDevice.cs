using System;
using System.Windows;
using System.Windows.Input;

namespace Danilins.Multitouch.Framework
{
	class MultitouchDevice:InputDevice
	{
		private readonly int id;
		private IInputElement target;

		public MultitouchDevice(int id, IInputElement element)
		{
			this.id = id;
			target = element;
		}

		public override IInputElement Target
		{
			get { return target; }
		}

		public override PresentationSource ActiveSource
		{
			get { return null; }
		}

		public void SetTarget(IInputElement inputElement)
		{
			target = inputElement;
		}
	}
}
