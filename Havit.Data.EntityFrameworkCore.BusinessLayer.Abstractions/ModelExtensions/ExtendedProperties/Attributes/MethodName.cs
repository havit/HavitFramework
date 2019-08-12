using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes
{
	/// <summary>
	/// Atribut pro nastavení extended property MethodName na uložené proceduře.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodNameAttribute : ModelExtensionExtendedPropertiesAttribute
	{
		/// <inheritdoc />
		public override string ObjectType { get; } = "PROCEDURE";

		/// <summary>
		/// Určuje typ výsledku, který se má z metody volající stored proceduru vrátit.
		/// </summary>
		public string MethodName { get; }

		/// <summary>
		/// Konštruktor.
		/// </summary>
		public MethodNameAttribute(string methodName)
		{
			MethodName = methodName;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>
			{
				{ "MethodName", MethodName }
			};
		}
	}
}