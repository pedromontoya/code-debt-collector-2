using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Neudesic.Tools.CodeDebt
{
	public static class CodeDebtCollector
	{
		const char DebtSeparator = ':';
		const string CLikeCommentStart = "/*";
		const string CLikeCommentEnd = "*/";

		/*TODO:CR:C:4 Look into using regular expressions to find comment identifiers
		 */
		public static List<DebtInfo> FindCodeDebt(string codeBase, List<string> commentIdentifiers, string[] lines, string sourcePath, string sourceFile)
		{
			//Trace.WriteLine(string.Format("Searching {0}", projectPath));
			List<DebtInfo> debtList = new List<DebtInfo>();
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();
				DebtInfo debtInfo = null;
				foreach (string commentIdentifier in commentIdentifiers)
				{
					debtInfo = FindCodeDebtEntry(commentIdentifier, line);
					if (debtInfo != null)
					{
						debtInfo.LineNumber = i + 1;	//Line number is relative to 1
						debtInfo.LineCount = lines.Length;
						debtInfo.SourcePath = sourcePath;
						debtInfo.SourceFile = sourceFile;
						debtList.Add(debtInfo);
						Trace.WriteLine(string.Format("\rSourcePath={0}\\{1}:{2}:{3}:{4}",
							sourcePath, sourceFile, debtInfo.LineNumber, debtInfo.DebtType, debtInfo.Comments));
						break;
					}
				}
			}
			return debtList;
		}

		private static DebtInfo FindCodeDebtEntry(string commentIdentifier, string line)
		{
			if (line.Length >= commentIdentifier.Length + 2)
			{
				if (line.Substring(0, commentIdentifier.Length) != commentIdentifier)
					return null;
				int pos = commentIdentifier.Length;
				///////////////////////////////////////////////////////////////////////////////////////////////////////
				//The correct format for codedebt line for C like langauages is:
				//		//<debttype>:<initials>:<debtpriority>:<hours to paydebt>:<comment>
				//	or
				//		/*<debttype>:<initials>:<debtpriority>:<hours to paydebt>:<comment>*/
				//
				//For VB the syntax is:
				//		'<debttype>:<initials>:<debtpriority>:<hours to paydebt>:<comment>
				//
				//Not certain we need to do this but this code will handle C like languages and VB where the line looks
				//like ////// TODO:CR:A:3 blah, blah, blah. For VB '''''' TODO:CR:A:3 blah, blah, blah.
				//This loop could be eliminated and the code will enforce the use of a single identifier followed
				//by the debt data.
				for (int i = pos; i < line.Length; i++)
				{
					if (line[i] != commentIdentifier[0])
						break;
					pos++;
				}
				///////////////////////////////////////////////////////////////////////////////////////////////////////
				line = line.Substring(pos).TrimStart();
				//Now look for codedebt identifiers and extract the codedebt data.
				foreach (string key in CodeDebtConfiguration.DebtIdentifiers)
				{
					if (line.Length >= key.Length)
					{
						if (key == line.Substring(0, key.Length))
						{
							DebtInfo debtInfo = GetCodeDebtData(line, key);
							if (commentIdentifier == CLikeCommentStart)
								if (debtInfo.Comments.Length >= CLikeCommentEnd.Length)
									if (debtInfo.Comments.Substring(debtInfo.Comments.Length - CLikeCommentEnd.Length) == CLikeCommentEnd)
										debtInfo.Comments = debtInfo.Comments.Remove(debtInfo.Comments.Length - CLikeCommentEnd.Length);
							return debtInfo;
						}
					}
				}
			}
			return null;
		}

		enum TokenType : int
		{
			Initials,
			Criticality,
			Estimate,
			Comments
		}
		enum Criticality
		{
			Critical = 'A',
			Important = 'B',
			NiceToHave = 'C'
		}

		private static DebtInfo GetCodeDebtData(string line, string debtType)
		{
			DebtInfo debtInfo = new DebtInfo(debtType);
			try
			{
				bool validToken = true;
				int pos = debtType.Length;
				for (int i = 0; i <= (int)TokenType.Comments; i++)
				{
					if (validToken)
					{
						//This will make certain that we don't exceed the line when there is a token but the rest is empty.
						int start = (line.Length >= pos + 1) ? 1 : 0;
						line = line.Substring(pos + start).TrimStart();
						//This is a kludge for the estimate. We will allow a space or colon delimiter just in case the developer left off
						//the final colon seperating the estimate from the comment.
						char[] delims = (i == (int)TokenType.Estimate) ? new char[] { DebtSeparator, ' ' } : new char[] { DebtSeparator };
						pos = line.IndexOfAny(delims);
					}
					//If the token was invalid, treat remainder of line as a comment.
					if (!validToken || pos < 0)
					{
						ExtractComments(line, line.Length, debtInfo);
						break;
					}
					validToken = ExtractToken(line, debtInfo, pos, (TokenType)i);
				}
			}
			catch(Exception exc)
			{
				debtInfo.Comments = line.TrimStart();
				Exception baseExc = exc.GetBaseException();
				Trace.WriteLine(string.Format("Exception={0}\r\nTargetSite={1}\r\n{2}",
					baseExc.Message, baseExc.TargetSite.Name, baseExc.StackTrace));
			}
			ValidateDebtInfo(debtInfo);
			return debtInfo;
		}

		private static bool ExtractToken(string line, DebtInfo debtInfo, int length, TokenType tokenType)
		{
			bool validToken;
			switch (tokenType)
			{
				case TokenType.Initials:
					validToken = ExtractInitials(line, length, debtInfo);
					break;
				case TokenType.Criticality:
					validToken = ExtractCriticality(line, length, debtInfo);
					break;
				case TokenType.Estimate:
					validToken = ExtractEstimate(line, length, debtInfo);
					break;
				case TokenType.Comments:
					validToken = ExtractComments(line, length, debtInfo);
					break;
				default:
					validToken = false;
					break;
			}
			return validToken;
		}

		private static void ValidateDebtInfo(DebtInfo debtInfo)
		{
			//If no criticality not found, default to type "A"
			if (debtInfo.Criticality == null || debtInfo.Criticality == 0)
				debtInfo.Criticality = (char)Criticality.Critical;
			if (debtInfo.Initials == null || debtInfo.Initials.Length > 5)
				debtInfo.Initials = string.Empty;
			if (debtInfo.Comments == null)
				debtInfo.Comments = string.Empty;
		}
		private static bool ExtractInitials(string line, int length, DebtInfo debtInfo)
		{
			bool validToken = length < 5;
			if (validToken)
				debtInfo.Initials = line.Substring(0, length).Trim();
			return validToken;
		}
		
		private static bool ExtractCriticality(string line, int length, DebtInfo debtInfo)
		{
			//Next token should be the debt criticality. The switch statement will verify that it is correct.
			string criticality = line.Substring(0, length).ToUpper().Trim();
			bool validToken = true;
			switch (criticality)
			{
				case "A":
				case "B":
				case "C":
					break;
				default:
					criticality = ((char)Criticality.Critical).ToString();
					validToken = false;
					break;
			}
			//Conversion should be safe since the default is to correct to critical.
			debtInfo.Criticality = Convert.ToChar(criticality);
			return validToken;
		}
		private static bool ExtractEstimate(string line, int length, DebtInfo debtInfo)
		{
			double estimate;
			bool validToken = double.TryParse(line.Substring(0, length).Trim(), out estimate);
			if (validToken)
				debtInfo.Estimate = estimate;
			return validToken;
		}
		private static bool ExtractComments(string line, int length, DebtInfo debtInfo)
		{
			//We extract whatever remains as the comment.
			int start = 0;
			for (int i = 0; i < line.Length; i++ )
			{
				if (line[i] == DebtSeparator)
					start++;
			}
			debtInfo.Comments = line.Substring(start).Trim();
			return true;
		}
	}
}
