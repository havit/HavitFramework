using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Repository objektů typu TEntity s podporou asynchronního zpracování.
	/// </summary>	
	[Obsolete("Replaced by IRepository<TEntity> - All methods from this interface has been moved to IRepositoryAsync<TEntity>. Run code generator if the interface is present in a generated code.", error: true)]
	public interface IRepositoryAsync<TEntity>
		where TEntity : class
	{

	}
}