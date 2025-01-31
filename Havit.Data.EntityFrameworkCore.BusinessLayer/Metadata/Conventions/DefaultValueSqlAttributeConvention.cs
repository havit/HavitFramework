using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.ComponentModel;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Konvence nastavuje hodnotu z atributu <see cref="DefaultValueAttribute"/> jako DefaultValueSql, pokud dosud žádná výchozí hodnota nebyla nastavena.
/// </summary>
public class DefaultValueSqlAttributeConvention : PropertyAttributeConventionBase<DefaultValueSqlAttribute>
{
	public DefaultValueSqlAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
	{
	}

	protected override void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, DefaultValueSqlAttribute attribute, MemberInfo clrMember, IConventionContext context)
	{
		// Systémové tabulky - nemá cenu řešit, nebudou mít attribut.
		// Podpora pro suppress - nemá význam, stačí nepoužít attribut.

		propertyBuilder.HasDefaultValueSql(attribute.Value, fromDataAnnotation: true /* DataAnnotation */);
		propertyBuilder.ValueGenerated(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never, fromDataAnnotation: true /* DataAnnotation */); // https://stackoverflow.com/questions/40655968/how-to-force-default-values-in-an-insert-with-entityframework-core
	}
}
