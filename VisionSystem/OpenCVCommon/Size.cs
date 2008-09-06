using System;

namespace OpenCVCommon
{
	public struct Size
	{
		public float Height { get; set; }
		public float Width { get; set; }

		public Size(float height, float width)
			: this()
		{
			Height = height;
			Width = width;
		}
	}
}