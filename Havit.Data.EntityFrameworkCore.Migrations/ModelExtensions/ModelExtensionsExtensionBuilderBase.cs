using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// Base class for adding more features to ModelExtensions functionality.
	///
	/// <para>For implementing new functionality, register implementations for various extension points using <see cref="WithOption"/> method.</para>
	/// </summary>
	public abstract class ModelExtensionsExtensionBuilderBase : IModelExtensionsExtensionBuilderInfrastructure
	{
        private readonly ModelExtensionsExtensionBuilder modelExtensionsExtensionBuilder;

        /// <summary>
        /// Konstruktor.
        /// </summary>
		protected ModelExtensionsExtensionBuilderBase(ModelExtensionsExtensionBuilder modelExtensionsExtensionBuilder)
		{
            Contract.Assert<ArgumentNullException>(modelExtensionsExtensionBuilder != null);
            this.modelExtensionsExtensionBuilder = modelExtensionsExtensionBuilder;
		}

        /// <summary>
        /// Gets the core options builder.
        /// </summary>
        private DbContextOptionsBuilder OptionsBuilder => ((IModelExtensionsExtensionBuilderInfrastructure)modelExtensionsExtensionBuilder).OptionsBuilder;

        DbContextOptionsBuilder IModelExtensionsExtensionBuilderInfrastructure.OptionsBuilder => OptionsBuilder;

        /// <summary>
        /// Sets an option by cloning the extension used to store the settings. This ensures the builder
        /// does not modify options that are already in use elsewhere.
        /// </summary>
        protected virtual ModelExtensionsExtensionBuilder WithOption(Func<ModelExtensionsExtension, ModelExtensionsExtension> setAction)
		{
			((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
				setAction(OptionsBuilder.Options.FindExtension<ModelExtensionsExtension>() ?? new ModelExtensionsExtension()));

            return modelExtensionsExtensionBuilder;
        }
	}
}