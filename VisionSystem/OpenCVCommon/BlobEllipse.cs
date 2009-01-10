using System;
using System.Drawing;

namespace OpenCVCommon
{
	public struct BlobEllipse
	{
		public Point Center { get; set; }
		public Size Size { get; set; }
		public float Angle { get; set; }
		public int Id { get; set; }

		public BlobEllipse(Point center, Size size, float angle, int id)
			: this()
		{
			Center = center;
			Size = size;
			Angle = angle;
			Id = id;
		}
	}
}