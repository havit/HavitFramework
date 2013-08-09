using System;
using System.Collections.Generic;
using System.Text;

using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínky testující rozsah datumů.
	/// </summary>
	public static class DateRangeCondition
	{
		#region Create
		/// <summary>
		/// Vytvoří podmínku testující, zda je datum v intervalu datumů.
		/// </summary>
		public static Condition Create(IOperand operand, DateTime? date1, DateTime? date2)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			if ((date1 == null) && (date2 == null))
			{
				return EmptyCondition.Create();
			}

			if ((date1 != null) && (date2 != null))
			{
				return new TernaryCondition("({0} >= {1} and {0} < {2})", operand, ValueOperand.Create(date1.Value), ValueOperand.Create(date2.Value));
			}

			if (date1 != null)
			{
				return DateCondition.Create(operand, ComparisonOperator.GreaterOrEquals, date1.Value);
			}

			//if (date2 != null)
			//{
			return DateCondition.Create(operand, ComparisonOperator.Lower, date2.Value);
			//}

		} 
		#endregion

		#region CreateDays
		/// <summary>
		/// Vytvoří podmínku testující, zda je den data (datumu) v intervalu dnů datumů.
		/// Zajišťuje, aby hodnota operandu byla větší nebo rovna datu date1 a aby byla menší než půlnoc konce date2.
		/// Jinými slovy: Argumenty moho obsahovat datum a čas, ale testuje se jen datum bez času. Potom 
		/// je zajišťováno: DATUM(date1) &lt;= DATUM(operand) &lt;= DATUM(date2).
		/// </summary>
		public static Condition CreateDays(IOperand operand, DateTime? date1, DateTime? date2)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return Create(operand,
				date1 == null ? null : (DateTime?)date1.Value.Date,
				date2 == null ? null : (DateTime?)date2.Value.Date.AddDays(1));
		} 
		#endregion
	}
}
