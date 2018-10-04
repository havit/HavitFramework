using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.PayPal.TypeConverters
{
	/// <summary>
	/// Základní type-converter k PayPalCurrency, který je připraven k použití v BL
	/// pro ukládání property typu <see cref="PayPalCurrency"/> pomocí její hodnoty <see cref="PayPalCurrency.NumericCode"/>.
	/// </summary>
	public class PayPalCurrencyTypeConverter : ITypeConverter<PayPalCurrency> 	
	{
		#region CanConvertTo, CanConvertFrom
		/// <summary>
		/// Returns whether this converter can convert the object to the specified type.
		/// </summary>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
		/// <returns>
		/// 	<c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.
		/// </returns>
		public bool CanConvertTo(Type destinationType)
		{
			if (destinationType == typeof(Int32))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter.
		/// </summary>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
		/// <returns>
		/// 	<c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.
		/// </returns>
		public bool CanConvertFrom(Type sourceType)
		{
			if (sourceType == typeof(Int32))
			{
				return true;
			}
			return false;
		}
		#endregion

		#region ConvertFrom
		/// <summary>
		/// Converts the given value to the type of this converter.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <returns>
		/// An Object that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
		public PayPalCurrency ConvertFrom(object value)
		{
			if (value == null)
			{
				return null;
			}

			if (!(value is Int32))
			{
				throw new NotSupportedException(String.Format("Konverze z typu {0} není podporována.", value.GetType().FullName));
			}

			PayPalCurrency result = PayPalCurrency.FindByNumericCode((int)value);
			if (result == null)
			{
				throw new ArgumentException("Hodnotu nelze konvertovat, příslušná měna nebyla nalezena.", "value");
			}

			return result;
		}
		#endregion

		#region ConvertTo
		/// <summary>
		/// Converts the given value object to the specified type, using the arguments.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <param name="destinationType">The <see cref="T:System.Type"/> to convert the value parameter to.</param>
		/// <returns>
		/// An Object that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The destinationType parameter is a null reference.</exception>
		public object ConvertTo(PayPalCurrency value, Type destinationType)
		{
			if ((destinationType != typeof(Int32))
				&& (destinationType != typeof(Int32?)))
			{
				throw new NotSupportedException(String.Format("Konverze do typu {0} není podporována.", destinationType.FullName));
			}

			if (value == null)
			{
				if (destinationType == typeof(Int32?))
				{
					return null;
				}
				throw new NotSupportedException(String.Format("Hodnotu null nelze na typ {0} převést.", destinationType.FullName));
			}

			return value.NumericCode;
		}
		#endregion
	}
}
