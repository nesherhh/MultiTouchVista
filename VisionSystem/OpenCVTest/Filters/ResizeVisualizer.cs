using System;

namespace OpenCVTest.Filters
{
	public partial class ResizeVisualizer : FilterVisualization
	{
		public ResizeVisualizer(FilterContext context)
			: base(new Resize(context), context)
		{
			InitializeComponent();
		}
	}
}