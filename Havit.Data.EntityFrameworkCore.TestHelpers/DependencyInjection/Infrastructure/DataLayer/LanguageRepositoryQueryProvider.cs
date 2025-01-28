using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;

namespace Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;

/// <summary>
/// Nepotřebujeme implementaci metod. Třída pomáhá jen k ověření, zda lze resolvovat instance z DI containeru.
/// </summary>
internal class LanguageRepositoryQueryProvider : IRepositoryQueryProvider<Language, int>
{
	public Func<DbContext, IAsyncEnumerable<Language>> GetGetAllAsyncQuery()
	{
		throw new NotImplementedException();
	}

	public Func<DbContext, IEnumerable<Language>> GetGetAllQuery()
	{
		throw new NotImplementedException();
	}

	public Func<DbContext, int, CancellationToken, Task<Language>> GetGetObjectAsyncQuery()
	{
		throw new NotImplementedException();
	}

	public Func<DbContext, int, Language> GetGetObjectQuery()
	{
		throw new NotImplementedException();
	}

	public Func<DbContext, int[], IAsyncEnumerable<Language>> GetGetObjectsAsyncQuery()
	{
		throw new NotImplementedException();
	}

	public Func<DbContext, int[], IEnumerable<Language>> GetGetObjectsQuery()
	{
		throw new NotImplementedException();
	}
}
