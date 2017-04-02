**How to compile recent check-ins:**

You will need .NET Framework 3.5 with Service Pack 1 and Visual Studio 2008 and [Windows SDK](http://www.microsoft.com/downloads/details.aspx?FamilyId=F26B1AA4-741A-433A-9BE5-FA919850BDBF&displaylang=en) to compile MultiTouchVista. It should work also with Express edition of Visual Studio but I didn't check this out.

Download one of the recent cheange sets and extract zip file.
You have to open and compile solutions in Visual Studio from _Main Source_ folder in the following order:
# Multitouch.InputProviders.sln
# Multitouch.Service.sln
# Multitouch.Configuration.sln
# Multitouch.Framework.sln
# Multitouch.Driver.sln

For Multitouch.Framework.sln you need Windows SDK installed.

_Multitouch.Framework_ includes _TestApplication_ project that you can use to play with the framework and see how to use it. But before you start this project you have to start _Multitouch.Service.Console.exe_ located in folder _Main Source\Output_. After start you will see several (one for each attached mouse) mouse cursors with red dots. This is your emulated "fingers". Use this dots to interact with _TestApplication_.

If you don't want to start _Multitouch.Service.Console.exe_ each time, you can install _Multitouch.Service.exe_ as a Windows Service.
To do this run this line with elevated privileges: _installutil.exe Multitouch.Service.exe_. After this you can use Services in Control Panel to start and stop _Multitouch input_ service.

Hope this helps you to get started. If you have any questions please post them in Discussions.