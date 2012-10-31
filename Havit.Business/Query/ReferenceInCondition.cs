using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query
{
	/// <summary>
	/// Reference condition IN - třída pro vnitřní použití frameworku, řeší podmínku IN a různé implementace na různých platformách.
	/// </summary>
	internal class ReferenceInCondition : Condition
	{
		#region Private field
		private IOperand operand;
		private int[] ids;
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor
		/// </summary>
		internal ReferenceInCondition(IOperand operand, int[] ids)
		{
			this.operand = operand;
			this.ids = ids;
		}
		#endregion

		#region IsEmptyCondition
		/// <summary>
		/// Udává, zda podmínka reprezentuje prázdnou podmínku, která nebude renderována (např. prázdná AndCondition).
		/// </summary>
		public override bool IsEmptyCondition()
		{
			return false;
		}
		#endregion IsEmptyCondition

		#region GetWhereStatement
		/// <summary>
		/// Přidá část SQL příkaz pro sekci WHERE. Je VELMI doporučeno, aby byla podmínka přidána včetně závorek.
		/// </summary>
		public override void GetWhereStatement(DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		{
			switch (sqlServerPlatform)
			{
				case SqlServerPlatform.SqlServerCe35:
					GetWhereStatementForSqlServerCe35(command, whereBuilder);
					return;

				case SqlServerPlatform.SqlServer2005:
					GetWhereStatementForSqlServer2005(command, whereBuilder);
					return;

				default:
					GetWhereStatementForSqlServer2008(command, whereBuilder);
					break;
			}
		}
		#endregion

		#region GetWhereStatementForSqlServerCe35
		/// <summary>
		/// Řeší variantu podmínky where pro SQL Server CE 3.5.
		/// </summary>
		private void GetWhereStatementForSqlServerCe35(DbCommand command, StringBuilder whereBuilder)
		{
			whereBuilder.AppendFormat("({{0}} IN ({0})", String.Join(",", Array.ConvertAll<int, string>(ids, item => item.ToString())));
		}
		#endregion

		#region GetWhereStatementForSqlServer2005
		/// <summary>
		/// Řeší variantu podmínky where pro SQL Server 2005.
		/// </summary>
		private void GetWhereStatementForSqlServer2005(DbCommand command, StringBuilder whereBuilder)
		{
			if (ids.Length < 2000)
			{
				IOperand idsOperand = SqlInt32ArrayOperand.Create(ids);
				whereBuilder.AppendFormat("({0} IN (SELECT [Value] FROM dbo.IntArrayToTable({1})))", operand.GetCommandValue(command), idsOperand.GetCommandValue(command));
			}
			else
			{
				bool wasFirst = false;
				int startIndex = 0;

				whereBuilder.Append("(");

				while (startIndex < ids.Length)
				{
					if (wasFirst)
					{
						whereBuilder.Append(" OR ");
					}
					wasFirst = true;

					int length = Math.Min(ids.Length - startIndex, 1999);
					int[] subarray = new int[length];
					Array.Copy(ids, startIndex, subarray, 0, length);

					IOperand idsOperand = SqlInt32ArrayOperand.Create(ids);
					whereBuilder.AppendFormat("({0} IN (SELECT [Value] FROM dbo.IntArrayToTable({1})))", operand.GetCommandValue(command), idsOperand.GetCommandValue(command));

					startIndex += length;
				}
				whereBuilder.Append(")");
				
			}
		}
		#endregion

		#region GetWhereStatementForSqlServer2008
		/// <summary>
		/// Řeší variantu podmínky where pro SQL Server 2008.
		/// </summary>
		private void GetWhereStatementForSqlServer2008(DbCommand command, StringBuilder whereBuilder)
		{
			IOperand idsOperand = IntArrayTableTypeOperand.Create(ids);
			whereBuilder.AppendFormat("({0} IN (SELECT [Value] FROM {1}))", operand.GetCommandValue(command), idsOperand.GetCommandValue(command));
		}
		#endregion

	}
}
