using System.Configuration;

namespace Neudesic.Tools.CodeDebt.Configuration
{
    public class IgnorePathSettingElement : ConfigurationElement
	{
		[ConfigurationProperty("ignorePath", IsRequired = true, IsKey = true)]
        public string IgnorePath
		{
			get
			{
				return (string)base["ignorePath"];
			}
			set
			{
				base["ignorePath"] = value;
			}
		}
	}
}
