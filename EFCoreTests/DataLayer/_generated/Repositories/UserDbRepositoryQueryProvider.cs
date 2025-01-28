using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Microsoft.EntityFrameworkCore;
using DbContext = Havit.Data.EntityFrameworkCore.DbContext;

namespace Havit.EFCoreTests.DataLayer.Repositories;

internal class UserDbRepositoryQueryProvider : IRepositoryQueryProvider<Havit.EFCoreTests.Model.User, System.Guid>
{
	private readonly ISoftDeleteManager _softDeleteManager;

	private readonly Func<DbContext, System.Guid, Havit.EFCoreTests.Model.User> _getObjectQuery;
	private readonly Func<DbContext, System.Guid, CancellationToken, Task<Havit.EFCoreTests.Model.User>> _getObjectAsyncQuery;
	private readonly Func<DbContext, System.Guid[], IEnumerable<Havit.EFCoreTests.Model.User>> _getObjectsQuery;
	private readonly Func<DbContext, System.Guid[], IAsyncEnumerable<Havit.EFCoreTests.Model.User>> _getObjectsAsyncQuery;
	private readonly Func<DbContext, IEnumerable<Havit.EFCoreTests.Model.User>> _getAllQuery;
	private readonly Func<DbContext, IAsyncEnumerable<Havit.EFCoreTests.Model.User>> _getAllAsyncQuery;

	public UserDbRepositoryQueryProvider(ISoftDeleteManager softDeleteManager)
	{
		_softDeleteManager = softDeleteManager;

		_getObjectQuery = EF.CompileQuery((DbContext dbContext, System.Guid id) => dbContext
			.Set<Havit.EFCoreTests.Model.User>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.User, System.Guid>.GetObject)))
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Guid id, CancellationToken cancellationToken) => dbContext
			.Set<Havit.EFCoreTests.Model.User>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.User, System.Guid>.GetObjectAsync)))
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectsQuery = EF.CompileQuery((DbContext dbContext, System.Guid[] ids) => dbContext
			.Set<Havit.EFCoreTests.Model.User>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.User, System.Guid>.GetObjects)))
			.Where(entity => ids.Contains(entity.Id)));

		_getObjectsAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Guid[] ids) => dbContext
			.Set<Havit.EFCoreTests.Model.User>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.User, System.Guid>.GetObjectsAsync)))
			.Where(entity => ids.Contains(entity.Id)));

		_getAllQuery = EF.CompileQuery((DbContext dbContext) => dbContext
			.Set<Havit.EFCoreTests.Model.User>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.User, System.Guid>.GetAll)))
			.WhereNotDeleted(_softDeleteManager));

		_getAllAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext) => dbContext
			.Set<Havit.EFCoreTests.Model.User>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.User, System.Guid>.GetAllAsync)))
			.WhereNotDeleted(_softDeleteManager));
	}

	public Func<DbContext, System.Guid, Havit.EFCoreTests.Model.User> GetGetObjectQuery() => _getObjectQuery;
	public Func<DbContext, System.Guid, CancellationToken, Task<Havit.EFCoreTests.Model.User>> GetGetObjectAsyncQuery() => _getObjectAsyncQuery;
	public Func<DbContext, System.Guid[], IEnumerable<Havit.EFCoreTests.Model.User>> GetGetObjectsQuery() => _getObjectsQuery;
	public Func<DbContext, System.Guid[], IAsyncEnumerable<Havit.EFCoreTests.Model.User>> GetGetObjectsAsyncQuery() => _getObjectsAsyncQuery;
	public Func<DbContext, IAsyncEnumerable<Havit.EFCoreTests.Model.User>> GetGetAllAsyncQuery() => _getAllAsyncQuery;
	public Func<DbContext, IEnumerable<Havit.EFCoreTests.Model.User>> GetGetAllQuery() => _getAllQuery;
}
