using Neudesic.Tools.CodeDebt.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Neudesic.Tools.CodeDebt
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Trace.WriteLine("Usage:");
				Trace.WriteLine("\tCodeDebtTool <codeBase> <projectPath1> <projectPath2> <projectPath3>...\r\n\r\n");
				Trace.WriteLine("\tWhere <codeBase> is a repository and <projectPath> is local project path.");
			}
			else
			{
				List<string> projectPaths = new List<string>();
				projectPaths.AddRange(args);	//Adds all incoming arguments to collection
				projectPaths.RemoveAt(0);		//Removes the codebase argument
				FindFilesForReportingCodeDebt(RepositoryFactory.Instance, args[0], projectPaths);
			}
#if DEBUG
			WaitForKeyPress();
#endif
		}
#if DEBUG
		private static void WaitForKeyPress()
		{
			if (Debugger.IsAttached)
			{
				Trace.WriteLine("Press any key to continue...");
				Console.ReadLine();
			}
		}
#endif
		private static void FindFilesForReportingCodeDebt(ICodeDebtRepository codeRepository, string codeBase, List<string> projectPaths)
		{
			//If codedebt has already been reported we can just return.
			if (codeRepository.IsCodeDebtAlreadyReported(codeBase))
				return;
			List<Task> parentTasks = new List<Task>();
			foreach (string projectPath in projectPaths)
			{
				parentTasks.Add(new TaskFactory().StartNew(() =>
					{
						Trace.WriteLine(string.Format("Running CodeDebtTool on all files beginning at root {0}.", projectPath));
						//WaitForKeyPress();
						Dictionary<string, string> ht = new Dictionary<string, string>();
						List<Task> childTasks = new List<Task>();
						foreach (FileTypeInfo ft in CodeDebtConfiguration.FileTypeInfos)
						{
							childTasks.Add(new TaskFactory().StartNew( () => 
							{
								try
								{
									string[] projectFiles = Directory.GetFiles(projectPath, ft.FileType, SearchOption.AllDirectories);
									foreach (string projectFile in projectFiles)
									{
										if (!ht.ContainsKey(projectFile))
										{
											ht.Add(projectFile, null);
											ReportCodeDebtInFile(codeRepository, codeBase, ft, projectFile);
										}
									}
								}
								catch (Exception exc)
								{
									Exception baseExc = exc.GetBaseException();
									Trace.WriteLine(string.Format("Exception={0}\r\nTargetSite={1}\r\n{2}",
										baseExc.Message, baseExc.TargetSite.Name, baseExc.StackTrace));
								}
							}));
						}
						Task.WaitAll(childTasks.ToArray());
						Trace.WriteLine(string.Format("Completed running CodeDebtTool on all files beginning at root {0}.\r\n", projectPath));
					}));
			}
			Task.WaitAll(parentTasks.ToArray());
		}

		private static void ReportCodeDebtInFile(ICodeDebtRepository codeRepository, string codeBase, FileTypeInfo ft, string projectFile)
		{
			string sourcePath = Path.GetDirectoryName(projectFile);
			string sourceFile = Path.GetFileName(projectFile);
			string[] lines = File.ReadAllLines(projectFile);

			List<DebtInfo> debtList = CodeDebtCollector.FindCodeDebt(codeBase, ft.CommentIdentifiers, lines, sourcePath, sourceFile);
			if (debtList.Count > 0)
				codeRepository.Save(codeBase, debtList);
		}

	}
}
