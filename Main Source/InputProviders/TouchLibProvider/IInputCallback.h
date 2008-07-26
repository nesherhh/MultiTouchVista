#pragma once

#include "TouchData.h"

namespace TouchLibProvider
{
	interface class IInputCallback
	{
		void FingerUp(TouchData data) abstract;
		void FingerDown(TouchData data) abstract;
		void FingerUpdate(TouchData data) abstract;
	};
}