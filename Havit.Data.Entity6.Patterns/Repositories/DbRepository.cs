using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.XPath;
using Havit.Data.Entity.Patterns.Infrastructure;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
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
		private readonly IDbContext dbContext;
		private readonly IDataSource<TEntity> dataSource;
		private readonly IDataLoader dataLoader;
		private readonly IDataLoaderAsync dataLoaderAsync;
		private readonly IEntityKeyAccessor<TEntity, int> entityKeyAccessor = new EntityKeyAccessor<TEntity>();

		/// <summary>
		/// Přístup k lokálním objektům v dictionary v konstatním čase.
		/// Dictionary je vytvořena až při použití této vlastnosti, od té doby je též udržována.
		/// </summary>
		protected internal Dictionary<int, TEntity> DbSetLocalsDictionary
		{
			get
			{
				if (_dbSetLocalsDictionary == null)
				{
					ObservableCollection<TEntity> dbSetLocal = dbContext.ExecuteWithoutAutoDetectChanges(() => DbSet.Local); // každé šahnutí na DbSet.Local způsobuje spuštění changetrackeru
					if (!_dbSetLocalsDictionaryInitialized)
					{
						dbSetLocal.CollectionChanged += DbSetLocal_CollectionChanged;
						dbContext.RegisterAfterSaveChangesAction(DbContext_AfterSaveChangesAction);
						_dbSetLocalsDictionaryInitialized = true;
					}

					_dbSetLocalsDictionary = dbSetLocal.Where(EntityNotInAddedState).ToDictionary(entity => entityKeyAccessor.GetEntityKey(entity));
				}
				return _dbSetLocalsDictionary;
			}
		}
		private Dictionary<int, TEntity> _dbSetLocalsDictionary;
		private bool _dbSetLocalsDictionaryInitialized = false;

		/// <summary>
		/// DbSet, nad kterým je DbRepository postaven.
		/// </summary>
		protected DbSet<TEntity> DbSet => _dbSet.Value;
		
		/// <summary>
		/// Implementačně jako Lazy, aby kontruktor nevyzvedával DbSet. To umožňuje psát unit testy s mockem dbContextu bez setupu metody Set (dbContext nemusí nic umět).
		/// </summary>
		private readonly Lazy<DbSet<TEntity>> _dbSet;

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
		/// </summary>
		protected IQueryable<TEntity> Data => dataSource.Data;

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
		/// </summary>
		protected IQueryable<TEntity> DataWithDeleted => dataSource.DataWithDeleted;

		/// <summary>
		/// SoftDeleteManager používaný repository.
		/// </summary>
		protected ISoftDeleteManager SoftDeleteManager { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected DbRepository(IDbContext dbContext, IDataSource<TEntity> dataSource, IDataLoader dataLoader, IDataLoaderAsync dataLoaderAsync, ISoftDeleteManager softDeleteManager)
		{
			Contract.Requires<ArgumentException>(dbContext != null);
			Contract.Requires<ArgumentException>(dataSource != null);
			Contract.Requires<ArgumentException>(softDeleteManager != null);

			this.dbContext = dbContext;
			this.dataSource = dataSource;
			this.dataLoader = dataLoader;
			this.dataLoaderAsync = dataLoaderAsync;
			this.SoftDeleteManager = softDeleteManager;
			this._dbSet = new Lazy<DbSet<TEntity>>(() => dbContext.Set<TEntity>());
		}

		private void DbSetLocal_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// pokud jsme ještě nepotřebovali kolekci DbSetLocalsDictionary, nemusíme nic dělat - kolekce se inicializuje z aktuálních dat, až bude potřeba
			if (_dbSetLocalsDictionary != null)
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						if (_dbSetLocalsDictionary != null)
						{							
							e.NewItems.Cast<TEntity>().Where(EntityNotInAddedState).ToList().ForEach(entity => _dbSetLocalsDictionary.Add(entityKeyAccessor.GetEntityKey(entity), entity));
						}
						break;

					case NotifyCollectionChangedAction.Move:
						// NOOP
						break;

					case NotifyCollectionChangedAction.Remove:
						foreach (TEntity item in e.OldItems)
						{
							_dbSetLocalsDictionary.Remove(entityKeyAccessor.GetEntityKey(item));
						}

						break;

					case NotifyCollectionChangedAction.Replace:
						throw new NotSupportedException(e.Action.ToString());

					case NotifyCollectionChangedAction.Reset:
						_dbSetLocalsDictionary.Clear();
						break;

					default:
						throw new NotSupportedException(e.Action.ToString());
				}
			}
		}

		private void DbContext_AfterSaveChangesAction()
		{
			_dbSetLocalsDictionary = null;
		}

		private bool EntityNotInAddedState(TEntity entity)
		{
			return entityKeyAccessor.GetEntityKey(entity) != default(int);
		}

		/// <summary>
		/// Vrací objekt dle Id.
		/// Objekt má načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
		public TEntity GetObject(int id)
		{
			Contract.Requires<ArgumentException>(id != default(int));

			TEntity result = dbContext.ExecuteWithoutAutoDetectChanges(() => DbSet.Find(id));
			if (result == null)
			{
				ThrowObjectNotFoundException(id);
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
			Contract.Requires<ArgumentException>(id != default(int));

			TEntity result = await dbContext.ExecuteWithoutAutoDetectChanges(() => DbSet.FindAsync(id));
			if (result == null)
			{
				ThrowObjectNotFoundException(id);					
			}

			await LoadReferencesAsync(new TEntity[] { result });
			return result;
		}

		/// <summary>
		/// Vrací instance objektů dle Id.
		/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Alespoň jeden objekt nebyl nalezen.</exception>
		public List<TEntity> GetObjects(params int[] ids)
		{			
			Contract.Requires(ids != null);

			HashSet<TEntity> loadedEntities = new HashSet<TEntity>();
			HashSet<int> idsToLoad = new HashSet<int>();

			foreach (int id in ids)
			{
				Contract.Assert<ArgumentException>(id != default(int));

				if (DbSetLocalsDictionary.TryGetValue(id, out TEntity loadedEntity))
				{
					loadedEntities.Add(loadedEntity);
				}
				else
				{
					idsToLoad.Add(id);
				}
			}

			List<TEntity> result = new List<TEntity>(ids.Length);
			result.AddRange(loadedEntities);

			if (idsToLoad.Count > 0)
			{
				var query = GetInQuery(idsToLoad.ToArray());
				var loadedObjects = query.ToList();

				if (idsToLoad.Count != loadedObjects.Count)
				{
					int[] missingObjectIds = idsToLoad.Except(loadedObjects.Select(entityKeyAccessor.GetEntityKey)).ToArray();
					ThrowObjectNotFoundException(missingObjectIds);					
				}

				result.AddRange(loadedObjects);
			}

			LoadReferences(result.ToArray());
			return result;
		}

		/// <summary>
		/// Vrací instance objektů dle Id.
		/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Alespoň jeden objekt nebyl nalezen.</exception>
		public async Task<List<TEntity>> GetObjectsAsync(params int[] ids)
		{
			Contract.Requires(ids != null);

			HashSet<TEntity> loadedEntities = new HashSet<TEntity>();
			HashSet<int> idsToLoad = new HashSet<int>();

			foreach (int id in ids)
			{
				Contract.Assert<ArgumentException>(id != default(int));

				if (DbSetLocalsDictionary.TryGetValue(id, out TEntity loadedEntity))
				{
					loadedEntities.Add(loadedEntity);
				}
				else
				{
					idsToLoad.Add(id);
				}
			}

			List<TEntity> result = new List<TEntity>(ids.Length);
			result.AddRange(loadedEntities);

			if (idsToLoad.Count > 0)
			{
				var query = GetInQuery(idsToLoad.ToArray());
				var loadedObjects = await query.ToListAsync();

				if (idsToLoad.Count != loadedObjects.Count)
				{
					int[] missingObjectIds = idsToLoad.Except(loadedObjects.Select(entityKeyAccessor.GetEntityKey)).ToArray();
					ThrowObjectNotFoundException(missingObjectIds);
				}

				result.AddRange(loadedObjects);
			}

			await LoadReferencesAsync(result.ToArray());
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
				_all = Data.ToArray();
				LoadReferences(_all);

				if (!_allInitialized)
				{
					dbContext.RegisterAfterSaveChangesAction(() =>
					{
						_all = null;
					});
					_allInitialized = true;
				}
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
				_all = await Data.ToArrayAsync();
				await LoadReferencesAsync(_all);

				if (!_allInitialized)
				{
					dbContext.RegisterAfterSaveChangesAction(() =>
					{
						_all = null;
					});
					_allInitialized = true;
				}
			}
			return new List<TEntity>(_all);
		}

		private TEntity[] _all;
		private bool _allInitialized;

		/// <summary>
		/// Vrací dotaz pro GetObjects/GetObjectsAsync.
		/// </summary>
		private IQueryable<TEntity> GetInQuery(int[] ids)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
			Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(
					Expression.Call(
					null,
					typeof(System.Linq.Enumerable)
							.GetMethods(BindingFlags.Public | BindingFlags.Static)
							 .Where(m => m.Name == "Contains")
							 .Select(m => new
								 {
									 Method = m,
									 Params = m.GetParameters(),
									 Args = m.GetGenericArguments()
								 })
							 .Where(x => x.Params.Length == 2)
							 .Select(x => x.Method)
							 .First()
							.MakeGenericMethod(typeof(int)),
					Expression.Constant(ids),
					Expression.Property(parameter, typeof(TEntity), "Id")), 
				parameter);
			return DbSet.Where(expression);
		}

		/// <summary>
		/// Zajistí načtení vlastností definovaných v meodě GetLoadReferences.
		/// </summary>
		protected void LoadReferences(params TEntity[] entities)
		{
			Contract.Requires(entities != null);

			dataLoader.LoadAll(entities, GetLoadReferences().ToArray());
		}

		/// <summary>
		/// Zajistí načtení vlastností definovaných v meodě GetLoadReferences.
		/// </summary>
		protected async Task LoadReferencesAsync(params TEntity[] entities)
		{	
			Contract.Requires(entities != null);

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

		private void ThrowObjectNotFoundException(params int[] missingIds)
		{
			Contract.Requires(missingIds != null);
			Contract.Requires(missingIds.Length > 0);

			string exceptionText = (missingIds.Length == 1)
				? String.Format("Object {0} with key {1} not found.", typeof(TEntity).Name, missingIds[0])
				: String.Format("Objects {0} with keys {1} not found.", typeof(TEntity).Name, String.Join(", ", missingIds.Select(item => item.ToString())));

			throw new Havit.Data.Patterns.Exceptions.ObjectNotFoundException(exceptionText);
		}
	}
}