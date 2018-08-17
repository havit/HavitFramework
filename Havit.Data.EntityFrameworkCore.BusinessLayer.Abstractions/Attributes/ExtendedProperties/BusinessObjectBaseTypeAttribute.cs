using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	public class BusinessObjectBaseTypeAttribute : ExtendedPropertiesAttribute
	{
		public string TypeName { get; }

		public BusinessObjectBaseTypeAttribute(string typeName)
		{
			TypeName = typeName;
		}

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
		{
			{ "BusinessObjectBaseType", TypeName }
		};
	}
}