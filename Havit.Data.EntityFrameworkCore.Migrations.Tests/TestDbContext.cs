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
		optionsBuilder.UseModelExtensions(
			builder => builder
				.UseStoredProcedures()
				.UseViews());
		optionsBuilder.UseSqlServer(new Microsoft.Data.SqlClient.SqlConnection("Database=Dummy"));
		optionsBuilder.EnableServiceProviderCaching(false);

		// stub out Model Extender types, so all extenders in test assembly don't interfere with tests.
		// Tests should setup their own types when necessary.
		optionsBuilder.SetModelExtenderTypes(Enumerable.Empty<TypeInfo>());
	}
}
