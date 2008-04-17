using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Logic.LegacySupport.Cursors;
using WFPoint = System.Drawing.Point;

namespace Danilins.Multitouch.Logic.LegacySupport
{
	class ContactData : IDisposable
	{
		Dispatcher dispatcher;

		public int Id { get; set; }
		public Point Location { get; set; }

		Timer timer;
		bool timerFired;
		int timerFiredCount;
		bool hasMoved;
		bool disposed;
		ICursor cursor;

		public ContactData(ContactInfo info, Dispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			timerFired = false;
			timerFiredCount = 0;
			disposed = false;

			Id = info.Id;
			Location = info.Rectangle.Location;
			timer = new Timer(1000);
			timer.AutoReset = false;
			timer.Elapsed += timer_Elapsed;
			timer.Start();

			ShowTimerCircleCursor();
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!hasMoved && !disposed)
			{
				timerFired = true;
				timerFiredCount++;
				if (timerFiredCount < 2)
					timer.Start();
				else if (timerFiredCount == 2)
				{
					CloseTimerCursor();
					ContactActionHelper.LeftDown(Location);
				}
			}
		}

		public void Dispose()
		{
			disposed = true;
			CloseTimerCursor();
			
			if (!hasMoved)
			{
				if (!timerFired)
				{
					ShowClickCursor();
					ContactActionHelper.LeftTap(Location);
				}
				else if (timerFiredCount == 1)
					ContactActionHelper.RightTap(Location);
				else if (timerFiredCount == 2)
				{
					ShowClickCursor();
					ContactActionHelper.LeftUp(Location);
				}
			}
			else
			{
				if (timerFired)
					ContactActionHelper.RightUp(Location);
				else
					ContactActionHelper.LeftUp(Location);
			}
		}

		private void ShowTimerCircleCursor()
		{
			dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate { cursor = CursorFactory.CreateTimeCircleCursor(Location.X, Location.Y); });
		}

		private void ShowClickCursor()
		{
			dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate { CursorFactory.CreateClickCursor(Location.X, Location.Y); });
		}

		private void CloseTimerCursor()
		{
			if (cursor != null)
			{
				dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate { cursor.Close(); });
				cursor = null;
			}
		}

		public void Move(ContactInfo info)
		{
			if (disposed)
				throw new ObjectDisposedException("Contact is already up");

			double distance = (info.Rectangle.Location - Location).Length;
			if (distance > 1)
			{
				CloseTimerCursor();

				if (!hasMoved)
				{
					if (timerFired)
						ContactActionHelper.RightDown(Location);
					else
						ContactActionHelper.LeftDown(Location);
				}

				hasMoved = true;
				Location = info.Rectangle.Location;
				ContactActionHelper.Drag(Location);
			}
		}
	}
}