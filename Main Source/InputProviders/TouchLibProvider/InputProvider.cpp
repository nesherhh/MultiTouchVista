#include "StdAfx.h"
#include "InputProvider.h"
#include "TouchListener.h"

#include "TouchLibContactChangedEventArgs.h"

using namespace System::Threading;
using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	InputProvider::InputProvider(void)
	{}

	void InputProvider::FingerDown(TouchData data)
	{
		Input(this, gcnew TouchLibContactChangedEventArgs(data, ContactState::New));
	}

	void InputProvider::FingerUp(TouchData data)
	{
		Input(this, gcnew TouchLibContactChangedEventArgs(data, ContactState::Removed));
	}

	void InputProvider::FingerUpdate(TouchData data)
	{
		Input(this, gcnew TouchLibContactChangedEventArgs(data, ContactState::Moved));
	}

	bool InputProvider::IsRunning::get(void)
	{
		return isRunning;
	}

	void InputProvider::Start(void)
	{
		if(listener)
			Stop();

		screenBounds = System::Windows::Forms::Screen::PrimaryScreen->Bounds;

		listener = new TouchListener(this, true);
		Thread^ thread = gcnew Thread(gcnew ThreadStart(this, &TouchLibProvider::InputProvider::OnThreadStart));
		thread->Start();
	}

	void InputProvider::OnThreadStart(void)
	{
		listener->Start();
		isRunning = true;
	}

	void InputProvider::Stop(void)
	{
		isRunning = false;
		listener->Stop();
		delete listener;
	}

	System::Drawing::Rectangle InputProvider::ScreenBounds::get(void)
	{
		return screenBounds;
	}
}