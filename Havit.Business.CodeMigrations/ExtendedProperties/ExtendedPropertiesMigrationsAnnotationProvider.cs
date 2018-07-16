using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal class ExtendedPropertiesMigrationsAnnotationProvider : SqlServerMigrationsAnnotationProvider
	{
		public ExtendedPropertiesMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }

		public override IEnumerable<IAnnotation> For(IProperty property)
		{
			return base.For(property)
				.Concat(property.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.AnnotationsFilter));
		}

		public override IEnumerable<IAnnotation> ForRemove(IEntityType entityType)
		{
			return base.ForRemove(entityType)
				.Concat(entityType.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.AnnotationsFilter));
		}
	}
}
