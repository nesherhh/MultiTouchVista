#ifndef __JB_CONTROL_PANEL_APPLET__
#define __JB_CONTROL_PANEL_APPLET__
///////////////////////////////////////////////////////////////////////////////
//
// File           : $Workfile:   ControlApplet.hpp  $
// Version        : $Revision:   1.0  $
// Function       : Provide a framework for control panel applets.
//
// Author         : $Author:   Len  $
// Date           : $Date:   Dec 11 1997 21:49:08  $
//
// Notes          : 
// Modifications  :
//
// $Log:   D:/Documents/Len/Sources/Stuff/ControlApplet/PVCS/ControlApplet.hpv  $
// 
//    Rev 1.0   Dec 11 1997 21:49:08   Len
// Initial revision.
// 
///////////////////////////////////////////////////////////////////////////////
//
// Copyright 1997 JetByte Limited.
//
// JetByte Limited grants you ("Licensee") a non-exclusive, royalty free, 
// licence to use, modify and redistribute this software in source and binary 
// code form, provided that i) this copyright notice and licence appear on all 
// copies of the software; and ii) Licensee does not utilize the software in a 
// manner which is disparaging to JetByte Limited.
//
// This software is provided "AS IS," without a warranty of any kind. ALL
// EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND WARRANTIES, INCLUDING 
// ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE 
// OR NON-INFRINGEMENT, ARE HEREBY EXCLUDED. JETBYTE LIMITED AND ITS LICENSORS 
// SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF 
// USING, MODIFYING OR DISTRIBUTING THE SOFTWARE OR ITS DERIVATIVES. IN NO 
// EVENT WILL JETBYTE LIMITED BE LIABLE FOR ANY LOST REVENUE, PROFIT OR DATA, 
// OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, INCIDENTAL OR PUNITIVE 
// DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, ARISING 
// OUT OF THE USE OF OR INABILITY TO USE SOFTWARE, EVEN IF JETBYTE LIMITED 
// HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
//
// This software is not designed or intended for use in on-line control of
// aircraft, air traffic, aircraft navigation or aircraft communications; or in
// the design, construction, operation or maintenance of any nuclear
// facility. Licensee represents and warrants that it will not use or
// redistribute the Software for such purposes.
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Function prototypes for a control panel applet DLL.
///////////////////////////////////////////////////////////////////////////////

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpvReserved);
LONG CALLBACK CPlApplet(HWND hWnd, UINT uMsg, LONG lParam1, LONG lParam2);

///////////////////////////////////////////////////////////////////////////////
// Namespace: JetByteTools
///////////////////////////////////////////////////////////////////////////////

namespace JetByteTools {

///////////////////////////////////////////////////////////////////////////////
// Include files
///////////////////////////////////////////////////////////////////////////////

#include "cpl.h"

///////////////////////////////////////////////////////////////////////////////
// Classes defined in this file
///////////////////////////////////////////////////////////////////////////////

class CJBControlPanelApplet;  // The control panel applet base class.

///////////////////////////////////////////////////////////////////////////////
// CJBControlPanelApplet
///////////////////////////////////////////////////////////////////////////////

class CJBControlPanelApplet
{
   public :

      // Construction and destruction

      CJBControlPanelApplet(
         int nIconID, 
         int nNameID, 
         int nDescID);
      
      virtual ~CJBControlPanelApplet();

      // Applet message handling.

      virtual BOOL OnInit();
      virtual BOOL OnInquire(LPCPLINFO lpCPlInfo);
      virtual BOOL OnNewInquire(LPNEWCPLINFO lpNewCPlInfo);
      virtual BOOL OnDoubleClick(HWND hWnd, LONG appletData) = 0;
      virtual BOOL OnStartWithParams(HWND hWnd, LPSTR params);
      virtual BOOL OnStop(LONG appletData);
      virtual BOOL OnExit();
  
   private :

      // Applet data

      int m_nIconID;
      int m_nNameID;
      int m_nDescID;
      CJBControlPanelApplet *m_pNext;

      // Static access functions to manipulate all applets in this DLL.

      friend BOOL WINAPI ::DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpvReserved);
      friend LONG CALLBACK ::CPlApplet(HWND hWnd, UINT uMsg, LONG lParam1, LONG lParam2);

      static void SetInstanceHandle(HINSTANCE hInstance);

      static LONG Init();
      static LONG GetCount();
      static LONG Inquire(LONG appletIndex, LPCPLINFO lpCPlInfo);
      static LONG NewInquire(LONG appletIndex, LPNEWCPLINFO lpCPlInfo);
      static LONG DoubleClick(HWND hWnd, LONG lParam1, LONG lParam2);
      static LONG StartWithParams(HWND hWnd, LONG lParam1, LPSTR lParam2);
      static LONG Stop(LPARAM lParam1, LPARAM lParam2);
      static LONG Exit();
      
      // Private helper function

      static CJBControlPanelApplet *GetAppletByIndex(LONG index);

      // Static applet data 
      
      static HINSTANCE s_hInstance;
      static CJBControlPanelApplet *s_pListHead;

      // Disable copying: Do not implement these functions...

      CJBControlPanelApplet(const CJBControlPanelApplet &rhs);
      CJBControlPanelApplet &operator=(const CJBControlPanelApplet &rhs);
};

///////////////////////////////////////////////////////////////////////////////
// Namespace: JetByteTools
///////////////////////////////////////////////////////////////////////////////

} // End of namespace JetByteTools 

#endif // __JB_CONTROL_PANEL_APPLET__

///////////////////////////////////////////////////////////////////////////////
// End of file...
///////////////////////////////////////////////////////////////////////////////
