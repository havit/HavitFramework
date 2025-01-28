using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.EFCoreTests.Model;
using System.Linq.Expressions;

namespace Havit.EFCoreTests.DataLayer.Lookups;

public class UserLookupService : LookupServiceBase<object, User, System.Guid>, IUserLookupService
{
	public UserLookupService(IEntityLookupDataStorage lookupStorage, IRepository<User, Guid> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager) : base(lookupStorage, repository, dbContext, entityKeyAccessor, softDeleteManager)
	{
	}

	protected override Expression<Func<User, object>> LookupKeyExpression => user => new { A = user.Username, B = user.Username };

	protected override LookupServiceOptimizationHints OptimizationHints => LookupServiceOptimizationHints.None;

	public User GetUserByUsername(string username) => GetEntityByLookupKey(new { A = username, B = username });
}
