using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Framework.Input
{
	class ContactClickStates
	{
		class ClickState
		{
			public ClickState(ContactState state, DateTime time)
			{
				State = state;
				Time = time;
			}

			public ContactState State { get; set; }
			public DateTime Time { get; set; }
			public int Count { get; set; }
		}

		Dictionary<Rect, ClickState> states;
		Timer cleanUpTimer;

		public ContactClickStates()
		{
			states = new Dictionary<Rect, ClickState>();
			cleanUpTimer = new Timer(StatesCleanup);
			cleanUpTimer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
		}

		void StatesCleanup(object stateValue)
		{
			lock(states)
			{
				DateTime now = DateTime.Now;
				var toDelete = from state in states
							   where (now - state.Value.Time).Seconds > 3
							   select state;
				foreach (var d in toDelete.ToList())
					states.Remove(d.Key);
			}
		}

		public ContactAction GetAction(ContactInfo info)
		{
			ContactAction action;
			switch(info.State)
			{
				case ContactState.Down:
					action = ProcessDownState(info);
					break;
				case ContactState.Up:
					action = ProcessUpState(info);
					break;
				default:
					action = ContactAction.None;
					break;
			}
			return action;
		}

		ClickState SearchPreviousState(Point center)
		{
			lock (states)
			{
				var result = (from keyPair in states
							  where keyPair.Key.Contains(center)
							  select keyPair.Value).FirstOrDefault();
				return result;
			}
		}

		ContactAction ProcessDownState(ContactInfo info)
		{
			DateTime now = DateTime.Now;
			ClickState previousStatus = SearchPreviousState(info.Center);
			if (previousStatus == null)
			{
				lock (states)
				{
					states.Add(info.Rectangle, new ClickState(ContactState.Down, now));
				}
			}
			else
			{
				if (previousStatus.State == ContactState.Up && (now - previousStatus.Time).Milliseconds > 300)
					previousStatus.Count = 0;

				previousStatus.State = ContactState.Down;
				lock (states)
				{
					previousStatus.Time = now;
				}
			}
			return ContactAction.None;
		}

		ContactAction ProcessUpState(ContactInfo info)
		{
			ClickState previousState = SearchPreviousState(info.Center);
			DateTime now = DateTime.Now;
			if (previousState != null && previousState.State == ContactState.Down && (now - previousState.Time).Seconds < 1)
			{
				previousState.State = ContactState.Up;
				lock (states)
				{
					previousState.Time = now;
				}
				previousState.Count++;
				if (previousState.Count == 2)
					return ContactAction.DoubleClick;
				return ContactAction.Click;
			}
			return ContactAction.None;
		}
	}
}