using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Havit.ComponentModel
{
	/// <summary>
	/// Convertor datových typů.
	/// Inspirováno UniversalTypeConverterem (http://www.codeproject.com/Articles/248440/Universal-Type-Converter).
	/// Převzaty jsou konverze pomocí DefaultTypeConvertoru a IConvertible.
	/// </summary>
	public static partial class UniversalTypeConverter
	{
		#region CanConvertTo<T>
		/// <summary>
		/// Determines whether the given value can be converted to the specified type using the current CultureInfo.
		/// </summary>
		/// <typeparam name="T">The Type to test.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
		public static bool CanConvertTo<T>(object value)
		{
			T result;
			return TryConvertTo(value, out result, CultureInfo.CurrentCulture);
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
			T result;
			return TryConvertTo(value, out result, culture);
		}
		#endregion

		#region TryConvertTo<T>
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
			return TryConvertTo(value, out result, culture);
		}
		#endregion

		#region ConvertTo<T>
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
			return ConvertTo<T>(value, culture);
		}
		#endregion

		#region CanConvertTo
		/// <summary>
		/// Determines whether the given value can be converted to the specified type using the current CultureInfo.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="targetType">The Type to test.</param>
		/// <returns>true if <paramref name="value"/> can be converted to <paramref name="targetType"/>; otherwise, false.</returns>
		public static bool CanConvertTo(object value, Type targetType)
		{
			object result;
			return TryConvertTo(value, targetType, out result, CultureInfo.CurrentCulture);
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
			object result;
			return TryConvertTo(value, targetType, out result, culture);
		}
		#endregion

		#region TryConvertTo
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
		#endregion

		#region ConvertTo
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
		#endregion	
	}
}
