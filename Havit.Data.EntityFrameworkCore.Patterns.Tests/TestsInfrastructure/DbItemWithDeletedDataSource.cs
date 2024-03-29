﻿using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class DbItemWithDeletedDataSource : DbDataSource<ItemWithDeleted>
{
	public DbItemWithDeletedDataSource(IDbContext dbContext, SoftDeleteManager softDeleteManager) : base(dbContext, softDeleteManager)
	{
	}
}