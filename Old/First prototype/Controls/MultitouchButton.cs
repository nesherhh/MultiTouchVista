using System;
using System.Windows;
using System.Windows.Controls;
using Danilins.Multitouch.Framework;

namespace Danilins.Multitouch.Controls
{
	public class MultitouchButton : Button
	{
		static MultitouchButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultitouchButton), new FrameworkPropertyMetadata(typeof(MultitouchButton)));
		}

		public MultitouchButton()
		{
			MultitouchScreen.AddContactClickHandler(this, OnContactClick);
			MultitouchScreen.AddContactDownHandler(this, OnContactDown);
			MultitouchScreen.AddContactUpHandler(this, OnContactUp);
			MultitouchScreen.AddContactLeaveHandler(this, OnContactLeave);
		}

		void OnContactLeave(object sender, ContactEventArgs e)
		{
			if (IsPressed)
				IsPressed = false;
		}

		protected virtual void OnContactUp(object sender, ContactEventArgs e)
		{
			IsPressed = false;
		}

		protected virtual void OnContactDown(object sender, ContactEventArgs e)
		{
			IsPressed = true;
		}

		protected virtual void OnContactClick(object sender, ContactEventArgs e)
		{
			OnClick();
		}
	}
}
