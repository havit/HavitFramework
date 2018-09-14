using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	/// <summary>
	/// Atribút pre nastavenie MethodAccessModifier extended property na uloženej procedúre.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodAccessModifierAttribute : DbInjectionExtendedPropertiesAttribute
	{
		/// <inheritdoc />
		public override string ObjectType { get; } = "PROCEDURE";

		/// <summary>
		/// Modifikátor prístupu pre metódu vygenerovnú BusinessLayerGeneratorom, ktorá zapúzdruje prístup k uloženej procedúre.
		/// </summary>
		public string Modifier { get; }

		/// <summary>
		/// Konštruktor.
		/// </summary>
		public MethodAccessModifierAttribute(string modifier)
		{
			Modifier = modifier;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
		{
			{ "MethodAccessModifier", Modifier }
		};
	}
}