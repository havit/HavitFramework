using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Havit.ComponentModel
{
	public static partial class UniversalTypeConverter
	{
		#region TryConvertByIConvertibleImplementation
		/// <summary>
		/// Pokusí se provést převedení typu pomocí IConvertible.
		/// Implementace převzata z UniversalTypeConverter.
		/// </summary>
		private static bool TryConvertByIConvertibleImplementation(object value, Type destinationType, IFormatProvider formatProvider, ref object result)
		{
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				try
				{
					if (destinationType == typeof(Boolean))
					{
						result = convertible.ToBoolean(formatProvider);
						return true;
					}
					if (destinationType == typeof(Byte))
					{
						result = convertible.ToByte(formatProvider);
						return true;
					}
					if (destinationType == typeof(Char))
					{
						result = convertible.ToChar(formatProvider);
						return true;
					}
					if (destinationType == typeof(DateTime))
					{
						result = convertible.ToDateTime(formatProvider);
						return true;
					}
					if (destinationType == typeof(Decimal))
					{
						result = convertible.ToDecimal(formatProvider);
						return true;
					}
					if (destinationType == typeof(Double))
					{
						result = convertible.ToDouble(formatProvider);
						return true;
					}
					if (destinationType == typeof(Int16))
					{
						result = convertible.ToInt16(formatProvider);
						return true;
					}
					if (destinationType == typeof(Int32))
					{
						result = convertible.ToInt32(formatProvider);
						return true;
					}
					if (destinationType == typeof(Int64))
					{
						result = convertible.ToInt64(formatProvider);
						return true;
					}
					if (destinationType == typeof(SByte))
					{
						result = convertible.ToSByte(formatProvider);
						return true;
					}
					if (destinationType == typeof(Single))
					{
						result = convertible.ToSingle(formatProvider);
						return true;
					}
					if (destinationType == typeof(UInt16))
					{
						result = convertible.ToUInt16(formatProvider);
						return true;
					}
					if (destinationType == typeof(UInt32))
					{
						result = convertible.ToUInt32(formatProvider);
						return true;
					}
					if (destinationType == typeof(UInt64))
					{
						result = convertible.ToUInt64(formatProvider);
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
			return false;
		}
		#endregion

		#region TryConvertByDefaultTypeConverters
		/// <summary>
		/// Pokusí se provést převedení typu pomocí TypeConverterů.
		/// Implementace převzata z UniversalTypeConverter.
		/// </summary>
		private static bool TryConvertByDefaultTypeConverters(object value, Type destinationType, CultureInfo culture, ref object result)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
			if (converter != null)
			{
				if (converter.CanConvertFrom(value.GetType()))
				{
					try
					{
						// ReSharper disable AssignNullToNotNullAttribute
						result = converter.ConvertFrom(null, culture, value);
						// ReSharper restore AssignNullToNotNullAttribute
						return true;
					}
					// ReSharper disable EmptyGeneralCatchClause
					catch
					{
						// ReSharper restore EmptyGeneralCatchClause
					}
				}
			}

			converter = TypeDescriptor.GetConverter(value);
			if (converter != null)
			{
				if (converter.CanConvertTo(destinationType))
				{
					try
					{
						result = converter.ConvertTo(null, culture, value, destinationType);
						return true;

					}
					// ReSharper disable EmptyGeneralCatchClause
					catch
					{
						// ReSharper restore EmptyGeneralCatchClause
					}
				}
			}
			return false;
		}
		#endregion

	}
}
