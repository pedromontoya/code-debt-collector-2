using System.Configuration;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	public class FileTypeSettingElement : ConfigurationElement
	{
		[ConfigurationProperty("fileType", IsRequired = true, IsKey = true)]
		public string FileType
		{
			get
			{
				return (string)base["fileType"];
			}
			set
			{
				base["fileType"] = value;
			}
		}
		[ConfigurationProperty("commentIdentifiers", IsKey = true)]
		public string CommentIdentifiers
		{
			get
			{
				return (string)base["commentIdentifiers"];
			}
			set
			{
				base["commentIdentifiers"] = value;
			}
		}
	}
}
