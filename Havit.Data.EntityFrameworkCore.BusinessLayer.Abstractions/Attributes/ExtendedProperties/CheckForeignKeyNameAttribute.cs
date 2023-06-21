using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// ExtendedProperty pro označení business objektů, aby nedocházelo ke kontrole konvencí pojmenování cizích klíčů (resp. resp. aby vlastnosti nekončily "ID" a nebyly cizím klíčem).
/// </summary>
/// <remarks>
/// CheckForeignKeyName = true/false
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class CheckForeignKeyNameAttribute : ExtendedPropertiesAttribute
{
	private readonly bool value;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CheckForeignKeyNameAttribute(bool value)
	{
		this.value = value;
	}

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
	{
		{ "CheckForeignKeyName", value.ToString().ToLower() }
	};
}
