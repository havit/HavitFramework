using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// ExtendedProperty pro ignorování tabulky.
	/// </summary>
	/// <remarks>
	/// Ignored = true
	/// </remarks>
	public class IgnoredAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Název extended property.
		/// </summary>
		/// <remarks>Ignored.</remarks>
		public static readonly string ExtendedPropertyName = "Ignored";

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>()
		{
			{ ExtendedPropertyName, "true" },
		};
	}
}