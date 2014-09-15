using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neudesic.Tools.CodeDebt.Repositories
{
	public interface ICodeDebtRepository
	{
		string Name { get; set; }
		string ConnectionString { get; set; }
		bool IsCodeDebtAlreadyReported(string codeBase);
		void Save(string codeBase, List<DebtInfo> debtList);
	}
}
