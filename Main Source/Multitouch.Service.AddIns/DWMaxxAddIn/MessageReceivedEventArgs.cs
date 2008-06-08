using System;
using System.Windows.Forms;

namespace DWMaxxAddIn
{
	class MessageReceivedEventArgs : EventArgs
	{
		public Message Message { get; private set; }

		public MessageReceivedEventArgs(Message message)
		{
			Message = message;
		}
	}
}
