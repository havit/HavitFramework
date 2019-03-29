using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	/// <summary>
	/// <para>Allows performing specific configuration of DbInjections functionality.</para>
	/// <para>
	///		Instances of this class are typically returned from methods that configure the context to use a
	///     particular extension of DbInjections functionality.
	/// </para>
	/// </summary>
	public abstract class DbInjectionsExtensionBuilderBase<TBuilder>
		where TBuilder : DbInjectionsExtensionBuilderBase<TBuilder>
	{
		/// <summary>
        /// Konstruktor.
        /// </summary>
		protected DbInjectionsExtensionBuilderBase(DbContextOptionsBuilder optionsBuilder)
		{
			Contract.Assert<ArgumentNullException>(optionsBuilder != null);

			OptionsBuilder = optionsBuilder;
		}

		/// <summary>
		/// Gets the core options builder.
		/// </summary>
		protected virtual DbContextOptionsBuilder OptionsBuilder { get; }

		/// <summary>
		/// Sets an option by cloning the extension used to store the settings. This ensures the builder
		/// does not modify options that are already in use elsewhere.
		/// </summary>
		protected virtual TBuilder WithOption(Func<DbInjectionsExtension, DbInjectionsExtension> setAction)
		{
			((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
				setAction(OptionsBuilder.Options.FindExtension<DbInjectionsExtension>() ?? new DbInjectionsExtension()));

			return (TBuilder)this;
		}
	}
}