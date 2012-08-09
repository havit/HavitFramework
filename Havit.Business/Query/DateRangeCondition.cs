using System;
using System.Collections.Generic;
using System.Text;


namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující rozsah datumù.
	/// </summary>
	public static class DateRangeCondition
	{
		/// <summary>
		/// Vytvoøí podmínku testující, zda je datum v intervalu datumù.
		/// </summary>
		public static Condition Create(IOperand operand, DateTime date1, DateTime date2)
		{
			return new TernaryCondition("({0} >= {1} and {0} < {2})", operand, ValueOperand.Create(date1), ValueOperand.Create(date2));
		}

		/// <summary>
		/// Vytvoøí podmínku testující, zda je den data (datumu) v intervalu dnù datumù.
		/// Zajišuje, aby hodnota operandu byla vìtší nebo rovna datu date1 a aby byla menší ne pùlnoc konce date2.
		/// Jinımi slovy: Argumenty moho obsahovat datum a èas, ale testuje se jen datum bez èasu. Potom 
		/// je zajišováno: DATUM(date1) &lt;= DATUM(operand) &lt; DATUM(date2).
		/// </summary>
		public static Condition CreateDays(IOperand operand, DateTime date1, DateTime date2)
		{
			return Create(operand, date1.Date, date2.Date.AddDays(1));
		}
	}
}
