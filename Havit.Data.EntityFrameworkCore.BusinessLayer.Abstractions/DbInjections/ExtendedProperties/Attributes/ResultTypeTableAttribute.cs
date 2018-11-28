using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	/// <summary>
	/// Atribut pro nastavení extended property ResultTypeTable na uložené proceduře.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ResultTypeTableAttribute : DbInjectionExtendedPropertiesAttribute
	{
		/// <inheritdoc />
		public override string ObjectType { get; } = "PROCEDURE";

		/// <summary>
		/// Určuje typ výsledku, který se má z metody volající stored proceduru vrátit.
		/// </summary>
		public string ResultTypeTable { get; }

		/// <summary>
		/// Konštruktor.
		/// </summary>
		public ResultTypeTableAttribute(string resultTableType)
		{
			ResultTypeTable = resultTableType;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>
			{
				{ "ResultTypeTable", ResultTypeTable }
			};
		}
	}
}