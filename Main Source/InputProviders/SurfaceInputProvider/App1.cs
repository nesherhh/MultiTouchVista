using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Multitouch.Contracts;
using Contact = Microsoft.Surface.Core.Contact;
using Point = System.Windows.Point;

namespace SurfaceInputProvider
{
	/// <summary>
	/// This is the main type for your application.
	/// </summary>
	public class App1 : Game
	{
		private readonly InputProvider inputProvider;
		private readonly List<Contact> oldContacts;

		private readonly GraphicsDeviceManager graphics;
		private ContactTarget contactTarget;
		private readonly Color backgroundColor = new Color(81, 81, 81);
		private bool applicationLoadCompleteSignalled;

		private UserOrientation currentOrientation = UserOrientation.Bottom;
		private Matrix screenTransform = Matrix.Identity;
		private Matrix inverted;

		// application state: Activated, Previewed, Deactivated,
		// start in Activated state
		private bool isApplicationActivated = true;
		private bool isApplicationPreviewed;

		/// <summary>
		/// The graphics device manager for the application.
		/// </summary>
		protected GraphicsDeviceManager Graphics
		{
			get { return graphics; }
		}

		/// <summary>
		/// The target receiving all surface input for the application.
		/// </summary>
		protected ContactTarget ContactTarget
		{
			get { return contactTarget; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		///<param name="inputProvider"></param>
		public App1(InputProvider inputProvider)
		{
			this.inputProvider = inputProvider;
			graphics = new GraphicsDeviceManager(this);
			oldContacts = new List<Contact>();
		}

		#region Initialization

		/// <summary>
		/// Moves and sizes the window to cover the input surface.
		/// </summary>
		private void SetWindowOnSurface()
		{
			Debug.Assert(Window.Handle != IntPtr.Zero,
				"Window initialization must be complete before SetWindowOnSurface is called");
			if (Window.Handle == IntPtr.Zero)
				return;

			// We don't want to run in full-screen mode because we need
			// overlapped windows, so instead run in windowed mode
			// and resize to take up the whole surface with no border.

			// Make sure the graphics device has the correct back buffer size.
			InteractiveSurface interactiveSurface = InteractiveSurface.DefaultInteractiveSurface;
			if (interactiveSurface != null)
			{
				graphics.PreferredBackBufferWidth = interactiveSurface.Width;
				graphics.PreferredBackBufferHeight = interactiveSurface.Height;
				graphics.ApplyChanges();

				// Remove the border and position the window.
				Program.RemoveBorder(Window.Handle);
				Program.PositionWindow(Window);
			}
		}

		/// <summary>
		/// Initializes the surface input system. This should be called after any window
		/// initialization is done, and should only be called once.
		/// </summary>
		private void InitializeSurfaceInput()
		{
			Debug.Assert(Window.Handle != IntPtr.Zero,
				"Window initialization must be complete before InitializeSurfaceInput is called");
			if (Window.Handle == IntPtr.Zero)
				return;
			Debug.Assert(contactTarget == null,
				"Surface input already initialized");
			if (contactTarget != null)
				return;

			// Create a target for surface input.
			contactTarget = new ContactTarget(IntPtr.Zero, true);
			contactTarget.EnableInput();
		}

		/// <summary>
		/// Reset the application's orientation and transform based on the current launcher orientation.
		/// </summary>
		private void ResetOrientation()
		{
			UserOrientation newOrientation = ApplicationLauncher.Orientation;

			if (newOrientation == currentOrientation) { return; }

			currentOrientation = newOrientation;

			screenTransform = currentOrientation == UserOrientation.Top ? inverted : Matrix.Identity;
		}

		#endregion

		#region Overridden Game Methods

		/// <summary>
		/// Allows the app to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			SetWindowOnSurface();
			InitializeSurfaceInput();

			// Set the application's orientation based on the current launcher orientation
			currentOrientation = ApplicationLauncher.Orientation;

			// Subscribe to surface application activation events
			ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
			ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
			ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;

			// Setup the UI to transform if the UI is rotated.
			// Create a rotation matrix to orient the screen so it is viewed correctly
			// when the user orientation is 180 degress different.
			inverted = Matrix.CreateRotationZ(MathHelper.ToRadians(180)) *
					   Matrix.CreateTranslation(graphics.GraphicsDevice.Viewport.Width,
												 graphics.GraphicsDevice.Viewport.Height,
												 0);

			if (currentOrientation == UserOrientation.Top)
			{
				screenTransform = inverted;
			}

			base.Initialize();
		}

		/// <summary>
		/// Load your graphics content.
		/// </summary>
		protected override void LoadContent()
		{
			// TODO: Load any content
		}

		/// <summary>
		/// Unload your graphics content.
		/// </summary>
		protected override void UnloadContent()
		{
			Content.Unload();
		}

		/// <summary>
		/// Allows the app to run logic such as updating the world,
		/// checking for collisions, gathering input and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (isApplicationActivated || isApplicationPreviewed)
			{
				if (isApplicationActivated)
				{
					// TODO: Process contacts, 
					// use the following code to get the state of all current contacts.
					IEnumerable<Contact> contacts = contactTarget.GetState();
					contacts = contacts.Where(contact => contact.IsFingerRecognized);

					IEnumerable<Contact> removedContacts = oldContacts.Except(contacts, ContactsComparer.Instance);
					IEnumerable<Contact> newContacts = contacts.Except(oldContacts, ContactsComparer.Instance);
					IEnumerable<Contact> movedContacts = contacts.Except(newContacts, ContactsComparer.Instance);


					List<Multitouch.Contracts.Contact> result = new List<Multitouch.Contracts.Contact>();

					ProcessContacts(removedContacts, result, ContactState.Removed);
					ProcessContacts(movedContacts, result, ContactState.Moved);
					ProcessContacts(newContacts, result, ContactState.New);

					//if(movedContacts.Count() > 0)
					//    Trace.WriteLine("Moved: " + movedContacts.Count());
					//if(newContacts.Count() > 0)
					//    Trace.WriteLine("Added: " + newContacts.Count());
					//if(removedContacts.Count() > 0)
					//    Trace.WriteLine("Removed: " + removedContacts.Count());

					
					inputProvider.OnNewFrame(new NewFrameEventArgs(Stopwatch.GetTimestamp(), result, Enumerable.Empty<Image>()));

					oldContacts.Clear();
					oldContacts.AddRange(contacts);
				}

				// TODO: Add your update logic here
			}

			base.Update(gameTime);
		}

