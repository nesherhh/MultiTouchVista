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

		public static readonly DependencyPropertyKey PicturesPropertyKey = DependencyProperty.RegisterReadOnly("Pictures",
			typeof(ObservableCollection<string>), typeof(Window1), new PropertyMetadata(null));
		public static readonly DependencyProperty PicturesProperty = PicturesPropertyKey.DependencyProperty;

		public Window1()
		{
			Contacts1 = new ObservableCollection<Contact>();
			Contacts2 = new ObservableCollection<Contact>();
			SetValue(PicturesPropertyKey, new ObservableCollection<string>());
			DataContext = this;
			InitializeComponent();
		}

		public ObservableCollection<string> Pictures
		{
			get { return (ObservableCollection<string>)GetValue(PicturesProperty); }
		}

		protected override void OnInitialized(EventArgs e)
		{
			string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			string[] pictures = Directory.GetFiles(picturesPath + @"\Riga", "*.jpg");
			foreach (string file in pictures.Take(5))
				Pictures.Add(file);

			base.OnInitialized(e);
		}

		private void list1_NewContact(object sender, NewContactEventArgs e)
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

		private void list2_NewContact(object sender, NewContactEventArgs e)
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("I was clicked");
		}

		int repeatButtonCount = 0;
		private void RepeatButton_Click(object sender, RoutedEventArgs e)
		{
			repeatButtonCount++;
			repeatButton.Content = repeatButtonCount;
		}
	}
}
