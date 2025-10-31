using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.Infrastructure;

/// <summary>
/// Tests involving <see cref="CompositeMigrationsAnnotationProviderExtension"/>, e.g. registration of required services, resolving etc.
/// </summary>
[TestClass]
public class CompositeMigrationsAnnotationProviderExtensionTests
{
	/// <summary>
	/// Tests whether registering same <see cref="IMigrationsAnnotationProvider"/> into <see cref="CompositeMigrationsAnnotationProvider"/>
	/// does not produce duplicate types.
	/// </summary>
	[TestMethod]
	public void CompositeMigrationsAnnotationProvider_RegisterSameProviderTwice_ProviderIsRegisteredOnlyOnce()
	{
		static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseExtendedMigrationsInfrastructure();

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(optionsBuilder.Options
				.FindExtension<CompositeMigrationsAnnotationProviderExtension>()
				.WithAnnotationProvider<FakeMigrationsAnnotationProvider>()
				.WithAnnotationProvider<FakeMigrationsAnnotationProvider>());
		}

		using (var dbContext = new ExtendedMigrationsTestDbContext(OnConfiguring))
		{
			_ = dbContext.Model;

			Assert.AreEqual(1, dbContext.CompositeMigrationsAnnotationProviderExtension.Providers.Count(type => type == typeof(FakeMigrationsAnnotationProvider)));
		}
	}

	private class FakeMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
		public FakeMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{
		}
	}
}