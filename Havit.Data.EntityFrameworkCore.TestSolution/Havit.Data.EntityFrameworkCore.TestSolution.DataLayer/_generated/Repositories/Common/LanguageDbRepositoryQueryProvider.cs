﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Microsoft.EntityFrameworkCore;
using DbContext = Havit.Data.EntityFrameworkCore.DbContext;

namespace Havit.Data.EntityFrameworkCore.TestSolution.DataLayer.Repositories.Common;

internal class LanguageDbRepositoryQueryProvider : IRepositoryQueryProvider<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language, System.Int32>
{
	private readonly ISoftDeleteManager _softDeleteManager;

	private readonly Func<DbContext, System.Int32, Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language> _getObjectQuery;
	private readonly Func<DbContext, System.Int32, CancellationToken, Task<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> _getObjectAsyncQuery;
	private readonly Func<DbContext, System.Int32[], IEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> _getObjectsQuery;
	private readonly Func<DbContext, System.Int32[], IAsyncEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> _getObjectsAsyncQuery;
	private readonly Func<DbContext, IEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> _getAllQuery;
	private readonly Func<DbContext, IAsyncEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> _getAllAsyncQuery;

	public LanguageDbRepositoryQueryProvider(ISoftDeleteManager softDeleteManager)
	{
		_softDeleteManager = softDeleteManager;

		_getObjectQuery = EF.CompileQuery((DbContext dbContext, System.Int32 id) => dbContext
			.Set<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>()
			.TagWith("LanguageDbRepository.GetObject")
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Int32 id, CancellationToken cancellationToken) => dbContext
			.Set<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>()
			.TagWith("LanguageDbRepository.GetObjectAsync")
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectsQuery = EF.CompileQuery((DbContext dbContext, System.Int32[] ids) => dbContext
			.Set<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>()
			.TagWith("LanguageDbRepository.GetObjects")
			.Where(entity => ids.Contains(entity.Id)));

		_getObjectsAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Int32[] ids) => dbContext
			.Set<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>()
			.TagWith("LanguageDbRepository.GetObjectsAsync")
			.Where(entity => ids.Contains(entity.Id)));

		_getAllQuery = EF.CompileQuery((DbContext dbContext) => dbContext
			.Set<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>()
			.TagWith("LanguageDbRepository.GetAll")
			.WhereNotDeleted(_softDeleteManager));

		_getAllAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext) => dbContext
			.Set<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>()
			.TagWith("LanguageDbRepository.GetAllAsync")
			.WhereNotDeleted(_softDeleteManager));
	}

	public Func<DbContext, System.Int32, Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language> GetGetObjectQuery() => _getObjectQuery;
	public Func<DbContext, System.Int32, CancellationToken, Task<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> GetGetObjectAsyncQuery() => _getObjectAsyncQuery;
	public Func<DbContext, System.Int32[], IEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> GetGetObjectsQuery() => _getObjectsQuery;
	public Func<DbContext, System.Int32[], IAsyncEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> GetGetObjectsAsyncQuery() => _getObjectsAsyncQuery;
	public Func<DbContext, IAsyncEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> GetGetAllAsyncQuery() => _getAllAsyncQuery;
	public Func<DbContext, IEnumerable<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>> GetGetAllQuery() => _getAllQuery;
}
