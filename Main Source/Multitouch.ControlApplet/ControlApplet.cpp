///////////////////////////////////////////////////////////////////////////////
//
// File           : $Workfile:   ControlApplet.cpp  $
// Version        : $Revision:   1.0  $
// Function       : Provide a framework for control panel applets.
//
// Author         : $Author:   Len  $
// Date           : $Date:   Dec 11 1997 21:49:00  $
//
// Notes          : 
// Modifications  :
//
// $Log:   D:/Documents/Len/Sources/Stuff/ControlApplet/PVCS/ControlApplet.cpv  $
// 
//    Rev 1.0   Dec 11 1997 21:49:00   Len
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

#include "windows.h"

#include "ControlApplet.hpp"

using namespace JetByteTools;

///////////////////////////////////////////////////////////////////////////////
// DLL Entry points for Control Panel Applets
///////////////////////////////////////////////////////////////////////////////

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID /* lpvReserved */)
{
   if (dwReason == DLL_PROCESS_ATTACH)
	{
      CJBControlPanelApplet::SetInstanceHandle(hInstance);
   }

   return TRUE;
}

LONG CALLBACK CPlApplet( HWND hWnd, UINT uMsg, LONG lParam1, LONG lParam2 )
{
   LONG result = 0;

   switch( uMsg )
   {
      case CPL_INIT :                  // Applet initialisation 

         result = CJBControlPanelApplet::Init();

      break;

      case CPL_GETCOUNT :              // How many applets in the DLL?
         
         result = CJBControlPanelApplet::GetCount();

      break;

		case CPL_INQUIRE:				      //  Tell Control Panel about this applet
         
         result = CJBControlPanelApplet::Inquire(lParam1, (LPCPLINFO)lParam2);

      break;

      case CPL_NEWINQUIRE:				   //  Tell Control Panel about this applet

         result = CJBControlPanelApplet::NewInquire(
            lParam1, 
            (LPNEWCPLINFO)lParam2);
      
      break;

      case CPL_DBLCLK :                //  If applet icon is selected...
		
         result = CJBControlPanelApplet::DoubleClick(hWnd, lParam1, lParam2);

		break;

#if(WINVER >= 0x0400)

      case CPL_STARTWPARMS :           // Started with RUNDLL

         result = CJBControlPanelApplet::StartWithParams(
            hWnd, 
            lParam1, 
            (LPSTR)lParam2);

      break;
#endif

      case CPL_STOP :                  // Applet shutdown

         result = CJBControlPanelApplet::Stop(lParam1, lParam2);

      break;

      case CPL_EXIT :                  // DLL shutdown

         result = CJBControlPanelApplet::Exit();

      break;

		default:
			break;
	}

   return result;
}

///////////////////////////////////////////////////////////////////////////////
// Namespace: JetByteTools
///////////////////////////////////////////////////////////////////////////////

