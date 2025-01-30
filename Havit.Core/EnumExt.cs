namespace Havit;

/// <summary>
/// Provides methods related to the basic enumeration type System.Enum.
/// </summary>
/// <remarks>
/// The class itself is not a descendant of System.Enum because System.Enum cannot be inherited.
/// </remarks>
public static class EnumExt
{
	/// <summary>
	/// Returns the value of the [Description("...")] attribute of a specific value of the specified enumeration type.
	/// </summary>
	/// <param name="enumType">Enumeration type</param>
	/// <param name="value">Value whose Description we want</param>
	/// <returns>Value of the [Description("...")] attribute</returns>
	/// <remarks>If the Description attribute is not defined, it returns an empty string.</remarks>
	/// <example>
	///	<code>
	/// using System.ComponentModel;<br/>
	/// <br/>
	/// public enum Colors<br/>
	/// {<br/>
	///		[Description("red")]<br/>
	///		Red,<br/>
	///	<br/>
	///		[Description("blue")]<br/>
	///		Blue<br/>
	///	}<br/>
	///	</code>
	/// </example>
	public static string GetDescription(Type enumType, object value)
	{
		string strRet = "";

		try
		{
			System.Reflection.FieldInfo objInfo =
				enumType.GetField(System.Enum.GetName(enumType, value));

			System.ComponentModel.DescriptionAttribute objDescription =
				(System.ComponentModel.DescriptionAttribute)objInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true)[0];

			strRet = objDescription.Description;
		}
		catch (Exception)
		{
			// description is missing
		}

		return strRet;
	}
}
