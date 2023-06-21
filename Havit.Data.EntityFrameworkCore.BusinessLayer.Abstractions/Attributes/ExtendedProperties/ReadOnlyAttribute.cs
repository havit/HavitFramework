using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// ExtendedProperty pro označení business objektů jako read only.
/// </summary>
/// <remarks>
/// ReadOnly = true/false
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class ReadOnlyAttribute : ExtendedPropertiesAttribute
{
	private readonly bool value;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ReadOnlyAttribute() : this(true)
	{
		// NOOP
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ReadOnlyAttribute(bool value)
	{
		this.value = value;
	}

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
	{
		{ "ReadOnly", value.ToString().ToLower() }
	};
}