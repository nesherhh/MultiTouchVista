using System;

namespace Danilins.Multitouch.Common.Filters
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class RangeAttribute:Attribute
	{
		public int Minimum { get; set; }
		public int Maximum { get; set; }

		public RangeAttribute(int maximum)
		{
			Minimum = 0;
			Maximum = maximum;
		}
	}
}
