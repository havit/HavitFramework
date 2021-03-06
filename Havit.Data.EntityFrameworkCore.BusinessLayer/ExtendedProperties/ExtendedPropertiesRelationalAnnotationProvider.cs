﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties
{
	internal class ExtendedPropertiesRelationalAnnotationProvider : RelationalAnnotationProvider
	{
		public ExtendedPropertiesRelationalAnnotationProvider(RelationalAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }

		public override IEnumerable<IAnnotation> For(IColumn column)
		{
			return column.PropertyMappings.Select(mapping => mapping.Property).SelectMany(Handle);
		}

		public override IEnumerable<IAnnotation> For(ITable table)
		{
			return table.EntityTypeMappings.Select(mapping => mapping.EntityType).SelectMany(Handle);
		}

		public override IEnumerable<IAnnotation> For(IRelationalModel model)
		{
			return Handle(model.Model);
		}

		private static IEnumerable<IAnnotation> Handle(IAnnotatable annotatable)
		{
			return annotatable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
		}
	}
}
