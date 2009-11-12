using System;
using System.Collections.Generic;
using System.Linq;
using ManagedWinapi.Windows;
using System.Windows.Forms;
using Microsoft.Surface.Core;
using Multitouch.Contracts;
using System.Diagnostics;
using Contact = Microsoft.Surface.Core.Contact;
using Image = Multitouch.Contracts.Image;
using System.Windows;
using System.Threading;

namespace SurfaceInputProvider
{
	class InputContext : ApplicationContext
	{
		readonly EventDispatchingNativeWindow window;
		readonly List<Contact> oldContacts;
		readonly ContactTarget contactTarget;
		readonly InputProvider inputProvider;
		readonly System.Windows.Forms.Timer timer;

		public InputContext(InputProvider provider)
		{
			inputProvider = provider;

			window = EventDispatchingNativeWindow.Instance;
			oldContacts = new List<Contact>();

			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

			contactTarget = new ContactTarget(IntPtr.Zero, true);
			contactTarget.EnableInput();

			timer = new System.Windows.Forms.Timer();
			timer.Interval = 1000 / 60;
			timer.Tick += timer_Tick;
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			IEnumerable<Contact> contacts = contactTarget.GetState();
			contacts = contacts.Where(contact => contact.IsFingerRecognized);

			IEnumerable<Contact> removedContacts = oldContacts.Except(contacts, ContactsComparer.Instance);
			IEnumerable<Contact> newContacts = contacts.Except(oldContacts, ContactsComparer.Instance);
			IEnumerable<Contact> movedContacts = contacts.Except(newContacts, ContactsComparer.Instance);


			List<Multitouch.Contracts.Contact> result = new List<Multitouch.Contracts.Contact>();

			ProcessContacts(removedContacts, result, ContactState.Removed);
			ProcessContacts(movedContacts, result, ContactState.Moved);
			ProcessContacts(newContacts, result, ContactState.New);

			//if (movedContacts.Count() > 0)
			//    Trace.WriteLine("Moved: " + movedContacts.Count());
			//if (newContacts.Count() > 0)
			//    Trace.WriteLine("Added: " + newContacts.Count());
			//if (removedContacts.Count() > 0)
			//    Trace.WriteLine("Removed: " + removedContacts.Count());

			inputProvider.OnNewFrame(new NewFrameEventArgs(Stopwatch.GetTimestamp(), result, Enumerable.Empty<Image>()));

			oldContacts.Clear();
			oldContacts.AddRange(contacts);
		}

		private static void ProcessContacts(IEnumerable<Contact> contacts, ICollection<Multitouch.Contracts.Contact> result, ContactState state)
		{
			foreach (Contact contact in contacts)
			{
				result.Add(new Multitouch.Contracts.Contact(contact.Id, state,
					new Point(contact.X, contact.Y), contact.MajorAxis, contact.MinorAxis));
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (contactTarget != null)
				contactTarget.Dispose();
			if (window != null)
				window.DestroyHandle();
			if (timer != null)
				timer.Stop();

			base.Dispose(disposing);
		}

		class ContactsComparer : IEqualityComparer<Contact>
		{
			private static ContactsComparer instance;

			public static ContactsComparer Instance
			{
				get
				{
					if (instance == null)
						instance = new ContactsComparer();
					return instance;
				}
			}

			public bool Equals(Contact x, Contact y)
			{
				return x.Id.Equals(y.Id);
			}

			/// <summary>
			/// Returns a hash code for the specified object.
			/// </summary>
			/// <returns>
			/// A hash code for the specified object.
			/// </returns>
			/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.
			///                 </param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
			///                 </exception>
			public int GetHashCode(Contact obj)
			{
				return obj.Id.GetHashCode();
			}
		}
	}
}
