using System;
using System.ComponentModel;
using System.Windows;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Controls
{
	public class ContactInfoModel:INotifyPropertyChanged
	{
		private readonly int id;
		private Rect position;
		private UIElement target;
		private Vector displacement;
		private Vector delta;
		private Point center;
		private ContactInfoModel oldModel;

		public event PropertyChangedEventHandler PropertyChanged;

		private ContactInfoModel(int id)
		{
			this.id = id;
		}

		public ContactInfoModel(ContactInfo contact)
		{
			id = contact.Id;
			Refresh(contact);
		}

		public int Id
		{
			get { return id; }
		}

		public Rect Position
		{
			get { return position; }
		}

		public Point Center
		{
			get { return center; }
		}

		public Vector Displacement
		{
			get { return displacement; }
		}

		public Vector Delta
		{
			get { return delta; }
		}

		public UIElement Target
		{
			get { return target; }
			set { target = value; }
		}

		public ContactInfoModel OldModel
		{
			get { return oldModel; }
		}

		public void Refresh(ContactInfo contact)
		{
			oldModel = new ContactInfoModel(id);
			oldModel.center = center;
			oldModel.position = position;
			oldModel.delta = delta;
			oldModel.displacement = displacement;
			oldModel.target = target;

			center = contact.Center;
			position = contact.Rectangle;
			delta = contact.Delta;
			displacement = contact.Displacement;

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("Position"));
				PropertyChanged(this, new PropertyChangedEventArgs("Center"));
				PropertyChanged(this, new PropertyChangedEventArgs("Delta"));
				PropertyChanged(this, new PropertyChangedEventArgs("Displacement"));
				PropertyChanged(this, new PropertyChangedEventArgs("Target"));
			}
		}
	}
}