using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EnumClassAttribute : ExtendedPropertiesAttribute
	{
		private readonly Dictionary<string, string> props = new Dictionary<string, string>
		{
			{ "EnumMode", "EnumClass" }
		};

		public string EnumPropertyName { get; }

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => props;

		public EnumClassAttribute()
		{
		}

		public EnumClassAttribute(string enumPropertyName)
		{
			EnumPropertyName = enumPropertyName;
			props.Add("EnumPropertyNameField", enumPropertyName);
		}
	}
}