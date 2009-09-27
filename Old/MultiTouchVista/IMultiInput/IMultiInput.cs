using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMultiInput
{
	public delegate void InputEventHandler(int ID, int x, int y, bool[] Buttons);
	public interface IMultiInput
	{
		void Initialize(Int32 HostHandle);
		string Name
		{
			get;
		}
		string Version
		{
			get;
		}
		string ReleaseDate
		{
			get;
		}
		string AboutInfo
		{
			get;
		}
		event InputEventHandler Input;
	}

}
