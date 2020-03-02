using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions
{
    /// <summary>
    /// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="CompositeMigrationsSqlGenerator"/>.
    /// </summary>
    public class CompositeMigrationsSqlGeneratorExtension : IDbContextOptionsExtension
	{
		private ImmutableList<Type> generatorTypes = ImmutableList.Create<Type>();

		private DbContextOptionsExtensionInfo _info;

		/// <inheritdoc />
		public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CompositeMigrationsSqlGeneratorExtension()
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected CompositeMigrationsSqlGeneratorExtension(CompositeMigrationsSqlGeneratorExtension copyFrom)
		{
			generatorTypes = copyFrom.generatorTypes;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        protected virtual CompositeMigrationsSqlGeneratorExtension Clone() => new CompositeMigrationsSqlGeneratorExtension(this);

        /// <summary>
        /// Registers specified type of <see cref="IMigrationOperationSqlGenerator"/>.
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="IMigrationOperationSqlGenerator"/></typeparam>
        /// <returns>Clone of <see cref="CompositeMigrationsSqlGeneratorExtension"/>.</returns>
        public CompositeMigrationsSqlGeneratorExtension WithGeneratorType<T>()
            where T : IMigrationOperationSqlGenerator
		{
			var clone = Clone();
			clone.generatorTypes = generatorTypes.Add(typeof(T));
			return clone;
		}

        /// <inheritdoc />
        public void ApplyServices(IServiceCollection services)
        {
            var currentProviderTypes = generatorTypes.ToArray();
			//         CompositeMigrationsSqlGenerator Factory(IServiceProvider serviceProvider)
			//         {
			//             var generators = currentProviderTypes.Select(type => (IMigrationOperationSqlGenerator)serviceProvider.GetService(type)).ToArray();
			//	return new CompositeMigrationsSqlGenerator(serviceProvider.GetService<MigrationsSqlGeneratorDependencies>(), serviceProvider.GetService<IMigrationsAnnotationProvider>(), generators);
			//}

			services.Add(currentProviderTypes.Select(t => ServiceDescriptor.Scoped(typeof(IMigrationOperationSqlGenerator), t)));
			// Dříve (EF Core 2.x) jsme měli Singleton, avšak při použití v EF Core 3.0 dostáváme při singletonu výjimku
			// System.ArgumentNullException: Value cannot be null. (Parameter 'currentContext')
			// vyhozenou z konstruktoru MigrationsSqlGeneratorDependencies.
			// Dle "dokumentace" (https://github.com/aspnet/EntityFrameworkCore/blob/24b9aa1d2e14fe2e737255ede9b2a7a623fcf2af/src/EFCore.Relational/Infrastructure/EntityFrameworkRelationalServicesBuilder.cs) máme mít Scoped.
			services.Replace(ServiceDescriptor.Scoped<IMigrationsSqlGenerator, CompositeMigrationsSqlGenerator>());
        }

        /// <inheritdoc />
        public void Validate(IDbContextOptions options)
        {
            // no validation
        }

		private class ExtensionInfo : DbContextOptionsExtensionInfo
		{
			public override bool IsDatabaseProvider => false;

			public override string LogFragment => "";

			public ExtensionInfo(IDbContextOptionsExtension dbContextOptionsExtension) : base(dbContextOptionsExtension)
			{
			}

			public override long GetServiceProviderHashCode()
			{
				return 0x581B;
			}

			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
			{
				// NOOP
			}
		}
	}
}