		private static void ProcessContacts(IEnumerable<Contact> contacts, ICollection<Multitouch.Contracts.Contact> result, ContactState state)
		{
			foreach (Contact contact in contacts)
			{
				result.Add(new Multitouch.Contracts.Contact(contact.Id, state,
					new Point(contact.X, contact.Y), contact.MajorAxis, contact.MinorAxis));
			}
		}

		/// <summary>
		/// This is called when the app should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			if (!applicationLoadCompleteSignalled)
			{
				// Dismiss the loading screen now that we are starting to draw
				ApplicationLauncher.SignalApplicationLoadComplete();
				applicationLoadCompleteSignalled = true;
			}

			//TODO: Rotate the UI based on the value of screenTransform here if desired

			graphics.GraphicsDevice.Clear(backgroundColor);

			//TODO: Add your drawing code here
			//TODO: Avoid any expensive logic if application is neither active nor previewed

			base.Draw(gameTime);
		}

		#endregion

		#region Application Event Handlers

		/// <summary>
		/// This is called when application has been activated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnApplicationActivated(object sender, EventArgs e)
		{
			// Update application state.
			isApplicationActivated = true;
			isApplicationPreviewed = false;

			// Orientaton can change between activations.
			ResetOrientation();

			//TODO: Enable audio, animations here

			//TODO: Optionally enable raw image here
		}

		/// <summary>
		/// This is called when application is in preview mode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnApplicationPreviewed(object sender, EventArgs e)
		{
			// Update application state.
			isApplicationActivated = false;
			isApplicationPreviewed = true;

			//TODO: Disable audio here if it is enabled

			//TODO: Optionally enable animations here
		}

		/// <summary>
		///  This is called when application has been deactivated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnApplicationDeactivated(object sender, EventArgs e)
		{
			// Update application state.
			isApplicationActivated = false;
			isApplicationPreviewed = false;

			//TODO: Disable audio, animations here

			//TODO: Disable raw image if it's enabled
		}

		#endregion

		#region IDisposable

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Release managed resources.
				IDisposable graphicsDispose = graphics;
				if (graphicsDispose != null)
				{
					graphicsDispose.Dispose();
				}
				if (contactTarget != null)
				{
					contactTarget.Dispose();
					contactTarget = null;
				}
			}

			// Release unmanaged Resources.

			// Set large objects to null to facilitate garbage collection.

			base.Dispose(disposing);
		}


		#endregion

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
