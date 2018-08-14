using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	public static class InfrastructureExtensions
	{
		public static void UseCodeMigrationsInfrastructure(this IDbContextOptionsBuilderInfrastructure optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			optionsBuilder.AddOrUpdateExtension(new CompositeMigrationsAnnotationProviderExtension().WithAnnotationProvider<SqlServerMigrationsAnnotationProvider>());
			optionsBuilder.AddOrUpdateExtension(new CompositeMigrationsSqlGeneratorExtension());
		}
	}
}