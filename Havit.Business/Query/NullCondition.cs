using System;
using System.Collections.Generic;
using System.Text;

using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínky testující null hodnoty.
	/// </summary>
	public static class NullCondition
	{
		#region CreateIsNull
		/// <summary>
		/// Vytvoří podmínku testující hodnotu na NULL.
		/// </summary>
		public static Condition CreateIsNull(IOperand operand)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			return new UnaryCondition(UnaryCondition.IsNullPattern, operand);
		} 
		#endregion

		#region CreateIsNotNull
		/// <summary>
		/// Vytvoří podmínku testující hodnotu na NOT NULL.
		/// </summary>
		public static Condition CreateIsNotNull(IOperand operand)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			return new UnaryCondition(UnaryCondition.IsNotNullPattern, operand);
		} 
		#endregion
	}
}
