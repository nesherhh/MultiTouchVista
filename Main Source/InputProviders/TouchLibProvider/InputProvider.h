#pragma once

#include "IInputCallback.h"
#include "TouchListener.h"
#include "TouchLibContactChangedEventArgs.h"

using namespace System::Collections::Generic;
using namespace System::Threading;
using namespace System::Windows;
using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	[System::AddIn::AddIn("TouchLibProvider", Publisher = "Daniel Danilin", Description = "Provides input from touchLib (http://nuigroup.com/touchlib)", Version = "2.0.0.0")]
	public ref class InputProvider : IInputCallback, IProvider
	{
	public:
		virtual event System::EventHandler<Multitouch::Contracts::NewFrameEventArgs^>^ NewFrame;
		
		InputProvider(void)
		{
			touchQueue = gcnew Queue<KeyValuePair<System::IntPtr, ContactState>>();
			timer = gcnew System::Timers::Timer(1000 / 60);
			timer->Elapsed += gcnew System::Timers::ElapsedEventHandler(this, &InputProvider::timer_Elapsed);
		}

		virtual property bool HasConfiguration
		{
			bool get(void)
			{
				return false;
			}
		}

		virtual property bool SendEmptyFrames;

		virtual UIElement^ GetConfiguration(void)
		{
			return nullptr;
		}

		virtual bool SendImageType(ImageType imageType, bool isEnable)
		{
			return false;
		}

		virtual property bool IsRunning
		{
			bool get(void)
			{
				return isRunning;
			}
		}

		virtual void Start(void)
		{
			if(listener)
				Stop();

			screenBounds = System::Windows::Forms::Screen::PrimaryScreen->Bounds;

			listener = new TouchListener(this, true);
			Thread^ thread = gcnew Thread(gcnew ThreadStart(this, &InputProvider::OnThreadStart));
			thread->Start();
		}

		virtual void Stop(void)
		{
			isRunning = false;
			listener->Stop();
			delete listener;
		}

	private:		
		bool isRunning;
		System::Timers::Timer^ timer;
		TouchListener* listener;
		Queue<KeyValuePair<System::IntPtr, ContactState>>^ touchQueue;
		System::Drawing::Rectangle screenBounds;

		void timer_Elapsed(System::Object^ sender, System::Timers::ElapsedEventArgs^ e)
		{
			Monitor::Enter(touchQueue);

			if(SendEmptyFrames || touchQueue->Count > 0)
			{
				NewFrame(this, gcnew TouchLibContactChangedEventArgs(screenBounds, touchQueue, System::Diagnostics::Stopwatch::GetTimestamp()));
			}

			Monitor::Exit(touchQueue);
		}

		void OnThreadStart(void)
		{
			listener->Start();
			isRunning = true;
		}

		virtual void FingerDown(TouchData data) = IInputCallback::FingerDown
		{
			Monitor::Enter(touchQueue);
			touchQueue->Enqueue(KeyValuePair<System::IntPtr, ContactState>((System::IntPtr)(void*)&data, ContactState::New));
			Monitor::Exit(touchQueue);
		}

		virtual void FingerUp(TouchData data) = IInputCallback::FingerUp
		{
			Monitor::Enter(touchQueue);
			touchQueue->Enqueue(KeyValuePair<System::IntPtr, ContactState>((System::IntPtr)(void*)&data, ContactState::Removed));
			Monitor::Exit(touchQueue);
		}

		virtual void FingerUpdate(TouchData data) = IInputCallback::FingerUpdate
		{
			Monitor::Enter(touchQueue);
			touchQueue->Enqueue(KeyValuePair<System::IntPtr, ContactState>((System::IntPtr)(void*)&data, ContactState::Moved));
			Monitor::Exit(touchQueue);
		}
	};
}