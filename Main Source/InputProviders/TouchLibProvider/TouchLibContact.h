#pragma once

#include "TouchData.h"

using namespace Multitouch::Contracts;

namespace TouchLibProvider
{
	ref class TouchLibContact : IContactData
	{
	public:
		TouchLibContact(System::Drawing::Rectangle screenBounds, TouchData data, ContactState state)
		{
			id = data.ID;
			position = System::Windows::Point(data.X * screenBounds.Width, data.Y * screenBounds.Height);

			minorAxis = data.height;
			majorAxis = data.width;

			this->state = state;
			orientation = data.angle;
			bounds = System::Windows::Rect(data.X - data.width / 2, data.Y - data.height / 2, data.width, data.height);
		}

		property int Id
		{
			virtual int get(void)
			{
				return id;
			}
		}

		property System::Windows::Point Position
		{
			virtual System::Windows::Point get(void)
			{
				return position;
			}
		}

		property double MajorAxis
		{
			virtual double get(void)
			{
				return majorAxis;
			}
		}

		property double MinorAxis
		{
			virtual double get(void)
			{
				return minorAxis;
			}
		}

		property double Orientation
		{
			virtual double get(void)
			{
				return orientation;
			}
		}

		property System::Windows::Rect Bounds
		{
			virtual System::Windows::Rect get(void)
			{
				return bounds;
			}
		}

		property double Area
		{
			virtual double get(void)
			{
				return area;
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
		System::Windows::Point position;
		double majorAxis;
		double minorAxis;
		double orientation;
		System::Windows::Rect bounds;
		ContactState state;
		double area;
	};
}