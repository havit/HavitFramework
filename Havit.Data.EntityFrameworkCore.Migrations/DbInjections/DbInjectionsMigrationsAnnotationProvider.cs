using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    /// Gives access to <see cref="IAnnotation"/>s that represent <see cref="IDbInjection"/> (used by EF Core Migrations on various elements of the <see cref="IModel" />).
    ///
    /// <see cref="IDbInjection"/> objects are stored as <see cref="IAnnotation"/> inside <see cref="IModel"/>.
    /// </summary>
    public class DbInjectionsMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        private readonly IDbInjectionAnnotationProvider annotationProvider;

        /// <inheritdoc />
        public DbInjectionsMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies, IDbInjectionAnnotationProvider annotationProvider)
            : base(dependencies)
        {
            this.annotationProvider = annotationProvider;
        }

        /// <inheritdoc />
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