using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSources;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.DataLayer
{
	public class LanguageRepository : DbRepository<Language>, ILanguageRepository
	{
		public LanguageRepository(IDbContext dbContext, IDataSource<Language> dataSource, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager) : base(dbContext, dataSource, dataLoader, softDeleteManager)
		{
		}
	}
}
