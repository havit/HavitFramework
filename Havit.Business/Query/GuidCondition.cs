using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytv��� podm�nky testuj�c� GUID hodnoty.
	/// </summary>
	public class GuidCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvo�� podm�nku testuj�c� rovnost hodnoty.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, Guid? value)
		{
			if (value == null)
			{
				return NullCondition.CreateIsNull(operand);
			}
			return CreateEquals(operand, ValueOperand.Create(value.Value));
		}

		/// <summary>
		/// Vytvo�� podm�nku testuj�c� rovnost hodnoty operand�.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
		} 
		#endregion

		#region Create
		/// <summary>
		/// Vytvo�� podm�nku testuj�c� hodnoty pomoc� zadan�ho oper�toru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, Guid value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvo�� podm�nku testuj�c� hodnoty pomoc� zadan�ho oper�toru.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		} 
		#endregion

	}
}
