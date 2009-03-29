﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Represents a contact.
	/// </summary>
	public class Contact : InputDevice
	{
		internal RawMultitouchReport InputArgs { get; set; }

		internal Contact(RawMultitouchReport report)
		{
			InputArgs = report;
		}

		/// <summary>
		/// Id of this contact.
		/// </summary>
		public int Id
		{
			get { return InputArgs.Context.Contact.Id; }
		}

		/// <summary>
		/// When overridden in a derived class, gets the element that receives input from this device.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The element that receives input.
		/// </returns>
		public override IInputElement Target
		{
			get { return InputArgs.Context.OverElement; }
		}

		/// <summary>
		/// Gets the directly over.
		/// </summary>
		/// <value>The directly over.</value>
		public IInputElement DirectlyOver
		{
			get { return InputArgs.Context.OverElement; }
		}

		/// <summary>
		/// Gets the object that has captured this contact.
		/// </summary>
		/// <value>The captured.</value>
		public IInputElement Captured
		{
			get { return InputArgs.Captured as IInputElement; }
		}

		/// <summary>
		/// When overridden in a derived class, gets the <see cref="T:System.Windows.PresentationSource"/> that is reporting input for this device.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The source that is reporting input for this device.
		/// </returns>
		public override PresentationSource ActiveSource
		{
			get { return PresentationSource.FromVisual(InputArgs.Context.Root); }
		}

		/// <summary>
		/// Get position relative to <see cref="UIElement"/>
		/// </summary>
		/// <param name="relativeTo">Position relative to this object. If <c>null</c>, then result will be relative to the root visual.</param>
		public Point GetPosition(UIElement relativeTo)
		{
			if (relativeTo != null)
				return InputArgs.Context.Root.TranslatePoint(InputArgs.Context.Contact.Position, relativeTo);
			return InputArgs.Context.Contact.Position;
		}

		/// <summary>
		/// Gets the history of points that was generated by this contact
		/// </summary>
		/// <param name="relativeTo">Positions relative to this object. If <c>null</c>, then result will be relative to the root visual.</param>
		/// <returns></returns>
		public IEnumerable<Point> GetPoints(UIElement relativeTo)
		{
			if (relativeTo != null)
				return InputArgs.Context.History.Select(c => InputArgs.Context.Root.TranslatePoint(c.Position, relativeTo));
			return InputArgs.Context.History.Select(c => c.Position);
		}

		/// <summary>
		/// Captures the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="captureMode">The capture mode.</param>
		/// <returns></returns>
		public bool Capture(IInputElement element, CaptureMode captureMode)
		{
			return MultitouchLogic.Current.ContactsManager.Capture(this, element, captureMode);
		}

		/// <summary>
		/// Captures the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public bool Capture(IInputElement element)
		{
			return Capture(element, CaptureMode.Element);
		}

		/// <summary>
		/// Releases the capture.
		/// </summary>
		public void ReleaseCapture()
		{
			MultitouchLogic.Current.ContactsManager.Capture(this, null, CaptureMode.None);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return InputArgs.Context.Contact.ToString();
		}

		/// <summary>
		/// Gets the area of contact
		/// </summary>
		public double Area
		{
			get { return InputArgs.Context.Contact.Area; }
		}

		/// <summary>
		/// Gets the bounding rect of contact
		/// </summary>
		public Rect BoundingRect
		{
			get { return InputArgs.Context.Contact.Bounds; }
		}

		/// <summary>
		/// Gets the major axis for an ellipse
		/// </summary>
		public double MajorAxis
		{
			get { return InputArgs.Context.Contact.MajorAxis; }
		}

		/// <summary>
		/// Gets the minor axis for an ellipse
		/// </summary>
		public double MinorAxis
		{
			get { return InputArgs.Context.Contact.MinorAxis; }
		}
	}
}