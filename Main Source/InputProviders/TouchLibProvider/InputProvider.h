#pragma once

#include "IInputCallback.h"
#include "TouchListener.h"

using namespace System::Windows;
using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	[System::AddIn::AddIn("TouchLibProvider", Publisher = "Daniel Danilin", Description = "Provides input from touchLib (http://nuigroup.com/touchlib)", Version = "2.0.0.0")]
	public ref class InputProvider : IInputCallback, IProvider
	{
	public:
		InputProvider(void);
		virtual event System::EventHandler<Multitouch::Contracts::InputDataEventArgs^>^ Input;
		virtual void Start(void) sealed;
		virtual void Stop(void) sealed;
		
		virtual property bool HasConfiguration
		{
			bool get(void)
			{
				return false;
			}
		}

		virtual UIElement^ GetConfiguration(void)
		{
			return nullptr;
		}

		property bool IsRunning
		{
			virtual bool get(void);
		}
		static property System::Drawing::Rectangle ScreenBounds
		{
			System::Drawing::Rectangle get(void);
		}

		virtual bool SendImageType(ImageType imageType, bool isEnable)
		{
			return false;
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