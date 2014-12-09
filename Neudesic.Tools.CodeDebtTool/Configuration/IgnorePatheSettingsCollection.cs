using System;
using System.Configuration;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	[ConfigurationCollection(typeof(IgnorePathSettingElement))]
	public class IgnorePathSettingsCollection : SettingsCollectionBase<IgnorePathSettingElement>
	{
        public IgnorePathSettingsCollection()
			: base("ignorePath")
		{

		}

		protected override object GetElementKey(ConfigurationElement element)
		{
            return ((IgnorePathSettingElement)element).IgnorePath;
		}
	}
}
