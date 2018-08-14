using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.StoredProcedures
{
	public class StoredProceduresMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
	    public StoredProceduresMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies) 
	        : base(dependencies)
	    {
	    }

	    public override IEnumerable<IAnnotation> For(IModel model)
	    {
	        return model.GetAnnotations().Where(StoredProceduresAnnotationsHelper.IsStoredProcedureAnnotation);
	    }
	}
}