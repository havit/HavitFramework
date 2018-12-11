using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Konvence nastavuje hodnotu z atributu <see cref="DefaultValueAttribute"/> jako DefaultValueSql, pokud dosud žádná výchozí hodnota nebyla nastavena.
	/// </summary>
	public class DefaultValueAttributeConvention : IModelConvention
	{
		/// <summary>
		/// Aplikuje konvenci.
		/// </summary>
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (var prop in modelBuilder.Model
				.GetApplicationEntityTypes(includeManyToManyEntities: false)
				.WhereNotConventionSuppressed(this) // testujeme entity types
				.SelectMany(e => e.GetProperties())
				.WhereNotConventionSuppressed(this) // testujeme properties
				.Where(p => !p.IsShadowProperty)
				.Select(p => new { Property = p, Attribute = (DefaultValueAttribute)p.PropertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() })
				.Where(p => p.Attribute != null))
			{
				if (prop.Property.Relational().DefaultValueSql != null || prop.Property.Relational().DefaultValue != null)
				{
					continue;
				}

				prop.Property.Relational().DefaultValueSql = prop.Attribute.Value?.ToString();
				prop.Property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never; // https://stackoverflow.com/questions/40655968/how-to-force-default-values-in-an-insert-with-entityframework-core
			}
		}
	}
}
