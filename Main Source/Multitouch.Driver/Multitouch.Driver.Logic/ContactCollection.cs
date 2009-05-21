using System;

namespace Multitouch.Driver.Logic
{
	class ContactCollection : ThreadSafeKeyedCollection<HidContactInfo>
	{
		/// <summary>
		/// Get <see cref="HidContactInfo"/> by its ID.
		/// </summary>
		/// <param name="id">ID of a contact</param>
		/// <param name="contact">Contact, if it is found, otherwise <c>null</c></param>
		/// <returns><c>true</c> if contact found, otherwise <c>falls</c></returns>
		public bool TryGet(int id, out HidContactInfo contact)
		{
			contact = null;
			foreach (HidContactInfo info in this)
			{
				if (info.Id == id)
				{
					contact = info;
					return true;
				}
			}
			return false;
		}
	}
}