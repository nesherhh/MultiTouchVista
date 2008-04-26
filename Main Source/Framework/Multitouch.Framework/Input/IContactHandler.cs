using System;

namespace Multitouch.Framework.Input
{
	public interface IContactHandler
	{
		void ProcessContactChange(int id, double x, double y, double width, double height, ContactState state);
	}
}
