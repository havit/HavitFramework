using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

		bool nullableType = false;
		// Nullable - extract the encapsulated type and continue to use this type as targetType
		if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition())
		{
			nullableType = true;
			targetType = Nullable.GetUnderlyingType(targetType);
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
