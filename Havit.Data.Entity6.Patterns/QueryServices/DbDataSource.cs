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
		/// Indikuje, zda mají být vráceny i (příznakem) smazané záznamy. Výchozí hodnota je false.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// IncludeDeleted je nastaveno na true, ale na datovém typu není podporováno mazání příznakem.
		/// </exception>
		public bool IncludeDeleted
		{
			get
			{
				return _includeDeleted;
			}
			set
			{
				if (value && !softDeleteManager.IsSoftDeleteSupported<TEntity>())
				{
					throw new InvalidOperationException("Nastavení IncludeDeleted na true na tomto typu není podporován.");
				}

				_includeDeleted = value;
			}
		}
		private bool _includeDeleted = false;

		/// <summary>
		/// Data z datového zdroje jako IQueryable.
		/// </summary>
		public IQueryable<TEntity> Data
		{
			get
			{
				if (!IncludeDeleted)
				{
					return dbSet.WhereNotDeleted(softDeleteManager);
				}
				else
				{
					return dbSet;
				}				
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
