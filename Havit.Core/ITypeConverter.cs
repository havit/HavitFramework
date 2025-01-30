namespace Havit;

/// <summary>
/// Interface for type conversion.
/// </summary>
/// <remarks>
/// Used, for example, in BL for conversion between DB type and BL type property.
/// </remarks>
/// <typeparam name="T">The type for which the type-converter is intended.</typeparam>
public interface ITypeConverter<T>
{
	/// <summary>
	/// Returns whether this converter can convert an object of the given type to the type of this converter.
	/// </summary>
	/// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
	/// <returns><c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.</returns>
	bool CanConvertFrom(Type sourceType);

	/// <summary>
	/// Returns whether this converter can convert the object to the specified type. 
	/// </summary>
	/// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
	/// <returns><c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.</returns>
	bool CanConvertTo(Type destinationType);

	/// <summary>
	/// Converts the given value object to the specified type, using the arguments. 
	/// </summary>
	/// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
	/// <exception cref="ArgumentNullException">The destinationType parameter is a null reference.</exception>
	/// <param name="value">The <see cref="Object"/> to convert.</param>
	/// <param name="destinationType">The <see cref="Type"/> to convert the value parameter to.</param>
	/// <returns>An Object that represents the converted value.</returns>
	object ConvertTo(T value, Type destinationType);

	/// <summary>
	/// Converts the given value to the type of this converter. 
	/// </summary>
	/// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
	/// <param name="value">The <see cref="Object"/> to convert.</param>
	/// <returns>An Object that represents the converted value.</returns>
	T ConvertFrom(object value);
}
