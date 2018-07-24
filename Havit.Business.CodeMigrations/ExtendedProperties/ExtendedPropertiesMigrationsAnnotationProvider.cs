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
			return Handle(property, base.For(property));
		}

		public override IEnumerable<IAnnotation> For(IEntityType entityType)
		{
			return Handle(entityType, base.For(entityType));
		}

		public override IEnumerable<IAnnotation> For(IModel model)
		{
			return Handle(model, base.For(model));
		}

		private static IEnumerable<IAnnotation> Handle(IAnnotatable annotatable, IEnumerable<IAnnotation> @base)
		{
			return @base.Concat(annotatable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation));
		}
	}
}
