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

namespace Havit.EFCoreTests.DataLayer.Repositories;

internal class AddressDbRepositoryQueryProvider : IRepositoryQueryProvider<Havit.EFCoreTests.Model.Address, System.Int32>
{
	private readonly Func<DbContext, System.Int32, Havit.EFCoreTests.Model.Address> _getObjectQuery;
	private readonly Func<DbContext, System.Int32, CancellationToken, Task<Havit.EFCoreTests.Model.Address>> _getObjectAsyncQuery;
	private readonly Func<DbContext, System.Int32[], IEnumerable<Havit.EFCoreTests.Model.Address>> _getObjectsQuery;
	private readonly Func<DbContext, System.Int32[], IAsyncEnumerable<Havit.EFCoreTests.Model.Address>> _getObjectsAsyncQuery;

	public AddressDbRepositoryQueryProvider()
	{
		_getObjectQuery = EF.CompileQuery((DbContext dbContext, System.Int32 id) => dbContext
			.Set<Havit.EFCoreTests.Model.Address>()
			.TagWith("AddressDbRepository.GetObject")
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Int32 id, CancellationToken cancellationToken) => dbContext
			.Set<Havit.EFCoreTests.Model.Address>()
			.TagWith("AddressDbRepository.GetObjectAsync")
			.Where(entity => entity.Id == id)
			.FirstOrDefault());

		_getObjectsQuery = EF.CompileQuery((DbContext dbContext, System.Int32[] ids) => dbContext
			.Set<Havit.EFCoreTests.Model.Address>()
			.TagWith("AddressDbRepository.GetObjects")
			.Where(entity => ids.Contains(entity.Id)));

		_getObjectsAsyncQuery = EF.CompileAsyncQuery((DbContext dbContext, System.Int32[] ids) => dbContext
			.Set<Havit.EFCoreTests.Model.Address>()
			.TagWith("AddressDbRepository.GetObjectsAsync")
			.Where(entity => ids.Contains(entity.Id)));
	}

	public Func<DbContext, System.Int32, Havit.EFCoreTests.Model.Address> GetGetObjectQuery() => _getObjectQuery;
	public Func<DbContext, System.Int32, CancellationToken, Task<Havit.EFCoreTests.Model.Address>> GetGetObjectAsyncQuery() => _getObjectAsyncQuery;
	public Func<DbContext, System.Int32[], IEnumerable<Havit.EFCoreTests.Model.Address>> GetGetObjectsQuery() => _getObjectsQuery;
	public Func<DbContext, System.Int32[], IAsyncEnumerable<Havit.EFCoreTests.Model.Address>> GetGetObjectsAsyncQuery() => _getObjectsAsyncQuery;
}
