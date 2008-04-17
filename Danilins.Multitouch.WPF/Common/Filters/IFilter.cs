using System;

namespace Danilins.Multitouch.Common.Filters
{
	public interface IFilter
	{
		byte[] Process(int width, int height, ref int bitPerPixel, byte[] source);
		ResultData LastResult { get; }
	}
}
