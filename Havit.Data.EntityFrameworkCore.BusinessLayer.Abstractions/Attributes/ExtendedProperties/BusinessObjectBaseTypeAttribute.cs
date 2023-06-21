using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// Extended Property pro BusinessObjectBaseType.
/// 
/// Určuje bázovou třídu pro generovaný kód business objektu.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BusinessObjectBaseTypeAttribute : ExtendedPropertiesAttribute
{
	/// <summary>
	/// Typ pro bázovou třídu generované třídy business objektu.
	/// </summary>
	public string TypeName { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public BusinessObjectBaseTypeAttribute(string typeName)
	{
		TypeName = typeName;
	}

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
	{
		{ "BusinessObjectBaseType", TypeName }
	};
}