namespace JetByteTools {

///////////////////////////////////////////////////////////////////////////////
// CJBControlPanelApplet
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Static member variables
///////////////////////////////////////////////////////////////////////////////

CJBControlPanelApplet *CJBControlPanelApplet::s_pListHead = 0;

HINSTANCE CJBControlPanelApplet::s_hInstance = (HINSTANCE)INVALID_HANDLE_VALUE;

///////////////////////////////////////////////////////////////////////////////
// Construction and destruction
///////////////////////////////////////////////////////////////////////////////

CJBControlPanelApplet::CJBControlPanelApplet(
   int nIconID, 
   int nNameID, 
   int nDescID)
   :  m_nIconID(nIconID), 
      m_nNameID(nNameID), 
      m_nDescID(nDescID)
{
   m_pNext = s_pListHead;
   s_pListHead = this;
}

CJBControlPanelApplet::~CJBControlPanelApplet()
{
   // Remove ourselves from the list of all CJBControlPanelApplet objects

   CJBControlPanelApplet *pApplet = s_pListHead;
   CJBControlPanelApplet *pPrevious = NULL;
   
   // First find ourselves in the list...

   while (pApplet && this != s_pListHead)
   {
      pPrevious = pApplet;
      pApplet = pApplet->m_pNext;
   }

   if (pApplet)           // Sanity check...
   {
      // Remove ourselves

      if (pPrevious)
      {
         pPrevious = pApplet->m_pNext;
      }
      else
      {
         s_pListHead = pApplet->m_pNext;
      }
   }

   m_pNext = NULL;   // clear our link to the next object 
                     // just to be tidy and to please lint...
}

///////////////////////////////////////////////////////////////////////////////
// Static members to manage all CJBControlPanelApplet
///////////////////////////////////////////////////////////////////////////////

void CJBControlPanelApplet::SetInstanceHandle(HINSTANCE hInstance)
{
   s_hInstance = hInstance;
}

LONG CJBControlPanelApplet::Init()
{
   BOOL ok = TRUE;
   
   CJBControlPanelApplet *pApplet = s_pListHead;

   while (pApplet && ok)
   {
      ok = pApplet->OnInit();

      pApplet = pApplet->m_pNext;
   }

   return ok;
}

LONG CJBControlPanelApplet::GetCount()
{
   LONG numApplets = 0;

   CJBControlPanelApplet *pApplet = s_pListHead;

   while (pApplet)
   {
      numApplets++;
      pApplet = pApplet->m_pNext;
   }

   return numApplets;
}

CJBControlPanelApplet *CJBControlPanelApplet::GetAppletByIndex(LONG index)
{
   LONG numApplets = 0;
   CJBControlPanelApplet *pApplet = s_pListHead;

   while (pApplet && numApplets < index)
   {
      numApplets++;
      pApplet = pApplet->m_pNext;
   }
   
   return pApplet;
}

LONG CJBControlPanelApplet::Inquire(LONG appletIndex, LPCPLINFO lpCPlInfo)
{
   LONG result = 1;

   CJBControlPanelApplet *pApplet = GetAppletByIndex(appletIndex);
   
   if (pApplet && pApplet->OnInquire(lpCPlInfo))
   {
      result = 0;
   }

   return result;
}

LONG CJBControlPanelApplet::NewInquire(LONG appletIndex, LPNEWCPLINFO lpCPlInfo)
{
   LONG result = 1;
   
   CJBControlPanelApplet *pApplet = GetAppletByIndex(appletIndex);
   
   if (pApplet && pApplet->OnNewInquire(lpCPlInfo))
   {
      result = 0;
   }

   return result;
}

LONG CJBControlPanelApplet::DoubleClick(
   HWND hWnd, 
   LONG appletIndex, 
   LONG appletData)
{
   LONG result = 1;
   
   CJBControlPanelApplet *pApplet = GetAppletByIndex(appletIndex);
   
   if (pApplet && pApplet->OnDoubleClick(hWnd, appletData))
   {
      result = 0;
   }

   return result;
}

LONG CJBControlPanelApplet::StartWithParams(
   HWND hWnd, 
   LONG appletIndex, 
   LPSTR params)
{
   LONG result = 1;
   
   CJBControlPanelApplet *pApplet = GetAppletByIndex(appletIndex);
   
   if (pApplet && pApplet->OnStartWithParams(hWnd, params))
   {
      result = 0;
   }

   return result;
}

LONG CJBControlPanelApplet::Stop(LONG appletIndex, LONG appletData)
{
   LONG result = 1;
   
   CJBControlPanelApplet *pApplet = GetAppletByIndex(appletIndex);
   
   if (pApplet && pApplet->OnStop(appletData))
   {
      result = 0;
   }

   return result;
}

LONG CJBControlPanelApplet::Exit()
{
   LONG result = 0;
   
   CJBControlPanelApplet *pApplet = s_pListHead;

   while (pApplet)
   {
      if (!pApplet->OnExit())
      {
         result = 1;
      }

      pApplet = pApplet->m_pNext;
   }

   return result;
}

///////////////////////////////////////////////////////////////////////////////
// Default behaviour
///////////////////////////////////////////////////////////////////////////////

BOOL CJBControlPanelApplet::OnInit()
{
   return TRUE;
}

BOOL CJBControlPanelApplet::OnInquire(LPCPLINFO lpCPlInfo)
{
   lpCPlInfo->lData = 0;
   lpCPlInfo->idIcon = m_nIconID;
   lpCPlInfo->idName = m_nNameID;
   lpCPlInfo->idInfo = m_nDescID;
   
   return TRUE;
}

BOOL CJBControlPanelApplet::OnNewInquire(LPNEWCPLINFO lpNewCPlInfo)
{
   BOOL ok = FALSE;

   if (lpNewCPlInfo->dwSize == sizeof(NEWCPLINFO))
   {
      lpNewCPlInfo->dwSize = (DWORD)sizeof(NEWCPLINFO);
      lpNewCPlInfo->dwFlags = 0;
      lpNewCPlInfo->dwHelpContext = 0;
      lpNewCPlInfo->lData = 0;
      lpNewCPlInfo->hIcon = LoadIcon( s_hInstance, MAKEINTRESOURCE( m_nIconID ) );
      lpNewCPlInfo->szHelpFile[ 0 ] = '\0';
   
      LoadString( s_hInstance, m_nNameID, lpNewCPlInfo->szName, 32 );
      LoadString( s_hInstance, m_nDescID, lpNewCPlInfo->szInfo, 64 );

      ok = TRUE;
   }

   return ok;
}

BOOL CJBControlPanelApplet::OnStartWithParams(HWND hWnd, LPSTR /* params */)
{
   return OnDoubleClick(hWnd, 0);
}

BOOL CJBControlPanelApplet::OnStop(LONG /* appletData */)
{
   return TRUE;
}

BOOL CJBControlPanelApplet::OnExit()
{
   return TRUE;
}

///////////////////////////////////////////////////////////////////////////////
// Namespace: JetByteTools
///////////////////////////////////////////////////////////////////////////////

} // End of namespace JetByteTools 

///////////////////////////////////////////////////////////////////////////////
// End of file...
///////////////////////////////////////////////////////////////////////////////

