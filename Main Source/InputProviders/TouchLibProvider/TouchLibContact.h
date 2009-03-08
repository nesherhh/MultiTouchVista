#pragma once

#include "TouchData.h"

using namespace Multitouch::Contracts;
using namespace System::Windows;

namespace TouchLibProvider
{
	ref class TouchLibContact : ContactData
	{
	public:
		TouchLibContact(System::Drawing::Rectangle screenBounds, TouchData data, ContactState state)
			: ContactData(data.ID, state, Point(data.X * screenBounds.Width, data.Y * screenBounds.Height), data.width, data.height)
		{
			Orientation = data.angle;
		}
	};
}