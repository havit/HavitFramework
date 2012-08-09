using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	/// <summary>
	/// Pomocník pro práci s výètem ComparisonOperator.
	/// </summary>
	public static class ComparisonOperatorHelper
	{
		/// <summary>
		/// Pøevede comparison operátor na øetìzec, napø. Equals na "=", NotEquals na "&lt;&gt;", apod.
		/// </summary>
		public static string GetOperatorText(ComparisonOperator comparsionOperator)
		{
			switch (comparsionOperator)
			{
				case ComparisonOperator.Equals:
					return "=";
				case ComparisonOperator.NotEquals:
					return "<>";
				case ComparisonOperator.GreaterOrEquals:
					return ">=";
				case ComparisonOperator.Greater:
					return ">";
				case ComparisonOperator.Lower:
					return "<";
				case ComparisonOperator.LowerOrEquals:
					return "<=";
				default:
					throw new ArgumentException("Neznámá hodnota ComparisonOperator.", "comparsionOperator");
			}
		}

	}
}
