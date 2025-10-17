using System.Globalization;

namespace Havit.ComponentModel;

/// <summary>
/// Data type converter.
/// Inspired by UniversalTypeConverter (http://www.codeproject.com/Articles/248440/Universal-Type-Converter).
/// Conversions using DefaultTypeConverter and IConvertible are taken.
/// </summary>
public static partial class UniversalTypeConverter
{
	/// <summary>
	/// Determines whether the given value can be converted to the specified type using the current CultureInfo.
	/// </summary>
	/// <typeparam name="T">The Type to test.</typeparam>
	/// <param name="value">The value to test.</param>
	/// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
	public static bool CanConvertTo<T>(object value)
	{
		return TryConvertTo(value, out T _, CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Determines whether the given value can be converted to the specified type using the given CultureInfo.
	/// </summary>
	/// <typeparam name="T">The Type to test.</typeparam>
	/// <param name="value">The value to test.</param>
	/// <param name="culture">The CultureInfo to use as the current culture.</param>
	/// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
	public static bool CanConvertTo<T>(object value, CultureInfo culture)
	{
		return TryConvertTo(value, out T _, culture);
	}

	/// <summary>
	/// Converts the given value to the given type using the current CultureInfo.
	/// A return value indicates whether the operation succeeded.
	/// </summary>
	/// <typeparam name="T">The type to which the given value is converted.</typeparam>
	/// <param name="value">The value which is converted.</param>
	/// <param name="result">An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
	/// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
	public static bool TryConvertTo<T>(object value, out T result)
	{
		return TryConvertTo(value, out result, CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Converts the given value to the given type using the given CultureInfo.
	/// A return value indicates whether the operation succeeded.
	/// </summary>
	/// <typeparam name="T">The type to which the given value is converted.</typeparam>
	/// <param name="value">The value which is converted.</param>
	/// <param name="result">An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
	/// <param name="culture">The CultureInfo to use as the current culture.</param>
	/// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
	public static bool TryConvertTo<T>(object value, out T result, CultureInfo culture)
	{
		var success = TryConvertTo(value, typeof(T), out object objectResult, culture);
		result = success ? (T)objectResult : default(T);
		return success;
	}

	/// <summary>
	/// Converts the given value to the given type using the current CultureInfo.
	/// </summary>
	/// <typeparam name="T">The type to which the given value is converted.</typeparam>
	/// <param name="value">The value which is converted.</param>
	/// <returns>An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
	public static T ConvertTo<T>(object value)
	{
		return ConvertTo<T>(value, CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Converts the given value to the given type using the given CultureInfo.
	/// </summary>
	/// <typeparam name="T">The type to which the given value is converted.</typeparam>
	/// <param name="value">The value which is converted.</param>
	/// <param name="culture">The CultureInfo to use as the current culture.</param>
	/// <returns>An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
	public static T ConvertTo<T>(object value, CultureInfo culture)
	{
		return (T)ConvertTo(value, typeof(T), culture);
	}

	/// <summary>
	/// Determines whether the given value can be converted to the specified type using the current CultureInfo.
	/// </summary>
	/// <param name="value">The value to test.</param>
	/// <param name="targetType">The Type to test.</param>
	/// <returns>true if <paramref name="value"/> can be converted to <paramref name="targetType"/>; otherwise, false.</returns>
	public static bool CanConvertTo(object value, Type targetType)
	{
		return TryConvertTo(value, targetType, out object _, CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Determines whether the given value can be converted to the specified type using the given CultureInfo.
	/// </summary>
	/// <param name="value">The value to test.</param>
	/// <param name="targetType">The Type to test.</param>
	/// <param name="culture">The CultureInfo to use as the current culture.</param>
	/// <returns>true if <paramref name="value"/> can be converted to <paramref name="targetType"/>; otherwise, false.</returns>
	public static bool CanConvertTo(object value, Type targetType, CultureInfo culture)
	{
		return TryConvertTo(value, targetType, out object _, culture);
	}

	/// <summary>
	/// Converts the given value to the given type using the current CultureInfo.
	/// A return value indicates whether the operation succeeded.
	/// </summary>
	/// <param name="value">The value which is converted.</param>
	/// <param name="targetType">The type to which the given value is converted.</param>
	/// <param name="result">An Object instance of type <paramref name="targetType">targetType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
	/// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
	public static bool TryConvertTo(object value, Type targetType, out object result)
	{
		return TryConvertTo(value, targetType, out result, CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Converts the given value to the given type using the current CultureInfo.
	/// </summary>
	/// <param name="value">The value which is converted.</param>
	/// <param name="targetType">The type to which the given value is converted.</param>
	/// <returns>An Object instance of type <paramref name="targetType">targetType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
	public static object ConvertTo(object value, Type targetType)
	{
		return ConvertTo(value, targetType, CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Converts the given value to the given type using the given CultureInfo.
	/// </summary>
	/// <param name="value">The value which is converted.</param>
	/// <param name="targetType">The type to which the given value is converted.</param>
	/// <param name="culture">The CultureInfo to use as the current culture.</param>
	/// <returns>An Object instance of type <paramref name="targetType">targetType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
	public static object ConvertTo(object value, Type targetType, CultureInfo culture)
	{
		object result;
		if (TryConvertTo(value, targetType, out result, culture))
		{
			return result;
		}
		throw new InvalidConversionException(value, targetType);
	}
}
