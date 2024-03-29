﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Internal;

internal static class DbContextExtensions
{
	internal static bool SupportsSqlServerOpenJson(this IDbContext dbContext)
	{
		var sqlServerSingletonOptions = (dbContext as Microsoft.EntityFrameworkCore.DbContext)?.GetService<ISqlServerSingletonOptions>();
		return (sqlServerSingletonOptions == null)
			? false
			: sqlServerSingletonOptions.CompatibilityLevel > 120;

	}
}
