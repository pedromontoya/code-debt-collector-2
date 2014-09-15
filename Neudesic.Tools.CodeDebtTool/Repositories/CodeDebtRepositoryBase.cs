
namespace Neudesic.Tools.CodeDebt.Repositories
{
	public abstract class CodeDebtRepositoryBase : ICodeDebtRepository
	{
		public string ConnectionString { get; set; }
		public string Name { get; set; }
		public abstract bool IsCodeDebtAlreadyReported(string codeBase);
		public abstract void Save(string codeBase, System.Collections.Generic.List<DebtInfo> debtList);
	}
}
