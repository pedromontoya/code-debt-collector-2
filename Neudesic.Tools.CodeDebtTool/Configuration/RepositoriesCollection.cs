using System.Configuration;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	[ConfigurationCollection(typeof(RepositorySettingElement))]
	public class RepositoriesCollection : SettingsCollectionBase<RepositorySettingElement>
	{
		public RepositoriesCollection()
			: base("repository")
		{

		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((RepositorySettingElement)element).Name;
		}
	}
}
