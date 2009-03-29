using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	class UIElementsList : IEnumerable<DependencyObject>
	{
		public DependencyObject NewOver { get; set; }

		readonly IDictionary<DependencyObject, ContactState> states;
		readonly List<DependencyObject> list;

		public UIElementsList(DependencyObject over)
		{
			NewOver = over;
			list = new List<DependencyObject>();
			states = new Dictionary<DependencyObject, ContactState>();
		}

		public bool Contains(DependencyObject element)
		{
			return list.Contains(element);
		}

		public ContactState GetState(DependencyObject element)
		{
			return states[element];
		}

		public void Add(DependencyObject element, ContactState state)
		{
			states.Add(element, state);
			list.Add(element);
		}

		public IEnumerator<DependencyObject> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static UIElementsList BuildDifference(UIElementsList newList, UIElementsList oldList)
		{
			if (oldList == null)
				return newList;
			if(newList == null)
				return null;
			UIElementsList result = new UIElementsList(oldList.NewOver);
			result.list.AddRange(oldList.list);
			foreach (DependencyObject obj in oldList.list)
				result.states.Add(obj, ContactState.Removed);
			result.NewOver = newList.NewOver;
			foreach (DependencyObject obj in newList.list)
			{
				if(result.states.ContainsKey(obj))
					result.states[obj] = ContactState.Moved;
				else
				{
					result.list.Add(obj);
					result.states.Add(obj, ContactState.New);
				}
			}
			return result;
		}
	}
}
