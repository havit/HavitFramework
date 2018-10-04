using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity;
using Havit.Data.Entity.Patterns.DataSources;
using Havit.Data.Entity.Patterns.QueryServices;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.DataLayer
{
	public class LanguageDataSource : DbDataSource<Language>
	{
		public LanguageDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager) : base(dbContext, softDeleteManager)
		{
		}
	}
}
