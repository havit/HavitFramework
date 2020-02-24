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
    /// </remarks>
    public class ModelExtensionsModelSource : ModelSource
    {
        private readonly ModelExtensionRegistrationConventionPlugin conventionPlugin;

        /// <inheritdoc />
        public ModelExtensionsModelSource(
            ModelExtensionRegistrationConventionPlugin conventionPlugin,
            ModelSourceDependencies dependencies)
            : base(dependencies)
        {
            this.conventionPlugin = conventionPlugin;
        }

        /// <summary>
        ///     Creates the model with Model Extensions annotations.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <remarks>
        ///     Implemented by adding <see cref="ModelExtensionRegistrationConvention"/> into <see cref="ConventionSet"/> used to create <see cref="IModel"/>.
        /// </remarks>
        /// <returns> The model to be used. </returns>
        protected override IModel CreateModel(DbContext context, IConventionSetBuilder conventionSetBuilder)
        {
            var conventionSet = conventionPlugin.ModifyConventions(conventionSetBuilder.CreateConventionSet());

            IModel model = base.CreateModel(context, new StaticConventionSetBuilder(conventionSet));

            return model;
        }
    }
}