using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodAccessModifierAttribute : DbInjectionExtendedPropertiesAttribute
	{
		public override string ObjectType { get; } = "PROCEDURE";

		public string Modifier { get; }

		public MethodAccessModifierAttribute(string modifier)
		{
			Modifier = modifier;
		}

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>
			{
				{ "MethodAccessModifier", Modifier }
			};
		}
	}
}