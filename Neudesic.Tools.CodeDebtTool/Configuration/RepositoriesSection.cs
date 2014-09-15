using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	public class RepositoriesSection : ConfigurationSection
	{
		[ConfigurationProperty("repositories", IsDefaultCollection = false)]
		[ConfigurationCollection(typeof(RepositoriesCollection))]
		public RepositoriesCollection RepositoriesConfiguration
		{
			get
			{
				return (RepositoriesCollection)base["repositories"];
			}
		}
		[ConfigurationProperty("default")]
		public string DefaultRepository
		{
			get
			{
				return (string)base["default"];
			}
		}
	}
}
