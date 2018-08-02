using System.Collections.Generic;
using System.Reflection;
using Havit.Business.CodeMigrations.ExtendedProperties;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	internal class CollectionTestExtendedPropertiesAttribute : ExtendedPropertiesAttribute
	{
		public string FooBar { get; set; }

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>()
			{
				{ $"Test_{memberInfo.Name}_{nameof(FooBar)}", FooBar },
			};
		}
	}
}
