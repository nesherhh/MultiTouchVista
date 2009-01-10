using System;
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
			inputProvider.EnqueueContact(tuioCursor);
		}

		public void updateTuioCursor(TuioCursor tuioCursor)
		{
			inputProvider.EnqueueContact(tuioCursor);
		}

		public void removeTuioCursor(TuioCursor tuioCursor)
		{
			inputProvider.EnqueueContact(tuioCursor);
		}

		public void refresh(long timestamp)
		{
			inputProvider.RaiseNewFrame(timestamp);
		}
	}
}