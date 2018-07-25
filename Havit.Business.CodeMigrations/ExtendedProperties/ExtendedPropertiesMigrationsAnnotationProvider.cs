using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
    internal class ExtendedPropertiesMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
		public ExtendedPropertiesMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }

		public override IEnumerable<IAnnotation> For(IProperty property)
		{
			return Handle(property);
		}

		public override IEnumerable<IAnnotation> For(IEntityType entityType)
		{
			return Handle(entityType);
		}

		public override IEnumerable<IAnnotation> For(IModel model)
		{
			return Handle(model);
		}

		private static IEnumerable<IAnnotation> Handle(IAnnotatable annotatable)
		{
			return annotatable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
		}
	}
}
