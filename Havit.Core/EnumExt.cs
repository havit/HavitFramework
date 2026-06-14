using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Havit;

/// <summary>
/// Provides methods related to the basic enumeration type System.Enum.
/// </summary>
/// <remarks>
/// The class itself is not a descendant of System.Enum because System.Enum cannot be inherited.
/// </remarks>
public static class EnumExt
{
	// Per enum type cache of "field name -> [Description] value" (only fields that have the attribute).
	// Avoids the reflection (GetField + GetCustomAttributes array allocation) on every call - GetDescription is typically called repeatedly when rendering enum value labels.
	private static readonly ConcurrentDictionary<Type, Dictionary<string, string>> descriptionCache = new ConcurrentDictionary<Type, Dictionary<string, string>>();

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
		string name = System.Enum.GetName(enumType, value);
		if (name == null)
		{
			// value is not defined in the enum
			return "";
		}

		Dictionary<string, string> descriptions = descriptionCache.GetOrAdd(enumType, BuildDescriptionMap);

		return descriptions.TryGetValue(name, out string description)
			? description
			: "";
	}

	private static Dictionary<string, string> BuildDescriptionMap(Type enumType)
	{
		var result = new Dictionary<string, string>();
		foreach (FieldInfo field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
		{
			var descriptionAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), true);
			if (descriptionAttributes.Length > 0)
			{
				result[field.Name] = descriptionAttributes[0].Description;
			}
		}
		return result;
	}
}
