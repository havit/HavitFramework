using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// ExtendedProperty pro "EnumClass".
/// </summary>
/// <remarks>
/// EnumMode = EnumClass<br/>
/// eventuelně též EnumPropertyNameField
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class EnumClassAttribute : ExtendedPropertiesAttribute
{
	private readonly Dictionary<string, string> props = new Dictionary<string, string>
	{
		{ "EnumMode", "EnumClass" }
	};

	/// <summary>
	/// Název sloupce, ve které je název vlastnosti k vygenerování (jinak PropertyName/Symbol).
	/// </summary>
	public string EnumPropertyName { get; }

	/// <summary>
	/// Kontruktor.
	/// </summary>
	public EnumClassAttribute()
	{
	}

	/// <summary>
	/// Kontruktor.
	/// </summary>
	public EnumClassAttribute(string enumPropertyName)
	{
		EnumPropertyName = enumPropertyName;
		props.Add("EnumPropertyNameField", enumPropertyName);
	}

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => props;
}