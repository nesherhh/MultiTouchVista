#pragma once

#include "IInputCallback.h"
#include "TouchListener.h"

using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	[System::AddIn::AddIn("TouchLibProvider", Publisher = "Daniel Danilin", Description = "Provides input from touchLib (http://nuigroup.com/touchlib)", Version = "1.0.0.0")]
	public ref class InputProvider : IInputCallback, IProvider
	{
	public:
		InputProvider(void);
		virtual event System::EventHandler<Multitouch::Contracts::ContactChangedEventArgs^>^ ContactChanged;
		virtual void Start(void) sealed;
		virtual void Stop(void) sealed;
		property bool IsRunning
		{
			virtual bool get(void);
		}
		static property System::Drawing::Rectangle ScreenBounds
		{
			System::Drawing::Rectangle get(void);
		}
	private:		
		bool isRunning;
		TouchListener* listener;
		static System::Drawing::Rectangle screenBounds;
		virtual void FingerUp(TouchData data) sealed = IInputCallback::FingerUp;
		virtual void FingerUpdate(TouchData data) sealed = IInputCallback::FingerUpdate;
		virtual void FingerDown(TouchData data) sealed = IInputCallback::FingerDown;
		void OnThreadStart(void);
	};
}