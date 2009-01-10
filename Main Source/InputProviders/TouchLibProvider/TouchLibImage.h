#pragma once

namespace TouchLibProvider
{
	ref class TouchLibImage : IImageData
	{
	public:

		TouchLibImage(void)
		{}

		virtual property int BitsPerPixel;
		virtual property int Width;
		virtual property ImageType Type;
		virtual property int Stride;
		virtual property int Height;
		virtual property array<byte>^ Data;
	};
}