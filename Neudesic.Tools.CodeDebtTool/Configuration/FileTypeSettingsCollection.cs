using System;
using System.Configuration;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	[ConfigurationCollection(typeof(FileTypeSettingElement))]
	public class FileTypeSettingsCollection : SettingsCollectionBase<FileTypeSettingElement>
	{
		public FileTypeSettingsCollection()
			: base("fileType")
		{

		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FileTypeSettingElement)element).FileType;
		}
	}
}
