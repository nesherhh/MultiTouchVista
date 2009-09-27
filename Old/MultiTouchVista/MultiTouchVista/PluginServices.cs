using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

namespace MultiTouchVista
{
	public class PluginServices
	{

		public struct AvailablePlugin
		{
			public string AssemblyPath;
			public string ClassName;
		}

		public static AvailablePlugin[] FindPlugins(string strPath, string strInterface)
		{
			ArrayList Plugins = new ArrayList();
			string[] strDLLs;
			int intIndex;
			Assembly objDLL;
			//Go through all DLLs in the directory, attempting to load them
			strDLLs = Directory.GetFileSystemEntries(strPath, "*.dll");
			for (intIndex = 0; intIndex <= strDLLs.Length - 1; intIndex++)
			{
				try
				{
					objDLL = Assembly.LoadFrom(strDLLs[intIndex]);
					ExamineAssembly(objDLL, strInterface, Plugins);
				}
				catch (Exception e)
				{
					//Error loading DLL, we don't need to do anything special
				}
			}
			//Return all plugins found
			AvailablePlugin[] Results = new AvailablePlugin[Plugins.Count];
			if (Plugins.Count != 0)
			{
				Plugins.CopyTo(Results);
				return Results;
			}
			else
			{
				return null;
			}
		}

		private static void ExamineAssembly(Assembly objDLL, string strInterface, ArrayList Plugins)
		{
			//System.Type objType;
			Type objInterface;
			AvailablePlugin Plugin;
			//Loop through each type in the DLL
			foreach (Type objType in objDLL.GetTypes())
			{
				//Only look at public types
				if (objType.IsPublic == true) {
					//Ignore abstract classes
					if (!((objType.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)) {
						//See if this type implements our interface
						objInterface = objType.GetInterface(strInterface, true);
						if ((objInterface != null)) {
							//It does
							Plugin = new AvailablePlugin();
							Plugin.AssemblyPath = objDLL.Location;
							Plugin.ClassName = objType.FullName;
							Plugins.Add(Plugin);
						}
					}
				}
			}
		}

		public static object CreateInstance(AvailablePlugin Plugin)
		{
			Assembly objDLL;
			object objPlugin;
			try
			{
				//Load dll
				objDLL = Assembly.LoadFrom(Plugin.AssemblyPath);
				//Create and return class instance
				objPlugin = objDLL.CreateInstance(Plugin.ClassName);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return null;
			}
			return objPlugin;
		}

	}

}




