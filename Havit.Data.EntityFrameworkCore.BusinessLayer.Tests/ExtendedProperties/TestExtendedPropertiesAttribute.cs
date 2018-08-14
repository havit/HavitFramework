using System.Collections.Generic;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties
{
	internal class TestExtendedPropertiesAttribute : ExtendedPropertiesAttribute
	{
		private readonly IDictionary<string, string> _extendedProperties;

		private TestExtendedPropertiesAttribute()
		{
			_extendedProperties = new Dictionary<string, string>();
		}

		public TestExtendedPropertiesAttribute(string name, string value)
			: this()
		{
			_extendedProperties.Add(name, value);
		}

		public TestExtendedPropertiesAttribute(string name1, string value1, string name2, string value2)
			: this()
		{
			_extendedProperties.Add(name1, value1);
			_extendedProperties.Add(name2, value2);
		}

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => _extendedProperties;
	}
}
