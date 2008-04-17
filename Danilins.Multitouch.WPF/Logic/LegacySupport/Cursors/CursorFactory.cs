using System;
using WFPoint = System.Drawing.Point;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	static class CursorFactory
	{
		public static ICursor CreateClickCursor(double x, double y)
		{
			//return new ClickCursorWPF(new System.Windows.Point(x, y));
			ClickCursor cursor = new ClickCursor();
			cursor.Show(new WFPoint((int)x, (int)y));
			return cursor;
		}

		public static ICursor CreateTimeCircleCursor(double x, double y)
		{
			//return new TimeCircleCursorWPF(new System.Windows.Point(x, y));
			TimeCircleCursor cursor = new TimeCircleCursor();
			cursor.Show(new WFPoint((int)x, (int)y));
			return cursor;
		}
	}
}
