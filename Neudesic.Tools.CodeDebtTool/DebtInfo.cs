using System.IO;

namespace Neudesic.Tools.CodeDebt
{
	public class DebtInfo
	{
		public DebtInfo(string debtType)
		{
			this.DebtType = debtType;
		}
		public string SourcePath { get; set; }
		public string SourceFile { get; set; }
		public int LineNumber { get; set; }
		public int LineCount { get; set; }
		public double Estimate { get; set; }
		public string DebtType { get; set; }
		public string Initials { get; set; }
		public char Criticality { get; set; }
		public string Comments { get; set; }
	}
}
