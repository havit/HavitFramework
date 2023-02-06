using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// Určuje celý název typu, který bude mít property vygenerovaná na základě sloupce v tabulce.
	/// Určeno zejména pro výčtové typy, které nemají podobu v databázi (nemají referenci do číselníku).
	/// Pokud jde o výčet, který je nullable, musí se do názvu typu uvést i otazník (ala int?).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyTypeAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Typ vygenerovanej property.
		/// </summary>
		public string PropertyType { get; }

		/// <summary>
		/// Určuje celý název typu, který bude použit jako konvertor datového typu při načítání hodnoty z databáze a při ukládání hodnoty do databáze.
		/// </summary>
		public string Converter { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public PropertyTypeAttribute(string propertyType)
		{
			if (string.IsNullOrEmpty(propertyType))
			{
				throw new ArgumentNullException(nameof(propertyType));
			}
			PropertyType = propertyType;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
			{
				{ "PropertyType", PropertyType }
			}
			.AddIfNotDefault("PropertyTypeConverter", Converter, null);
	}
}
