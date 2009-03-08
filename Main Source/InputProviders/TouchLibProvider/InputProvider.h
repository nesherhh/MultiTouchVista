#pragma once

#include "IInputCallback.h"
#include "TouchListener.h"

using namespace System::Collections::Generic;
using namespace System::Threading;
using namespace System::Windows;
using namespace System::Diagnostics;
using namespace System::ComponentModel::Composition;
using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	[AddIn("TouchLibProvider", Publisher = "Daniel Danilin", Description = "Provides input from touchLib (http://nuigroup.com/touchlib)", Version = "3.0.0.0")]
	[Export(IProvider::typeid)]
	public ref class InputProvider : IInputCallback, IProvider
	{
	public:
		virtual event System::EventHandler<Multitouch::Contracts::NewFrameEventArgs^>^ NewFrame;
		
		InputProvider(void)
		{
			touchQueue = gcnew Queue<Contact^>();
			timer = gcnew System::Timers::Timer(1000 / 60);
			timer->Elapsed += gcnew System::Timers::ElapsedEventHandler(this, &InputProvider::timer_Elapsed);
		}

		virtual ~InputProvider(void)
		{
			if(listener)
			{
				Stop();
				delete listener;
				listener = nullptr;
			}
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
			listener = nullptr;
		}

	private:		
		bool isRunning;
		System::Timers::Timer^ timer;
		TouchListener* listener;
		Queue<Contact^>^ touchQueue;
		System::Drawing::Rectangle screenBounds;

		void timer_Elapsed(System::Object^ sender, System::Timers::ElapsedEventArgs^ e)
		{
			Monitor::Enter(touchQueue);

			if(SendEmptyFrames || touchQueue->Count > 0)
			{
				NewFrame(this, gcnew NewFrameEventArgs(Stopwatch::GetTimestamp(), touchQueue, nullptr));
			}

			Monitor::Exit(touchQueue);
		}

		void OnThreadStart(void)
		{
			listener->Start();
			isRunning = true;
		}

		Contact^ GetContactData(TouchData data, ContactState state)
		{
			return gcnew Contact(data.ID, state, Point(data.X * screenBounds.Width, data.Y * screenBounds.Height), data.width, data.height);
		}

		virtual void FingerDown(TouchData data) = IInputCallback::FingerDown
		{
			Monitor::Enter(touchQueue);
			touchQueue->Enqueue(GetContactData(data, ContactState::New));
			Monitor::Exit(touchQueue);
		}

		virtual void FingerUp(TouchData data) = IInputCallback::FingerUp
		{
			Monitor::Enter(touchQueue);
			touchQueue->Enqueue(GetContactData(data, ContactState::Removed));
			Monitor::Exit(touchQueue);
		}

		virtual void FingerUpdate(TouchData data) = IInputCallback::FingerUpdate
		{
			Monitor::Enter(touchQueue);
			touchQueue->Enqueue(GetContactData(data, ContactState::Moved));
			Monitor::Exit(touchQueue);
		}
	};
}