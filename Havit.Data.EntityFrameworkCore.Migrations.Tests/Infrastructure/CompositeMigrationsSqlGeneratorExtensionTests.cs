using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.Infrastructure;

/// <summary>
/// Tests involving <see cref="CompositeMigrationsSqlGeneratorExtension"/>, e.g. registration of required services, resolving etc.
/// </summary>
[TestClass]
public class CompositeMigrationsSqlGeneratorExtensionTests
{
	/// <summary>
	/// Tests whether resolving IMigrationOperationSqlGenerator from two DbContext instances
	/// return two distinct instances of IMigrationOperationSqlGenerator.
	/// </summary>
	[TestMethod]
	public void CompositeMigrationsSqlGeneratorExtension_ResolvingIMigrationsSqlGeneratorTwice_ResolvedInstancesAreNotSame()
	{
		static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseExtendedMigrationsInfrastructure();

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(optionsBuilder.Options
				.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
				.WithGeneratorType<FakeMigrationOperationSqlGenerator>());
		}

		using (var dbContext1 = new ExtendedMigrationsTestDbContext(OnConfiguring))
		using (var dbContext2 = new ExtendedMigrationsTestDbContext(OnConfiguring))
		{
			var instance1 = dbContext1.GetService<IMigrationsSqlGenerator>();
			var instance2 = dbContext2.GetService<IMigrationsSqlGenerator>();
			Assert.AreNotSame(instance1, instance2);
		}
	}

	/// <summary>
	/// Tests whether resolved IMigrationOperationSqlGenerator have access to ICurrentDbContext,
	/// i.e. they're resolved in scope of current DbContext.
	/// </summary>
	[TestMethod]
	public void CompositeMigrationsSqlGeneratorExtension_ResolveMigrationOperationSqlGenerator_ResolvedMigrationOperationSqlGeneratorCanAccessDbContext()
	{
		static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseExtendedMigrationsInfrastructure();

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(optionsBuilder.Options
				.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
				.WithGeneratorType<FakeMigrationOperationSqlGenerator>());
		}

		using (var dbContext = new ExtendedMigrationsTestDbContext(OnConfiguring))
		{
			var composite = dbContext.GetService<IMigrationOperationSqlGenerator>();

			Assert.IsInstanceOfType(composite, typeof(FakeMigrationOperationSqlGenerator));

			Assert.IsNotNull(((FakeMigrationOperationSqlGenerator)composite).CurrentDbContext);
		}
	}

	/// <summary>
	/// Tests whether registering same <see cref="IMigrationOperationSqlGenerator"/> into <see cref="CompositeMigrationsSqlGeneratorExtension"/>
	/// does not produce duplicate types.
	/// </summary>
	[TestMethod]
	public void CompositeMigrationsSqlGeneratorExtension_RegisterSameGeneratorTwice_GeneratorIsRegisteredOnlyOnce()
	{
		static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseExtendedMigrationsInfrastructure();

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(optionsBuilder.Options
				.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
				.WithGeneratorType<FakeMigrationOperationSqlGenerator>()
				.WithGeneratorType<FakeMigrationOperationSqlGenerator>());
		}

		using (var dbContext = new ExtendedMigrationsTestDbContext(OnConfiguring))
		{
			_ = dbContext.Model;

			Assert.HasCount(1, dbContext.CompositeMigrationsSqlGeneratorExtension.GeneratorTypes);

			Assert.AreSame(typeof(FakeMigrationOperationSqlGenerator), dbContext.CompositeMigrationsSqlGeneratorExtension.GeneratorTypes.First());
		}
	}

	/// <summary>
	/// Tests whether registering multiple <see cref="IMigrationOperationSqlGenerator"/> into <see cref="CompositeMigrationsSqlGeneratorExtension"/>
	/// actually registers them in correct order (i.e. order they were registered).
	/// </summary>
	[TestMethod]
	public void CompositeMigrationsSqlGeneratorExtension_RegisterMultipleGenerators_GeneratorsAreRegisteredInCorrectOrder()
	{
		static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseExtendedMigrationsInfrastructure();

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(optionsBuilder.Options
				.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
				.WithGeneratorType<FakeMigrationOperationSqlGenerator>()
				.WithGeneratorType<SecondFakeMigrationOperationSqlGenerator>());
		}

		using (var dbContext = new ExtendedMigrationsTestDbContext(OnConfiguring))
		{
			_ = dbContext.Model;

			Assert.HasCount(2, dbContext.CompositeMigrationsSqlGeneratorExtension.GeneratorTypes);

			Assert.AreSame(typeof(FakeMigrationOperationSqlGenerator), dbContext.CompositeMigrationsSqlGeneratorExtension.GeneratorTypes[0]);
			Assert.AreSame(typeof(SecondFakeMigrationOperationSqlGenerator), dbContext.CompositeMigrationsSqlGeneratorExtension.GeneratorTypes[1]);
		}
	}

	private class FakeMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
	{
		public ICurrentDbContext CurrentDbContext { get; }

		public FakeMigrationOperationSqlGenerator(ICurrentDbContext currentDbContext)
		{
			CurrentDbContext = currentDbContext;
		}
	}

	private class SecondFakeMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
	{
	}
}