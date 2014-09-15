using System.Configuration;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	public class RepositorySettingElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
			set
			{
				base["name"] = value;
			}
		}
		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get
			{
				return (string)base["type"];
			}
			set
			{
				base["type"] = value;
			}
		}
		[ConfigurationProperty("connectionString")]
		public string ConnectionString
		{
			get
			{
				return (string)base["connectionString"];
			}
			set
			{
				base["connectionString"] = value;
			}
		}
	}
}
