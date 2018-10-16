using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer
{
	public class LanguageRepository : DbRepository<Language>, ILanguageRepository
	{
		public LanguageRepository(IDbContext dbContext, ILanguageDataSource dataSource, IEntityKeyAccessor<Language, int> entityKeyAccessor, IDataLoader dataLoader, IDataLoaderAsync dataLoaderAsync, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager)
			: base(dbContext, dataSource, entityKeyAccessor, dataLoader, dataLoaderAsync, softDeleteManager, entityCacheManager)
		{
			
		}
	}
}
