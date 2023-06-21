using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// Určuje výchozí hodnotu vlastnosti. Je uveden kód, který se kompiluje. Obvykle tedy například Nastaveni.Current.NejakaVychoziHodnota. Pokud by to měl být string, musí být s uvozovkami, atd.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BusinessLayerDefaultValueAttribute : ExtendedPropertiesAttribute
{
	/// <summary>
	/// Výchozí hodnota vlastnosti.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public BusinessLayerDefaultValueAttribute(string value)
	{
		Value = value;
	}

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
	{
		{ "DefaultValue", Value }
	};
}