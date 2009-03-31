using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls
{
    public class ElementRoutedEventArgs : RoutedEventArgs
    {
        private ContactEventArgs contactEventArgs;
        private FrameworkElement element;
        public FrameworkElement Element
        {
            get
            {
                return element;
            }
        }

        private Contact contact;
        public Contact Contact
        {
            get
            {
                return contact;
            }
        }

        private Point offset;
        public Point Offset
        {
            get
            {
                return offset;
            }
        }

        /// <summary>
        /// Gets the position relative to <see cref="IInputElement"/>.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <returns></returns>
        public Point GetPosition(UIElement relativeTo)
        {
            return contactEventArgs.GetPosition(relativeTo);
        }

        public ElementRoutedEventArgs(FrameworkElement element, Point offset, ContactEventArgs e)
        {
            contactEventArgs = e;
            contact = e.Contact;
            this.element = element;
            this.offset = offset;
        }
    }

    /// <summary>
    /// Extends <see cref="Multitouch.Framework.WPF.Controls.ScrollViewer"/> to respond to Multitouch events. It also has similar scrolling behavior like iPhone and dragging support and fires an event when an element is dragged out of it.
    /// </summary>
    public class DraggableScrollViewer : ScrollViewer
    {

        public delegate void ElementDraggedEventHandler
            (object sender, ElementRoutedEventArgs e);

        public static readonly RoutedEvent ElementDraggedEvent = EventManager.RegisterRoutedEvent("ElementDragged", RoutingStrategy.Direct, typeof(ElementDraggedEventHandler), typeof(DraggableScrollViewer));

        public event ElementDraggedEventHandler ElementDragged
        {
            add { AddHandler(ElementDraggedEvent, value); }
            remove { RemoveHandler(ElementDraggedEvent, value); }
        }

        private void RaiseElementDraggedEvent(FrameworkElement element, Point offset, ContactEventArgs e)
        {
            ElementRoutedEventArgs newEventArgs = new ElementRoutedEventArgs(element, offset, e);
            newEventArgs.RoutedEvent = ElementDraggedEvent;
            RaiseEvent(newEventArgs);
        }

        public DraggableScrollViewer()
        {
            AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactLeave);
            AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
        }

        private Dictionary<UIElement, Point> eltOffset = new Dictionary<UIElement, Point>();
        private Dictionary<int, UIElement> contactElements = new Dictionary<int, UIElement>();

        void OnNewContact(object sender, NewContactEventArgs e)
        {
            Point position = e.GetPosition(this);
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, position);
            if (hitTestResult != null)
            {
                FrameworkElement element = hitTestResult.VisualHit as FrameworkElement;
                if (element != null)
                {
                    FrameworkElement parent = element.Parent as FrameworkElement;
                    while (parent != null && !parent.Equals(sender))
                    {
                        element = parent;
                        parent = element.Parent as FrameworkElement;
                    }
                    if (eltOffset.ContainsKey(element))
                    {
                        contactElements[e.Contact.Id] = element;
                        eltOffset[element] = e.GetPosition(element);
                    }
                    else
                    {
                        contactElements.Add(e.Contact.Id, element);
                        eltOffset.Add(element, e.GetPosition(element));
                    }
                }
            }
        }

        void OnContactLeave(object sender, ContactEventArgs e)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitTestResult != null) return;

            if (contactElements.ContainsKey(e.Contact.Id) && eltOffset.ContainsKey(contactElements[e.Contact.Id]))
            {
                RaiseElementDraggedEvent(contactElements[e.Contact.Id] as FrameworkElement,
                                         eltOffset[contactElements[e.Contact.Id]], e);
                eltOffset.Remove(contactElements[e.Contact.Id]);
            }
        }
    }
}
