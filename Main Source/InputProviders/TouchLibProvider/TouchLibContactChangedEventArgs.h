#pragma once

#include "TouchLibContact.h"
#include "TouchLibImage.h"

using namespace System::Collections::Generic;
using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	ref class TouchLibContactChangedEventArgs : NewFrameEventArgs
	{
	public:
		TouchLibContactChangedEventArgs(System::Drawing::Rectangle screenBounds, IEnumerable<KeyValuePair<System::IntPtr, ContactState>>^ touches, long timestamp)
		{
			contacts = gcnew List<IContactData^>();
			images = gcnew List<IImageData^>();
			this->timestamp = timestamp;

			for each(KeyValuePair<System::IntPtr, ContactState> touch in touches)
			{
				TouchData touchData = *(TouchData*)(void*)touch.Key;
				TouchLibContact^ contact = gcnew TouchLibContact(screenBounds, touchData, touch.Value);
				contacts->Add(contact);
			}
		}
		
		virtual property System::Int64 Timestamp 
		{
			System::Int64 get(void) override
			{
				return timestamp;
			}
		}
		
		virtual property IList<IImageData^>^ Images
		{
			IList<IImageData^>^ get(void) override
			{
				return images;
			}
		}

		virtual property IList<IContactData^>^ Contacts
		{
			IList<IContactData^>^ get(void) override
			{
				return contacts;
			}
		}

	private:
		long timestamp;
		IList<IContactData^>^ contacts;
		IList<IImageData^>^ images;
	};
}