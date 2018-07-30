using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Business.CodeMigrations.DbInjections
{
    public class DbInjectionsAnnotationProvider : MigrationsAnnotationProvider
    {
        private readonly CompositeDbInjectionAnnotationProvider annotationProvider;

        public DbInjectionsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
            : base(dependencies)
        {
            annotationProvider = new CompositeDbInjectionAnnotationProvider(new[]
            {
                new StoredProcedureAnnotationProvider()
            });
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