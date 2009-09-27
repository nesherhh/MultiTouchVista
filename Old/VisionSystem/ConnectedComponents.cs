using System;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	public partial class ConnectedComponents : FilterVisualization
	{
		public ConnectedComponents(FilterContext context)
			: base(new ConnectedComponentsLabeling(), context)
		{
		}
	}
}