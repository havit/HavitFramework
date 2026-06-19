using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;

namespace Havit.ComponentModel;

public static partial class UniversalTypeConverter
{
	// TypeDescriptor.GetConverter is expensive (walks attributes and the TypeDescriptionProvider chain); cache the converter per type.
	private static readonly ConcurrentDictionary<Type, TypeConverter> typeConverterCache = new ConcurrentDictionary<Type, TypeConverter>();

	private static TypeConverter GetCachedConverter(Type type)
	{
		return typeConverterCache.GetOrAdd(type, static t => TypeDescriptor.GetConverter(t));
	}

	/// <summary>
	/// Attempts to perform type conversion using IConvertible.
	/// Implementation taken from UniversalTypeConverter.
	/// </summary>
	private static bool TryConvertByIConvertibleImplementation(object value, Type destinationType, IFormatProvider formatProvider, ref object result)
	{
		// Fast path for strings: use Try* parsing instead of throwing+catching exceptions on failure (a thrown+caught exception is very expensive).
		// Covers exactly the destination types the IConvertible branch below handles, with matching parsing semantics (NumberStyles/DateTimeStyles as used by Convert.ToXxx).
		if (value is string stringValue)
		{
			return TryConvertStringByParse(stringValue, destinationType, formatProvider, ref result);
		}

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

	/// <summary>
	/// Attempts to convert a string value to one of the value types otherwise handled by the IConvertible branch,
	/// using Try* parsing (no exceptions on failure). Parsing semantics match Convert.ToXxx(string, IFormatProvider).
	/// Returns false for destination types not handled here, so the caller can fall through to the TypeConverter path.
	/// </summary>
	private static bool TryConvertStringByParse(string value, Type destinationType, IFormatProvider formatProvider, ref object result)
	{
		if (destinationType == typeof(Boolean))
		{
			if (Boolean.TryParse(value, out bool parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Byte))
		{
			if (Byte.TryParse(value, NumberStyles.Integer, formatProvider, out byte parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(SByte))
		{
			if (SByte.TryParse(value, NumberStyles.Integer, formatProvider, out sbyte parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Int16))
		{
			if (Int16.TryParse(value, NumberStyles.Integer, formatProvider, out short parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(UInt16))
		{
			if (UInt16.TryParse(value, NumberStyles.Integer, formatProvider, out ushort parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Int32))
		{
			if (Int32.TryParse(value, NumberStyles.Integer, formatProvider, out int parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(UInt32))
		{
			if (UInt32.TryParse(value, NumberStyles.Integer, formatProvider, out uint parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Int64))
		{
			if (Int64.TryParse(value, NumberStyles.Integer, formatProvider, out long parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(UInt64))
		{
			if (UInt64.TryParse(value, NumberStyles.Integer, formatProvider, out ulong parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Single))
		{
			if (Single.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, formatProvider, out float parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Double))
		{
			if (Double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, formatProvider, out double parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Decimal))
		{
			if (Decimal.TryParse(value, NumberStyles.Number, formatProvider, out decimal parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(DateTime))
		{
			if (DateTime.TryParse(value, formatProvider, DateTimeStyles.AllowWhiteSpaces, out DateTime parsed)) { result = parsed; return true; }
			return false;
		}
		if (destinationType == typeof(Char))
		{
			if (value.Length == 1) { result = value[0]; return true; }
			return false;
		}

		// not a type handled by the IConvertible branch - let the caller try the TypeConverter path (string -> Guid, enum, custom types, ...)
		return false;
	}

	/// <summary>
	/// Attempts to perform type conversion using TypeConverters.
	/// Implementation taken from UniversalTypeConverter.
	/// </summary>
	private static bool TryConvertByDefaultTypeConverters(object value, Type destinationType, CultureInfo culture, ref object result)
	{
		TypeConverter converter = GetCachedConverter(destinationType);
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

		// ICustomTypeDescriptor instances must not be cached by type (the converter may be instance-specific).
		converter = (value is ICustomTypeDescriptor)
			? TypeDescriptor.GetConverter(value)
			: GetCachedConverter(value.GetType());
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
}
