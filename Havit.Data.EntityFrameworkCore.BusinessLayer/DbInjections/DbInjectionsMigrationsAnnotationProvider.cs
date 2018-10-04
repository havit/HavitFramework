using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class DbInjectionsMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        private readonly IDbInjectionAnnotationProvider annotationProvider;

        public DbInjectionsMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies, IDbInjectionAnnotationProvider annotationProvider)
            : base(dependencies)
        {
            this.annotationProvider = annotationProvider;
        }

        public override IEnumerable<IAnnotation> For(IModel model)
        {
            return model.GetAnnotations().Where(IsDbInjection);
        }

        private bool IsDbInjection(IAnnotation annotation)
        {
            return annotationProvider.GetDbInjections(new List<IAnnotation> { annotation }).Any();
        }
    }
}