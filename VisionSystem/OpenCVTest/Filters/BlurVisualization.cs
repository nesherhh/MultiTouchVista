using System;

namespace OpenCVTest.Filters
{
	public partial class BlurVisualization : FilterVisualization
	{
		public BlurVisualization(FilterContext context)
			: base(new Blur(context), context)
		{
			InitializeComponent();
		}
	}
}