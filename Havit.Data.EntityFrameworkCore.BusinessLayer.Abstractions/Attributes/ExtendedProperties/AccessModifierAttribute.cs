using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// ExtendedProperty pro "PropertyType".
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class AccessModifierAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Access modifier pro vlastnost.
		/// </summary>
		public string PropertyAccessModifier { get; set; }

		/// <summary>
		/// Access modifier pro getter vlastnosti.
		/// </summary>
		public string GetAccessModifier { get; set; }

		/// <summary>
		/// Access modifier pro setter vlastnosti.
		/// </summary>
		public string SetAccessModifier { get; set; }

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			var result = new Dictionary<string, string>();

			if (!String.IsNullOrEmpty(PropertyAccessModifier))
			{ 
				result.Add("PropertyAccessModifier", PropertyAccessModifier);
			}

			if (!String.IsNullOrEmpty(GetAccessModifier))
			{
				result.Add("GetAccessModifier", GetAccessModifier);
			}

			if (!String.IsNullOrEmpty(SetAccessModifier))
			{
				result.Add("SetAccessModifier", SetAccessModifier);
			}

			return result;
		}
	}
}
