using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.Repositories
{
	/// <summary>
	/// Repository objektů typu TEntity.
	/// </summary>
	public abstract class DbRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity>
		 where TEntity : class
	{
		private readonly IDataLoader dataLoader;
		private readonly IDataLoaderAsync dataLoaderAsync;
		private readonly ISoftDeleteManager softDeleteManager;

		private TEntity[] _all;

		/// <summary>
		/// DbSet, nad kterým je DbRepository postaven.
		/// </summary>
		protected DbSet<TEntity> DbSet { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected DbRepository(IDbContext dbContext, IDataLoader dataLoader, IDataLoaderAsync dataLoaderAsync, ISoftDeleteManager softDeleteManager)
		{
			Contract.Requires<ArgumentException>(dbContext != null);
			Contract.Requires<ArgumentException>(softDeleteManager != null);

			var dbSet = dbContext.Set<TEntity>();
			Contract.Assert(dbSet != null);

			this.dataLoader = dataLoader;
			this.dataLoaderAsync = dataLoaderAsync;
			this.softDeleteManager = softDeleteManager;

			DbSet = dbSet;
		}

		/// <summary>
		/// Vrací objekt dle Id.
		/// Objekt má načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
		public TEntity GetObject(int id)
		{
			TEntity result = DbSet.Find(id);
			if (result == null)
			{
				throw new ObjectNotFoundException(String.Format("Object {0} with key {1} not found.", this.GetType().Name, id));
			}
			LoadReferences(new TEntity[] { result });
			return result;
		}

		/// <summary>
		/// Vrací objekt dle Id.
		/// Objekt má načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
		public async Task<TEntity> GetObjectAsync(int id)
		{
			TEntity result = await DbSet.FindAsync(id);
			if (result == null)
			{
				throw new ObjectNotFoundException(String.Format("Object {0} with key {1} not found.", this.GetType().Name, id));
			}
			await LoadReferencesAsync(new TEntity[] { result });
			return result;
		}

		/// <summary>
		/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
		/// Dotaz na seznam objektů provede jednou, při opakovaném volání vrací data z paměti.
		/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		public List<TEntity> GetAll()
		{			
			if (_all == null)
			{
				_all = GetGetAllQuery().ToArray();
				LoadReferences(_all);
			}
			return new List<TEntity>(_all);
		}

		/// <summary>
		/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
		/// Dotaz na seznam objektů provede jednou, při opakovaném volání vrací data z paměti.
		/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		public async Task<List<TEntity>> GetAllAsync()
		{
			if (_all == null)
			{
				_all = await GetGetAllQuery().ToArrayAsync();
				await LoadReferencesAsync(_all);
			}
			return new List<TEntity>(_all);
		}

		/// <summary>
		/// Vrací dotaz pro GetAll/GetAllAsync.
		/// </summary>
		private IQueryable<TEntity> GetGetAllQuery()
		{
			return DbSet.WhereNotDeleted(softDeleteManager);
		}

		protected void LoadReferences(TEntity[] entities)
		{
			dataLoader.LoadAll(entities, GetLoadReferences().ToArray());
		}

		protected async Task LoadReferencesAsync(TEntity[] entities)
		{	
			await dataLoaderAsync.LoadAllAsync(entities, GetLoadReferences().ToArray());
		}

		/// <summary>
		/// Vrací expressions určující, které vlastnosti budou s objektem načteny.
		/// Načítání prování DbDataLoader.
		/// </summary>
		protected virtual IEnumerable<Expression<Func<TEntity, object>>> GetLoadReferences()
		{
			return Enumerable.Empty<Expression<Func<TEntity, object>>>();
		}
	}
}