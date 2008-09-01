using System;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	public partial class Normalization : FilterVisualization
	{
		public Normalization(FilterContext context)
			: base(new ContrastStretch(), context)
		{
			InitializeComponent();
		}
	}
}