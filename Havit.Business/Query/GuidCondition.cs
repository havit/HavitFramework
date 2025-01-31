using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query;

/// <summary>
/// Vytváří podmínky testující GUID hodnoty.
/// </summary>
public static class GuidCondition
{
	/// <summary>
	/// Vytvoří podmínku testující rovnost hodnoty.
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
	/// Vytvoří podmínku testující rovnost hodnoty operandů.
	/// </summary>
	public static Condition CreateEquals(IOperand operand1, IOperand operand2)
	{
		return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
	}

	/// <summary>
	/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
	/// </summary>
	public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, Guid value)
	{
		Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

		return Create(operand, comparisonOperator, ValueOperand.Create(value));
	}

	/// <summary>
	/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
	/// </summary>
	public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
	{
		Contract.Requires<ArgumentNullException>(operand1 != null, nameof(operand1));
		Contract.Requires<ArgumentNullException>(operand2 != null, nameof(operand2));

		return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
	}
}
