using System;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;

namespace Multitouch.Service.Logic.MEF
{
	class MultipleDirectoryCatalog : AggregateCatalog
	{
		public MultipleDirectoryCatalog(string path)
			: this(path, false, null)
		{ }

		public MultipleDirectoryCatalog(string path, bool watchDirector, string filter)
		{
			string directoryName = GetNormalizedDirectoryName(path);
			foreach (string subDirectory in Directory.GetDirectories(directoryName))
				Catalogs.Add(new DirectoryCatalog(subDirectory, watchDirector, filter));
		}

		// Paths are either Absolute, or relative to the AppDomainBase directory
		private static string GetNormalizedDirectoryName(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = GetRelativeDirectory("");
			}
			else
			{
				// Were we given an existing directory
				if (!Directory.Exists(path))
				{
					// Were we given a file name
					string dir = Path.GetDirectoryName(path);
					if (Directory.Exists(dir))
					{
						path = dir;
					}
					else
					{
						// Compute relative directory from AppDomain.ApplicationBase
						path = GetRelativeDirectory(path);

						if (!Directory.Exists(path))
						{
							throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, "directory not found - '{0}'", path));
						}
					}
				}
			}

			return Path.GetFullPath(path).ToUpperInvariant();
		}

		private static string GetRelativeDirectory(string relativePath)
		{
			string directory = relativePath;

			string baseDir = AppDomain.CurrentDomain.BaseDirectory;

			if (!string.IsNullOrEmpty(baseDir))
			{
				directory = Path.Combine(Path.GetDirectoryName(baseDir), relativePath);
			}

			return directory;
		}
	}
}