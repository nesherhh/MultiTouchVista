using System;
using openCV;

namespace OpenCVTest
{
	public class FilterContext
	{
		public double Width { get; set; }
		public double Height { get; set; }
		public double Fps { get; set; }
		public IplImage Background;
	}
}
