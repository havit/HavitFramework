using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// Class that bridges singleton <see cref="IModelSource"/> to scoped <see cref="IScopedModelSource"/> (<see cref="IModelSource"/>).
    ///
    /// Register <see cref="IScopedModelSource"/> into <see cref="DbContext"/>'s service provider with scoped lifetime.
    /// </summary>
    public class ScopeBridgingModelSource : ModelSource
    {
        /// <inheritdoc />
        public ScopeBridgingModelSource(ModelSourceDependencies dependencies) 
            : base(dependencies)
        {
        }

        /// <summary>
        ///     Creates model using <see cref="IScopedModelSource"/>, which is scoped per <see cref="DbContext"/>.
        ///
        ///     Caching is not affected, i.e. it is still active by original implementation in <see cref="ModelSource.GetModel"/>.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <returns> The model to be used. </returns>
        protected override IModel CreateModel(DbContext context, IConventionSetBuilder conventionSetBuilder)
        {
            return context.GetService<IScopedModelSource>().GetModel(context, conventionSetBuilder);
        }
    }
}