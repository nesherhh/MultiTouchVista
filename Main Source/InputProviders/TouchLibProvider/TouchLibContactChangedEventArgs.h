#pragma once

#include "TouchLibContact.h"

using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	ref class TouchLibContactChangedEventArgs : ContactChangedEventArgs
	{
	public:
		TouchLibContactChangedEventArgs(TouchData data, ContactState state)
		{
			contact = gcnew TouchLibContact(data, state);
		}

		property IContact^ Contact
		{
			virtual IContact^ get(void) override
			{
				return contact;
			}
		}
	private:
		IContact^ contact;
	};
}