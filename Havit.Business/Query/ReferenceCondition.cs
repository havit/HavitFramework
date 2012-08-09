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
		
	}
}
