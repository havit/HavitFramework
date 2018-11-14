using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// ExtendedProperty pro "CreateObjectAccessModifier".
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CreateObjectAccessModifierAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Access modifier pro metodu Create Object.
		/// </summary>
		public string AccessModifier { get; }
		
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CreateObjectAccessModifierAttribute(string accessModifier)
		{
			AccessModifier = accessModifier;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>
			{
				{ "CreateObjectAccessModifier", AccessModifier }
			};
		}
	}
}
