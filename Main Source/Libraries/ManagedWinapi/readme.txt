ManagedWinapi 0.3
~~~~~~~~~~~~~~~~~

A collection of .NET components that wrap PInvoke calls to access 
native API by managed code.

For documentation for these functions, look at ManagedWinapi.chm in the
binary release or generate it with Sandcastle Help File Builder available
from http://codeplex.com/SHFB from the source release.

This library is licensed by the GNU Lesser General Public License.

For more information see the website at http://mwinapi.sourceforge.net/

You may contact me at <schierlm@users.sourceforge.net>.


ManagedWinapi Tools 0.3
~~~~~~~~~~~~~~~~~~~~~~~

Started as a collection of ManagedWinapi samples, the Managed Winapi tools
evolved into a toolbox of lots of small but useful tools that solve their
specific task well. They can still be used as reference for how to use
ManagedWinapi, but their functionality may make it hard to find the
relevant code parts quickly.

See the website for a list of tools and descriptions.


Changelog
~~~~~~~~~


+++ 2008-06-14 Released Version 0.3 +++


- Added AccessibleEventListener ProcessId and ThreadId members to limit 
  events to process or thread. [bbauer]
- Set default event for ClipboardNotifier, Crosshair and Hotkey,
  Add properties Location and Size and methods IsValid and SendClose to SystemWindow.
  [suggested by Frank Koch]
- Add functionality to low-level keyboard hook for translating low-level
  events to typed keys.
- Added several new classes:
  * Compute several kinds of machine IDs from C#
  * Shutdown the system
  * Set the system time
  * Get list of codepoints supported by a font
- Winternal Explorer: 
  * added window-highlighting while dragging crosshair
  * Add checkbox to disable heuristics when determining control from point
    (these heuristics fail for some MDI applications like Visual Studio)
  * reduce flickering when crosshair is dragged
  * Fix a crash when calling TreeNodeData.Equals with an instance of a different 
    subclass as its parameter
  * Fix refresh of tree when releasing crosshair
  * Fix PossibleParents so that windows that are children of a window of a different 
    process (most prominent example: screensaver preview in control panel) can be
    found in tree.
- bug fixes:
  * Fix a problem with applications that return invalid accessible children.
    [submitted by Karl Gyllstrom <karl at cs dot unc dot edu>]
  * Use the managed Control#Capture property instead of calling the SetCapture
    PInvoke method.
  * Fix unsetting of modifier keys in LockKeyResetter
  * Set proper scancode for injected keyboard events
    (some applications, like MS-DOS based ones, need them).
- Use Sandcastle Help File Builder instead of NDoc for the API documentation


+++ 2007-05-28 Released Version 0.2 +++


- New SystemWindow features
  * Lots of new SystemWindow properties (see the documentation)
  * support arbitrary length window class names.
  * Create SystemWindows not only from a Form but from any Control.
- Add classes to inspect list boxes, combo boxes, list views and
  tree views.
- WindowContents framework that allows to retrieve contents of
  windows from other processes 
  * From Textbox/Listbox/Combobox/ListView/TreeView 
  * From any window that supports screen readers
- Crosshair control (drag crosshair and get point at destination)
- Shortcut editing text box control
- Accessibility (Screen reader support)
  * New SystemAccessibleObject class that wraps accessibility 
    objects of other processes
  * AccessibleObjectListener for listening to accessible events
- Helper method to check API return values
- Managed Audio Mixer API
- Managed hooks (Local message hook, low level hooks and journal 
  record/playback hooks) (need ManagedWinapiNativeHelper.dll)
- ExtendedFileInfo (Get icon and hard disk size for files)
- Extract keyboard specific functions into new KeyboardKey class,
  add functions to synthesize keystrokes and mouse actions
- InputBlocker to block input while synthesizing input events
- ProcessMemoryChunk: Access memory of another process
- Add ManagedWinapi tools/samples:
  * AOExplorer: Browse accessibility objects
  * ContentsSaver: Save contents of list boxes and other controls to a text file
  * ClipHancer: Clipboard enhancer tool that provides unlimited number of 
    clipboards.
  * DeskIconRestore: Tool to save and restore desktop icon positions
  * GuessEXE: Tool that guesses programming language of another program.
  * NeatKeys: a program to resize windows neatly by keyboard.
  * QuickMacro: Simple macro recorder using Journal Hooks
  * ShootNotes: Make screen shots from parts of screen, put them on your screen 
    and annotate them
  * TreeSize#: TreeSize in C# (with more features than the original one)
  * Volume fader
  * WinternalExplorer: Explorer for windows, accessible objects and processes
- bug fixes
  * fix VS GUI designer problem (class order) in Hotkey.cs
  * fix setter for SystemWindow's VisibilityFlag property
  * fix trivial bug in SystemWindow's FromPointEx
  * LockKeyResetter resets stuck keys (Shift, Alt, Ctrl, ...) properly
- new build file that builds all tools and documentation automatically


+++ 2006-08-27 Released Version 0.1 +++