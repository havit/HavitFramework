using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes;

/// <summary>
/// Atribút pre nastavenie Result extended property na uloženej procedúre.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ResultAttribute : ModelExtensionExtendedPropertiesAttribute
{
	/// <inheritdoc />
	public override string ObjectType { get; } = "PROCEDURE";

	/// <summary>
	/// Určuje typ výsledku, který se má z metody volající stored proceduru vrátit.
	/// </summary>
	public StoredProcedureResultType ResultType { get; }

	/// <summary>
	/// Konštruktor.
	/// </summary>
	public ResultAttribute(StoredProcedureResultType resultType)
	{
		ResultType = resultType;
	}

	/// <inheritdoc />
	public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
	{
		return new Dictionary<string, string>
		{
			{ "Result", ResultType.ToString() }
		};
	}
}