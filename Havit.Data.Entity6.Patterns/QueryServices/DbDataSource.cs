using System;
using System.Linq;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.QueryServices;

namespace Havit.Data.Entity.Patterns.QueryServices
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
		/// Data z datového zdroje jako IQueryable.
		/// </summary>
		public IQueryable<TEntity> Data
		{
			get
			{
				return dbSet.WhereNotDeleted(softDeleteManager);
			}
		}

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
