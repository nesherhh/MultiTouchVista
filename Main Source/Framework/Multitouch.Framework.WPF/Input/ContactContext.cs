using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Multitouch.Framework.WPF.Input
{
	using FrmContact = Framework.Input.Contact;

	class ContactContext
	{
		public ContactContext(FrmContact contact, UIElement root)
		{
			Contact = contact;
			Root = root;
			History = new Collection<FrmContact>();
		}

		public FrmContact Contact { get; private set; }
		public UIElement Root { get; private set; }

		public UIElementsList ElementsList { get; set; }
		public UIElement OverElement { get; set; }

		public Collection<FrmContact> History { get; set; }

		public ContactContext Clone()
		{
			return (ContactContext)MemberwiseClone();
		}
	}
}
