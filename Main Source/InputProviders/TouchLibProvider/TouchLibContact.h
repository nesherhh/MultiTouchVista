#pragma once

#include "TouchData.h"

using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	ref class TouchLibContact : IContact
	{
	public:
		TouchLibContact(TouchData data, ContactState state)
		{
			id = data.ID;
			x = data.X * InputProvider::ScreenBounds.Width;
			y = data.Y * InputProvider::ScreenBounds.Height;
			width = data.width;
			height = data.height;
			this->state = state;
		}

		property int Id
		{
			virtual int get(void)
			{
				return id;
			}
		}

		property double X
		{
			virtual double get(void)
			{
				return x;
			}
		}

		property double Y
		{
			virtual double get(void)
			{
				return y;
			}
		}

		property double Width
		{
			virtual double get(void)
			{
				return width;
			}
		}

		property double Height
		{
			virtual double get(void)
			{
				return height;
			}
		}

		property ContactState State
		{
			virtual ContactState get(void)
			{
				return state;
			}
		}

	private:
		int id;
		double x;
		double y;
		double width;
		double height;
		ContactState state;
	};
}