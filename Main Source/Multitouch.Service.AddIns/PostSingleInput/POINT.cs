using System;
using System.Runtime.InteropServices;

namespace PostSingleInput
{
	[StructLayout(LayoutKind.Sequential)]
	struct POINT
	{
		public int x;
		public int y;
		public POINT(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return string.Format("{0}x{1}", x, y);
		}
	}
}
