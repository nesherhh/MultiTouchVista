using System;
using System.IO;
using System.Text;

namespace Multitouch.Service.Views
{
	class OutputStream:TextWriter
	{
		public event EventHandler<EventArgs<string>> Updated;

		public override Encoding Encoding
		{
			get { return Encoding.Unicode; }
		}

		public override void WriteLine(string line)
		{
			base.WriteLine(line);
			OnUpdated(line);
		}

		protected virtual void OnUpdated(string line)
		{
			EventHandler<EventArgs<string>> updated = Updated;
			if(updated != null)
				updated(this, new EventArgs<string>(line));
		}
	}
}
