using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes;

/// <summary>
/// Atribut pro nastavení extended property Ignored na uložené proceduře.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class IgnoredAttribute : ModelExtensionExtendedPropertiesAttribute
{
	/// <inheritdoc />
	public override string ObjectType { get; } = "PROCEDURE";

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
	{
		return new Dictionary<string, string>
		{
			{ BusinessLayer.Attributes.ExtendedProperties.IgnoredAttribute.ExtendedPropertyName, "true" }
		};
	}
}