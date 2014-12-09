using Neudesic.Tools.CodeDebt.Configuration;
using System.Collections.Generic;
using System.Configuration;

namespace Neudesic.Tools.CodeDebt
{
	public class FileTypeInfo
	{
		public string FileType { get; set; }
		public List<string> CommentIdentifiers { get; set; }
	}
	public static class CodeDebtConfiguration
	{
		private static CodeDebtSettingsSection ms_configurationSettings;
		static CodeDebtConfiguration()
		{
			ms_configurationSettings = (CodeDebtSettingsSection)ConfigurationManager.GetSection("codeDebtSection");
		}

		public static List<FileTypeInfo> FileTypeInfos
		{
			get
			{
				var fileTypeInfos = new List<FileTypeInfo>();
				foreach (FileTypeSettingElement element in ms_configurationSettings.FileTypesConfiguration)
				{
					FileTypeInfo fileTypeInfo = new FileTypeInfo();
					fileTypeInfo.FileType = element.FileType;
					fileTypeInfo.CommentIdentifiers = new List<string>();

					foreach (string commentIdentifier in element.CommentIdentifiers.Split(','))
					{
						fileTypeInfo.CommentIdentifiers.Add(commentIdentifier.Trim());
					}
					fileTypeInfos.Add(fileTypeInfo);
				}
				return fileTypeInfos;
			}
		}

        public static List<string> IgnorePaths
        {
            get
            {
                List<string> ignorePaths = new List<string>();
                foreach (IgnorePathSettingElement ignorePath in ms_configurationSettings.IgnorePathsConfiguration)
                {
                    ignorePaths.Add(ignorePath.IgnorePath);
                }
                return ignorePaths;
            }
        }

		public static List<string> DebtIdentifiers
		{
			get
			{
				List<string> debtIdentifiers = new List<string>();
				foreach (string debtKey in ms_configurationSettings.DebtKeys.Split(','))
				{
					debtIdentifiers.Add(debtKey.Trim());
				}
				return debtIdentifiers;
			}
		}
	}
}
