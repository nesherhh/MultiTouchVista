using System;

namespace OpenCVTest.Filters
{
	public partial class GrayscaleVisualization : FilterVisualization
	{
		public GrayscaleVisualization(FilterContext context)
			: base(new Grayscale(context), context)
		{
			InitializeComponent();
		}
	}
}