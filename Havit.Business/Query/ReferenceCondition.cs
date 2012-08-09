using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínku testující referenèní hodnotu (cizí klíè).
	/// </summary>
	public static class ReferenceCondition
	{
		/// <summary>
		/// Vytvoøí podmínku na rovnost.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, int? id)
		{
			if (id <= 0)
			{
				throw new ArgumentException("ID objektu musí být kladné èíslo nebo null.", "id");
			}

			if (id == null || id < 0)
			{
				return NullCondition.CreateIsNull(operand);
			}
			else
			{
				return NumberCondition.CreateEquals(operand, id.Value);
			}
		}

		/// <summary>
		/// Vytvoøí podmínku na rovnost.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, BusinessObjectBase businessObject)
		{
			if (businessObject.IsNew)
			{
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");
			}

			return CreateEquals(operand, businessObject.ID);
		}

		/// <summary>
		/// Vytvoøí podmínku na rovnost dvou operandù.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return NumberCondition.CreateEquals(operand1, operand2);
		}

		/// <summary>
		/// Vytvoøí podmínku existence hodnoty v poli integerù.
		/// </summary>
		public static Condition CreateIn(IOperand operand, int[] ids)
		{
			return new BinaryCondition("{0} IN (SELECT Value FROM dbo.IntArrayToTable({1}))", operand, SqlInt32ArrayOperand.Create(ids));
		}
		
	}
}
