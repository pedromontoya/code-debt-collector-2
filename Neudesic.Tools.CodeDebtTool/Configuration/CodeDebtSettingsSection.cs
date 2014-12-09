using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	public class CodeDebtSettingsSection : ConfigurationSection
	{
		[ConfigurationProperty("fileTypes", IsDefaultCollection = false)]
		[ConfigurationCollection(typeof(FileTypeSettingsCollection))]
		public FileTypeSettingsCollection FileTypesConfiguration
		{
			get
			{
				return (FileTypeSettingsCollection)base["fileTypes"];
			}
		}
        [ConfigurationProperty("ignorePaths", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(IgnorePathSettingsCollection))]
        public IgnorePathSettingsCollection IgnorePathsConfiguration
        {
            get
            {
                return (IgnorePathSettingsCollection)base["ignorePaths"];
            }
        }
		[ConfigurationProperty("debtKeys")]
		public string DebtKeys
		{
			get
			{
				return (string)base["debtKeys"];
			}
		}
	}

}
