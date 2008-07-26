#pragma once
#include <vcclr.h>
#include "cv.h"
#include "cxcore.h"
#include "highgui.h"
#include "ITouchListener.h"
#include "TouchData.h"
#include "TouchScreenDevice.h"
#include "IInputCallback.h"

using namespace touchlib;
using namespace TouchLibProvider;

class TouchListener : public ITouchListener
{
private:
	ITouchScreen* screen;
	gcroot<IInputCallback^> callback;
	bool showDebugWindows;
	bool stop;
public:

	TouchListener(IInputCallback^ callback, bool showDebugWindows)
	{
		stop = false;
		this->callback = callback;
		this->showDebugWindows = showDebugWindows;
	}

	~TouchListener(void)
	{
		TouchScreenDevice::destroy();
	}

	void Start(void)
	{
		screen = TouchScreenDevice::getTouchScreen();
		screen->setDebugMode(showDebugWindows);
		
		System::String^ config = System::Environment::CurrentDirectory + "\\config.xml";
		char* cfg = (char*)(void*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(config);
		screen->loadConfig(cfg);

		std::string bgLabel = screen->findFirstFilter("backgroundremove");

		screen->registerListener(this);

		screen->beginProcessing();
		screen->beginTracking();

		do
		{
			int key = cvWaitKey(5);
			if(key == 98)
				screen->setParameter(bgLabel, "capture", "");
			
			screen->getEvents();
		}while(!stop);
	}

	void Stop(void)
	{
		stop = true;
	}

	virtual void fingerDown(TouchData data)
	{
		callback->FingerDown(data);
	}

	virtual void fingerUp(TouchData data)
	{
		callback->FingerUp(data);
	}

	virtual void fingerUpdate(TouchData data)
	{
		callback->FingerUpdate(data);
	}
};