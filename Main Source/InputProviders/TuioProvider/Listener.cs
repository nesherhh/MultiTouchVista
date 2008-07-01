using System;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	class Listener : TuioListener
	{
		readonly InputProvider inputProvider;

		public Listener(InputProvider inputProvider)
		{
			this.inputProvider = inputProvider;
		}

		public void addTuioObject(TuioObject tuioObject)
		{ }

		public void updateTuioObject(TuioObject tuioObject)
		{ }

		public void removeTuioObject(TuioObject tuioObject)
		{ }

		public void addTuioCursor(TuioCursor tuioCursor)
		{
			inputProvider.OnContactChanged(new TuioContactChangedEventArgs(tuioCursor, ContactState.New));
		}

		public void updateTuioCursor(TuioCursor tuioCursor)
		{
			inputProvider.OnContactChanged(new TuioContactChangedEventArgs(tuioCursor, ContactState.Moved));
		}

		public void removeTuioCursor(TuioCursor tuioCursor)
		{
			inputProvider.OnContactChanged(new TuioContactChangedEventArgs(tuioCursor, ContactState.Removed));
		}

		public void refresh(long timestamp)
		{ }
	}
}