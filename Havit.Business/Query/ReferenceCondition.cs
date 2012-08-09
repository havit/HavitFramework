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
		#region CreateEquals
		/// <summary>
		/// Vytvoøí podmínku na rovnost reference.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, int? id)
		{

			if (id == BusinessObjectBase.NoID)
			{
				throw new ArgumentException("ID objektu nesmí mít hodnotu BusinessObjectBase.NoID.", "id");
			}

			if (id == null)
			{
				return NullCondition.CreateIsNull(operand);
			}
			else
			{
				return NumberCondition.CreateEquals(operand, id.Value);
			}
		}

		/// <summary>
		/// Vytvoøí podmínku na rovnost reference.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, BusinessObjectBase businessObject)
		{
			if (businessObject == null)
			{
				return CreateEquals(operand, (int?)null);
			}

			if (businessObject.IsNew)
			{
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");
			}

			return CreateEquals(operand, businessObject.ID);
		}

		/// <summary>
		/// Vytvoøí podmínku na rovnost reference.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return NumberCondition.CreateEquals(operand1, operand2);
		}
		
		#endregion

		#region CreateNotEquals
		/// <summary>
		/// Vytvoøí podmínku na nerovnost reference.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand, int? id)
		{

			if (id == BusinessObjectBase.NoID)
			{
				throw new ArgumentException("ID objektu nesmí mít hodnotu BusinessObjectBase.NoID.", "id");
			}

			if (id == null)
			{
				return NullCondition.CreateIsNotNull(operand);
			}
			else
			{
				return NumberCondition.Create(operand, ComparisonOperator.NotEquals, id.Value);
			}
		}

		/// <summary>
		/// Vytvoøí podmínku na nerovnost reference.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand, BusinessObjectBase businessObject)
		{
			if (businessObject == null)
			{
				return CreateNotEquals(operand, (int?)null);
			}

			if (businessObject.IsNew)
			{
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");
			}

			return CreateNotEquals(operand, businessObject.ID);
		}

		/// <summary>
		/// Vytvoøí podmínku na nerovnost reference.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand1, IOperand operand2)
		{
			return NumberCondition.Create(operand1, ComparisonOperator.NotEquals, operand2);
		}
		
		#endregion

		#region CreateIn
		/// <summary>
		/// Vytvoøí podmínku existence reference v poli ID objektù.
		/// </summary>
		public static Condition CreateIn(IOperand operand, int[] ids)
		{
			return new BinaryCondition("{0} IN (SELECT Value FROM dbo.IntArrayToTable({1}))", operand, SqlInt32ArrayOperand.Create(ids));
		}
		#endregion

		#region CreateNotIn
		/// <summary>
		/// Vytvoøí podmínku neexistence hodnoty v poli ID objektù.
		/// </summary>
		public static Condition CreateNotIn(IOperand operand, int[] ids)
		{
			return new BinaryCondition("{0} NOT IN (SELECT Value FROM dbo.IntArrayToTable({1}))", operand, SqlInt32ArrayOperand.Create(ids));
		}
		#endregion
	}
}
