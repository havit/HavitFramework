using System;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataSources;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources
{
	/// <summary>
	/// Poskytuje datový zdroj objektů TEntity jako IQueryable.
	/// </summary>
	public abstract class DbDataSource<TEntity> : IDataSource<TEntity>
		where TEntity : class
	{
		private readonly ISoftDeleteManager softDeleteManager;
		private readonly Lazy<IDbSet<TEntity>> dbSetLazy;

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
		/// </summary>
		public IQueryable<TEntity> Data => dbSetLazy.Value.AsQueryable().WhereNotDeleted(softDeleteManager);

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
		/// </summary>
		public IQueryable<TEntity> DataWithDeleted => dbSetLazy.Value.AsQueryable();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected DbDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
		{
			this.dbSetLazy = new Lazy<IDbSet<TEntity>>(() => dbContext.Set<TEntity>());
			this.softDeleteManager = softDeleteManager;
		}
	}
}
