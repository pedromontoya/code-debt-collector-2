using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace Neudesic.Tools.CodeDebt.Repositories
{
	public class SqlServerCodeDebtRepository : CodeDebtRepositoryBase
	{
		//TODO:CR:A:2 Rewrite to use Entity Framework
		public override bool IsCodeDebtAlreadyReported(string codeBase)
		{
			using (SqlConnection connection = new SqlConnection(this.ConnectionString))
			using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 LastUpdatedDate"
													+ " FROM CodeDebt"
													+ " WHERE CodeBase = @CodeBase AND LastUpdatedDate > @LastUpdatedDate"
													+ " ORDER BY LastUpdatedDate DESC", connection))
			{
				connection.Open();
				DateTime dt = DateTime.Now.Subtract(new TimeSpan(23, 0, 0));
				cmd.Parameters.Add(new SqlParameter("@LastUpdatedDate", dt));
				cmd.Parameters.Add(new SqlParameter("@CodeBase", codeBase));
				using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					//If this is true it means that we found a record that is < 23 hours old
					return reader.Read();
				}
			}
		}

		public override void Save(string codeBase, List<DebtInfo> debtList)
		{
			if (debtList.Count == 0)
				return;
			using(TransactionScope ts = new TransactionScope())
			using (SqlConnection connection = new SqlConnection(this.ConnectionString))
			{
				connection.Open();
				foreach (DebtInfo info in debtList)
				{
					UpdateDatabase(connection, codeBase, info);
				}
				ts.Complete();
			}
		}

		//TODO:CR:B:2 Rewrite to use Entity Framework
		private static void UpdateDatabase(SqlConnection connection, string codeBase, DebtInfo info)
		{
			string sql = "INSERT INTO CodeDebt (CodeBase, SourcePath, SourceFile, Type, Initials, Criticality, Estimate, Comment, LineNumber, LineCount)"
									+ " VALUES (@CodeBase, @SourcePath, @SourceFile, @Type, @Initials, @Criticality, @Estimate, @Comment, @LineNumber, @LineCount)";
			using (SqlCommand cmd = new SqlCommand(sql, connection))
			{
				cmd.Parameters.Add(new SqlParameter("@CodeBase", codeBase));
				cmd.Parameters.Add(new SqlParameter("@SourcePath", info.SourcePath));
				cmd.Parameters.Add(new SqlParameter("@SourceFile", info.SourceFile));
				cmd.Parameters.Add(new SqlParameter("@Type", info.DebtType));
				cmd.Parameters.Add(new SqlParameter("@Initials", info.Initials));
				cmd.Parameters.Add(new SqlParameter("@Criticality", info.Criticality));
				cmd.Parameters.Add(new SqlParameter("@Estimate", info.Estimate));
				cmd.Parameters.Add(new SqlParameter("@Comment", info.Comments));
				cmd.Parameters.Add(new SqlParameter("@LineNumber", info.LineNumber));
				cmd.Parameters.Add(new SqlParameter("@LineCount", info.LineCount));

				cmd.CommandType = CommandType.Text;
				cmd.ExecuteNonQuery();
			}
		}


	}
}
