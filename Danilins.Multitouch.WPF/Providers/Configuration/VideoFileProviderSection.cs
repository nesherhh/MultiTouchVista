using System;
using System.Configuration;

namespace Danilins.Multitouch.Providers.Configuration
{
	internal class VideoFileProviderSection : FiltersSection
	{
		[ConfigurationProperty("videoFile", IsRequired = true)]
		public string VideoFile
		{
			get { return (string)this["videoFile"]; }
			set { this["videoFile"] = value; }
		}
	}
}