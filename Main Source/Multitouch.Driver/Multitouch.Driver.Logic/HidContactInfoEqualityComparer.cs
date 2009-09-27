using System;
using System.Collections.Generic;

namespace Multitouch.Driver.Logic
{
	class HidContactInfoEqualityComparer : IEqualityComparer<HidContactInfo>
	{
		public bool Equals(HidContactInfo x, HidContactInfo y)
		{
			return x.Id.Equals(x.Id);
		}

		public int GetHashCode(HidContactInfo obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
