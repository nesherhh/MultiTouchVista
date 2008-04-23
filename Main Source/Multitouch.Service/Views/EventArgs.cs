using System;

namespace Multitouch.Service.Views
{
	class EventArgs<T>:EventArgs
	{
		public T Item { get; private set; }

		public EventArgs(T item)
		{
			Item = item;
		}
	}
}
