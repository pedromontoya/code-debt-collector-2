using Neudesic.Tools.CodeDebt.Configuration;
using System;
using System.Configuration;
using System.Linq;

namespace Neudesic.Tools.CodeDebt.Repositories
{
	public static class RepositoryFactory
	{
		private static ICodeDebtRepository ms_repository = null;
		public static ICodeDebtRepository Instance
		{
			get
			{
				if (ms_repository == null)
				{
					RepositoriesSection repositorySettings = (RepositoriesSection)ConfigurationManager.GetSection("codeDebtRepositorySection");
					string name = ConfigurationManager.AppSettings["Repository"] ?? repositorySettings.DefaultRepository;

					var repositorySetting = (from RepositorySettingElement e in repositorySettings.RepositoriesConfiguration
											 where e.Name == name
											 select e).First();
					if (repositorySetting == null)
						throw new ArgumentException(string.Format("Could not find repository named {0}.", name), "name");
					ms_repository = Create(repositorySetting);
				}
				return ms_repository;
			}
		}

		private static ICodeDebtRepository Create(RepositorySettingElement repositorySetting)
		{
			Type repositoryType = Type.GetType(repositorySetting.Type);
			ICodeDebtRepository repository = Activator.CreateInstance(repositoryType) as ICodeDebtRepository;
			repository.Name = repositorySetting.Name;
			repository.ConnectionString = repositorySetting.ConnectionString;
			return repository;
		}
	}
}
