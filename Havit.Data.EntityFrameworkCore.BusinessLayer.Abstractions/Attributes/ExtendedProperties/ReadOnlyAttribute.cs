using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// ExtendedProperty pro označení business objektů jako read only.
	/// </summary>
	/// <remarks>
	/// ReadOnly = true
	/// </remarks>
	public class ReadOnlyAttribute : ExtendedPropertiesAttribute
	{
		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
		{
			{ "ReadOnly", "true" }
		};
	}
}