using System;
using Multitouch.Contracts;

namespace MultipleMice
{
	class MouseContactChangedEventArgs : InputDataEventArgs
	{
		object data;

		public MouseContactChangedEventArgs(MouseContact contact)
		{
			data = contact;
		}

		public override InputType Type
		{
			get { return InputType.Contact; }
		}

		public override object Data
		{
			get { return data; }
		}
	}
}
