using System;

namespace Danilins.Multitouch.Controls
{
	public interface IMultitouchable
	{
		ContactInfoModelCollection Contacts { get; }
	}
}