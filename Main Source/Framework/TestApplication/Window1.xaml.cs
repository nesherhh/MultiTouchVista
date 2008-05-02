using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Multitouch.Framework.WPF.Input;

namespace TestApplication
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1
	{
		public ObservableCollection<Contact> Contacts1 { get; private set; }
		public ObservableCollection<Contact> Contacts2 { get; private set; }

		public Window1()
		{
			Contacts1 = new ObservableCollection<Contact>();
			Contacts2 = new ObservableCollection<Contact>();
			DataContext = this;
			InitializeComponent();
		}
		protected override void OnInitialized(EventArgs e)
		{
			string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			string[] pictures = Directory.GetFiles(picturesPath, "*.jpg");
			itemsControl.ItemsSource = pictures.Take(5);

			base.OnInitialized(e);
		}

		private void list1_NewContact(object sender, ContactEventArgs e)
		{
			IDictionary<int, Contact> contacts = e.GetContacts(list1, MatchCriteria.LogicalParent);
			foreach (KeyValuePair<int, Contact> pair in contacts)
				Contacts1.Add(pair.Value);
		}

		private void list1_ContactMoved(object sender, ContactEventArgs e)
		{
			Contacts1.Clear();
			IDictionary<int, Contact> contacts = e.GetContacts(list1, MatchCriteria.LogicalParent);
			foreach (KeyValuePair<int, Contact> pair in contacts)
				Contacts1.Add(pair.Value);
		}

		private void list1_ContactRemoved(object sender, ContactEventArgs e)
		{
			Contact contact = e.Contact;
			if (IsParent(contact.Element, list1))
				Contacts1.Remove(contact);
		}

		private void list2_ContactMoved(object sender, ContactEventArgs e)
		{
			Contacts2.Clear();
			IDictionary<int, Contact> contacts = e.GetContacts(list2, MatchCriteria.LogicalParent);
			foreach (KeyValuePair<int, Contact> pair in contacts)
				Contacts2.Add(pair.Value);
		}

		private void list2_ContactRemoved(object sender, ContactEventArgs e)
		{
			Contact contact = e.Contact;
			if (IsParent(contact.Element, list2))
				Contacts2.Remove(contact);
		}

		private void list2_NewContact(object sender, ContactEventArgs e)
		{
			IDictionary<int, Contact> contacts = e.GetContacts(list2, MatchCriteria.LogicalParent);
			foreach (KeyValuePair<int, Contact> pair in contacts)
					Contacts2.Add(pair.Value);
		}

		bool IsParent(UIElement element, UIElement parent)
		{
			UIElement current = element;
			while (current != null && current != parent)
				current = VisualTreeHelper.GetParent(current) as UIElement;
			return current != null;
		}
	}
}
