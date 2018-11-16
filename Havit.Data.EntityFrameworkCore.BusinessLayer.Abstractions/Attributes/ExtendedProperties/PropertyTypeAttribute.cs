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
	public class PropertyTypeAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Access modifier pro metodu Create Object.
		/// </summary>
		public string PropertyType { get; }
		
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public PropertyTypeAttribute(string propertyType)
		{
			PropertyType = propertyType;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>
			{
				{ "PropertyType", PropertyType }
			};
		}
	}
}
