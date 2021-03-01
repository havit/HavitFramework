using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties
{
    internal class ExtendedPropertiesMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
		public ExtendedPropertiesMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }

		public override IEnumerable<IAnnotation> ForRemove(IColumn column)
		{
			return Handle(column);
		}

		public override IEnumerable<IAnnotation> ForRemove(ITable table)
		{
			return Handle(table);
		}

		public override IEnumerable<IAnnotation> ForRemove(IRelationalModel model)
		{
			return Handle(model);
		}

		private static IEnumerable<IAnnotation> Handle(IAnnotatable annotatable)
		{
			return annotatable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
		}
	}
}
