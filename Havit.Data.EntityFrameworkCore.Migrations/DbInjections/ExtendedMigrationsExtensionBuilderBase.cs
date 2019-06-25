using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	/// <summary>
	/// Base class for adding more features to DbInjections functionality.
	///
	/// <para>For implementing new functionality, register implementations for various extension points using <see cref="WithOption"/> method.</para>
	/// </summary>
	public abstract class ExtendedMigrationsExtensionBuilderBase : IExtendedMigrationsExtensionBuilderInfrastructure
	{
        private readonly ExtendedMigrationsExtensionBuilder extendedMigrationsExtensionBuilder;

        /// <summary>
        /// Konstruktor.
        /// </summary>
		protected ExtendedMigrationsExtensionBuilderBase(ExtendedMigrationsExtensionBuilder extendedMigrationsExtensionBuilder)
		{
            Contract.Assert<ArgumentNullException>(extendedMigrationsExtensionBuilder != null);
            this.extendedMigrationsExtensionBuilder = extendedMigrationsExtensionBuilder;
		}

        /// <summary>
        /// Gets the core options builder.
        /// </summary>
        private DbContextOptionsBuilder OptionsBuilder => ((IExtendedMigrationsExtensionBuilderInfrastructure)extendedMigrationsExtensionBuilder).OptionsBuilder;

        DbContextOptionsBuilder IExtendedMigrationsExtensionBuilderInfrastructure.OptionsBuilder => OptionsBuilder;

        /// <summary>
        /// Sets an option by cloning the extension used to store the settings. This ensures the builder
        /// does not modify options that are already in use elsewhere.
        /// </summary>
        protected virtual ExtendedMigrationsExtensionBuilder WithOption(Func<DbInjectionsExtension, DbInjectionsExtension> setAction)
		{
			((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
				setAction(OptionsBuilder.Options.FindExtension<DbInjectionsExtension>() ?? new DbInjectionsExtension()));

            return extendedMigrationsExtensionBuilder;
        }
	}
}