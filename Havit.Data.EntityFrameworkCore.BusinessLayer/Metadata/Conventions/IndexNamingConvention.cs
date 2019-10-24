using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	// TODO EF Core 3.0: Odstranit

	/*
	/// <summary>
	/// Přejmenovává indexy začínající IX_ na FKX_.
	/// </summary>
	public class IndexNamingConvention : IIndexAddedConvention, IPropertyAnnotationChangedConvention
	{
		public void ProcessIndexAdded(IConventionIndexBuilder indexBuilder, IConventionContext<IConventionIndexBuilder> context)
		{
			// Systémové tabulky nechceme změnit.
			if (indexBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (indexBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed<IndexNamingConvention>())
			{
				return;
			}

			string indexName = indexBuilder.Metadata.GetName();
			if ((indexName == indexBuilder.Metadata.GetDefaultName()) // budeme index měnit jen tehdy, má-li výchozí pojmenování
				&& indexName.StartsWith("IX_"))
			{
				indexBuilder.Metadata.SetName("FKX_" + indexName.Substring(3), fromDataAnnotation: false /* Convention * /);
			}
		}
	}
	*/
}
