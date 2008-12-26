using System;
using System.AddIn.Contract;

namespace Multitouch.Contracts.Contracts
{
	public interface IImageDataContract : IContract
	{
		int Width { get; }
		int Height { get; }
		int Stride { get; }
		int BitsPerPixel { get; }
		ImageType Type { get; }
		byte[] Data { get; }
	}
}