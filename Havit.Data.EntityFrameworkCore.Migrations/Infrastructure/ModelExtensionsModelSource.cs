using Havit.Data.EntityFrameworkCore.Migrations.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// Internal class for creating <see cref="IModel"/> that adds annotations for Model Extensions using convention.
    /// </summary>
    /// <remarks>
    /// Applies <see cref="ModelExtensionRegistrationConvention"/> only to main DbContext model (i.e. adds Model Extensions annotations).
    /// Does not pollute any other <see cref="IModel"/> created (within DbContext using this <see cref="IModelSource"/>) - mainly HistoryRepository's model.
    /// 
    /// Fixes bug #48448 with polluting HistoryRepository model (and other secondary models).
    ///
    /// <see cref="ModelExtensionsModelSource"/> is scoped, because <see cref="ModelExtensionRegistrationConventionPlugin"/> is scoped as well.
    /// However EF Core depends on <see cref="IModelSource"/> being singleton, so we want to adhere to this precondition.
    /// <see cref="ScopeBridgingModelSource"/> handles transitioning to scope of currently used DbContext along with caching implemented in <see cref="ModelSource"/>.
    /// </remarks>
    public class ModelExtensionsModelSource : ModelSource, IScopedModelSource
    {
        private readonly ModelExtensionRegistrationConventionPlugin conventionPlugin;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ModelExtensionsModelSource(
            ModelSourceDependencies dependencies,
            ModelExtensionRegistrationConventionPlugin conventionPlugin)
            : base(dependencies)
        {
            this.conventionPlugin = conventionPlugin;
        }

        /// <summary>
        ///     Creates the model with Model Extensions annotations. Caching is not implemented,
        ///     since dependency <see cref="ModelExtensionRegistrationConventionPlugin"/> has scoped lifestyle.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <remarks>
        ///     Implemented by adding <see cref="ModelExtensionRegistrationConvention"/> into <see cref="ConventionSet"/> used to create <see cref="IModel"/>.
        /// </remarks>
        /// <returns> The model to be used. </returns>
        public override IModel GetModel(DbContext context, IConventionSetBuilder conventionSetBuilder)
        {
            var conventionSet = conventionPlugin.ModifyConventions(conventionSetBuilder.CreateConventionSet());

            // suppress reason: need to call base implementation that actually creates IModel (without duplicating its code)
#pragma warning disable SA1100 // Do not prefix calls with base unless local implementation exists
            IModel model = base.CreateModel(context, new StaticConventionSetBuilder(conventionSet));
#pragma warning restore SA1100 // Do not prefix calls with base unless local implementation exists

            return model;
        }
    }
}