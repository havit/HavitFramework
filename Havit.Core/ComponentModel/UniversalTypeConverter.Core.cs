using Havit.Diagnostics.Contracts;
using System.Globalization;

namespace Havit.ComponentModel;

public static partial class UniversalTypeConverter
{
	/// <summary>
	/// Converts the given value to the given type using the given CultureInfo.
	/// A return value indicates whether the operation succeeded.
	/// </summary>
	/// <param name="value">The value which is converted.</param>
	/// <param name="targetType">The type to which the given value is converted.</param>
	/// <param name="result">An Object instance of type <paramref name="targetType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
	/// <param name="culture">The CultureInfo to use as the current culture.</param>
	/// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
	public static bool TryConvertTo(object value, Type targetType, out object result, CultureInfo culture)
	{
		Contract.Requires<ArgumentNullException>(targetType != null, nameof(targetType));

		// Nullable - extract the encapsulated type and continue to use this type as targetType.
		// Nullable.GetUnderlyingType returns non-null only for Nullable<T> (avoids IsGenericType + GetGenericTypeDefinition allocations on the hot path).
		Type underlyingType = Nullable.GetUnderlyingType(targetType);
		bool nullableType = underlyingType != null;
		if (nullableType)
		{
			targetType = underlyingType;
		}

		if (value == null)
		{
			result = null;
			if (!targetType.IsValueType || nullableType /*(Activator.CreateInstance(targetType) == null) */)
			{
				return true;
			}
			return false;
		}

		if (targetType.IsInstanceOfType(value))
		{
			result = value;
			return true;
		}

		// DBNull.Value (a database null) is treated the same way as null (unless the target type can hold the DBNull.Value itself, which is handled by IsInstanceOfType above).
		if (value == DBNull.Value)
		{
			result = null;
			if (!targetType.IsValueType || nullableType)
			{
				return true;
			}
			return false;
		}

		// Numeric (or other IConvertible) value to enum conversion is not covered by IConvertible nor TypeConverters below.
		// String to enum conversion is intentionally left to the EnumConverter (TryConvertByDefaultTypeConverters).
		if (targetType.IsEnum && (value is IConvertible) && (value is not string))
		{
			try
			{
				result = Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType), culture));
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		object tmpResult = null;
		if (TryConvertByIConvertibleImplementation(value, targetType, culture, ref tmpResult))
		{
			result = tmpResult;
			return true;
		}

		if (TryConvertByDefaultTypeConverters(value, targetType, culture, ref tmpResult))
		{
			result = tmpResult;
			return true;
		}

		result = null;
		return false;
	}
}
