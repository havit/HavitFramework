using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>	
	/// Pokud je vlastnost třídy modelu označena atributem <see cref="DataTypeAttribute"/> s hodnotou <see cref="DataType.Date"/>, pak se použije datový typ Date.	
	/// </summary>
	public class DataTypeAttributeConvention : PropertyAttributeConventionBase<DataTypeAttribute>
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DataTypeAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
		{
		}

		/// <inheritdoc />
		protected override void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, DataTypeAttribute attribute, MemberInfo clrMember, IConventionContext context)
		{
			// Systémové tabulky - nemá cenu řešit, nebudou mít attribut.
			// Podpora pro suppress - nemá význam, stačí nepoužít attribut.

			if (attribute.DataType == DataType.Date)
			{
				propertyBuilder.HasColumnType("Date", fromDataAnnotation: true /* DataAnnotation */);
			}
			else
			{
				throw new NotSupportedException($"DataType.{attribute.DataType} is not supported, the only supported value on DataTypeAttribute is DataType.Date.");
			}
		}
	}
}
