using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Microsoft.EntityFrameworkCore;
using DbContext = Havit.Data.EntityFrameworkCore.DbContext;

namespace Havit.EFCoreTests.DataLayer.Repositories;

internal class StateLocalizationDbRepositoryQueryProvider : IRepositoryQueryProvider<Havit.EFCoreTests.Model.StateLocalization, System.Int32>
{
	private readonly ISoftDeleteManager _softDeleteManager;

	private readonly Func<DbContext, System.Int32, Havit.EFCoreTests.Model.StateLocalization> _getObjectQuery;
	private readonly Func<DbContext, System.Int32, CancellationToken, Task<Havit.EFCoreTests.Model.StateLocalization>> _getObjectAsyncQuery;
	private readonly Func<DbContext, System.Int32[], IEnumerable<Havit.EFCoreTests.Model.StateLocalization>> _getObjectsQuery;
	private readonly Func<DbContext, System.Int32[], IAsyncEnumerable<Havit.EFCoreTests.Model.StateLocalization>> _getObjectsAsyncQuery;
	private readonly Func<DbContext, IEnumerable<Havit.EFCoreTests.Model.StateLocalization>> _getAllQuery;
	private readonly Func<DbContext, IAsyncEnumerable<Havit.EFCoreTests.Model.StateLocalization>> _getAllAsyncQuery;

	public StateLocalizationDbRepositoryQueryProvider(ISoftDeleteManager softDeleteManager)
	{
		_softDeleteManager = softDeleteManager;

		_getObjectQuery = EF.CompileQuery((DbContext dbContext, System.Int32 id) => dbContext
			.Set<Havit.EFCoreTests.Model.StateLocalization>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>.GetObject)))
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Int32 id, CancellationToken cancellationToken) => dbContext
			.Set<Havit.EFCoreTests.Model.StateLocalization>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>.GetObjectAsync)))
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectsQuery = EF.CompileQuery((DbContext dbContext, System.Int32[] ids) => dbContext
			.Set<Havit.EFCoreTests.Model.StateLocalization>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>.GetObjects)))
			.Where(entity => ids.Contains(entity.Id)));

		_getObjectsAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Int32[] ids) => dbContext
			.Set<Havit.EFCoreTests.Model.StateLocalization>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>.GetObjectsAsync)))
			.Where(entity => ids.Contains(entity.Id)));

		_getAllQuery = EF.CompileQuery((DbContext dbContext) => dbContext
			.Set<Havit.EFCoreTests.Model.StateLocalization>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>.GetAll)))
			.WhereNotDeleted(_softDeleteManager));

		_getAllAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext) => dbContext
			.Set<Havit.EFCoreTests.Model.StateLocalization>()
			.TagWith(Data.EntityFrameworkCore.QueryTagBuilder.CreateTag(typeof(Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository), nameof(Data.Patterns.Repositories.IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>.GetAllAsync)))
			.WhereNotDeleted(_softDeleteManager));
	}

	public Func<DbContext, System.Int32, Havit.EFCoreTests.Model.StateLocalization> GetGetObjectQuery() => _getObjectQuery;
	public Func<DbContext, System.Int32, CancellationToken, Task<Havit.EFCoreTests.Model.StateLocalization>> GetGetObjectAsyncQuery() => _getObjectAsyncQuery;
	public Func<DbContext, System.Int32[], IEnumerable<Havit.EFCoreTests.Model.StateLocalization>> GetGetObjectsQuery() => _getObjectsQuery;
	public Func<DbContext, System.Int32[], IAsyncEnumerable<Havit.EFCoreTests.Model.StateLocalization>> GetGetObjectsAsyncQuery() => _getObjectsAsyncQuery;
	public Func<DbContext, IAsyncEnumerable<Havit.EFCoreTests.Model.StateLocalization>> GetGetAllAsyncQuery() => _getAllAsyncQuery;
	public Func<DbContext, IEnumerable<Havit.EFCoreTests.Model.StateLocalization>> GetGetAllQuery() => _getAllQuery;
}
