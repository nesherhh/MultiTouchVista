using System;
using System.Collections.ObjectModel;

namespace Multitouch.Driver.Logic
{
	class ReportContacts : Collection<HidContactInfo>
	{
		protected override void InsertItem(int index, HidContactInfo item)
		{
			if (Count == MultiTouchReport.MaxContactsPerReport)
				throw new InvalidOperationException(string.Format("max number of contacts per report - {0}", MultiTouchReport.MaxContactsPerReport));
			base.InsertItem(index, item);
		}
	}
}
