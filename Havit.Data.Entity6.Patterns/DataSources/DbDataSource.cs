using System.Linq;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataSources;

namespace Havit.Data.Entity.Patterns.DataSources
{
	/// <summary>
	/// Poskytuje datový zdroj objektů TEntity jako IQueryable.
	/// </summary>
	public abstract class DbDataSource<TEntity> : IDataSource<TEntity>
		where TEntity : class
	{
		private readonly ISoftDeleteManager softDeleteManager;
		private readonly IQueryable<TEntity> dbSet;

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
		/// </summary>
		public IQueryable<TEntity> Data
		{
			get
			{
				return dbSet.WhereNotDeleted(softDeleteManager);
			}
		}

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
		/// </summary>
		public IQueryable<TEntity> DataWithDeleted
		{
			get
			{
				return dbSet.AsQueryable();
			}
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected DbDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
		{
			this.dbSet = dbContext.Set<TEntity>();
			this.softDeleteManager = softDeleteManager;
		}
	}
}
