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
	public abstract class DbInjectionsExtensionBuilderBase : IDbInjectionsExtensionBuilderInfrastructure
	{
        private readonly DbInjectionsExtensionBuilder dbInjectionsExtensionBuilder;

        /// <summary>
        /// Konstruktor.
        /// </summary>
		protected DbInjectionsExtensionBuilderBase(DbInjectionsExtensionBuilder dbInjectionsExtensionBuilder)
		{
            Contract.Assert<ArgumentNullException>(dbInjectionsExtensionBuilder != null);
            this.dbInjectionsExtensionBuilder = dbInjectionsExtensionBuilder;
		}

        /// <summary>
        /// Gets the core options builder.
        /// </summary>
        private DbContextOptionsBuilder OptionsBuilder => ((IDbInjectionsExtensionBuilderInfrastructure)dbInjectionsExtensionBuilder).OptionsBuilder;

        DbContextOptionsBuilder IDbInjectionsExtensionBuilderInfrastructure.OptionsBuilder => OptionsBuilder;

        /// <summary>
        /// Sets an option by cloning the extension used to store the settings. This ensures the builder
        /// does not modify options that are already in use elsewhere.
        /// </summary>
        protected virtual DbInjectionsExtensionBuilder WithOption(Func<DbInjectionsExtension, DbInjectionsExtension> setAction)
		{
			((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
				setAction(OptionsBuilder.Options.FindExtension<DbInjectionsExtension>() ?? new DbInjectionsExtension()));

            return dbInjectionsExtensionBuilder;
        }
	}
}