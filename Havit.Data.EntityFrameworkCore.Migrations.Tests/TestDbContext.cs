using System.Data.SqlClient;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests;

public class TestDbContext : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
#pragma warning disable CS0618 // Type or member is obsolete
		optionsBuilder.UseModelExtensions(
			builder => builder
				.UseStoredProcedures()
				.UseViews());
#pragma warning restore CS0618 // Type or member is obsolete
		optionsBuilder.UseSqlServer(new Microsoft.Data.SqlClient.SqlConnection("Database=Dummy"));
		optionsBuilder.EnableServiceProviderCaching(false);

		// stub out Model Extender types, so all extenders in test assembly don't interfere with tests.
		// Tests should setup their own types when necessary.
		optionsBuilder.SetModelExtenderTypes(Enumerable.Empty<TypeInfo>());
	}
}
