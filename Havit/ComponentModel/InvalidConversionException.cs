using System;

namespace Havit.ComponentModel
{

	/// <summary>
	/// The exception that is thrown when a conversion is invalid.
	/// </summary>
	internal class InvalidConversionException : InvalidOperationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConversionException">InvalidConversionException</see> class.
		/// </summary>
		public InvalidConversionException(object valueToConvert, Type destinationType)
			: base(String.Format(valueToConvert == null ? "Null is not convertible to type '{2}'." : "'{0}' ({1}) is not convertible to type '{2}'.",
								 valueToConvert == null ? "null" : valueToConvert.ToString(), // 0
								 valueToConvert == null ? "null" : valueToConvert.GetType().FullName, // 1
								 destinationType.FullName)) // 2
		{
		}
	}
}