#pragma once

#include "TouchLibContact.h"

using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	ref class TouchLibContactChangedEventArgs : InputDataEventArgs
	{
	public:
		TouchLibContactChangedEventArgs(TouchData data, ContactState state)
		{
			this->data = gcnew TouchLibContact(data, state);
		}
		
		property InputType Type
		{
			virtual InputType get(void) override
			{
				return InputType::Contact;
			}
		}

		property Object^ Data
		{
			virtual Object^ get(void) override
			{
				return data;
			}
		}
	private:
		Object^ data;
	};
}