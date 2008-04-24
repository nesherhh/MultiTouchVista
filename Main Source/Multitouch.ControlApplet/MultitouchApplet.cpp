#include "windows.h"
#include "ControlApplet.hpp"

using namespace JetByteTools;

class Applet1 : public CJBControlPanelApplet 
{
   public :

      Applet1() : CJBControlPanelApplet(100, 100, 101) {}

      BOOL OnDoubleClick(HWND hWnd, LONG appletData)
      {

		  // To Install this Applet, just copy Multitouch.cpl to the System32 folder
		  // Add code here to launch the C# Configuration app
		  // we will probably need to store the MTV install location in the registry

   	   MessageBox(hWnd, "Applet 1", "Hello", MB_ICONINFORMATION);

         return TRUE;
      }
};

Applet1 multitouchApplet;
