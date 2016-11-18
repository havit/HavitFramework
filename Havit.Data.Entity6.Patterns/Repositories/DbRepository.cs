using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.XPath;
using Havit.Data.Entity.Patterns.Helpers;
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
		private readonly IDbContext dbContext;
		private readonly IDataLoader dataLoader;
		private readonly IDataLoaderAsync dataLoaderAsync;

		private TEntity[] _all;

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
					if (!_dbSetLocalsDictionaryInitialized)
					{
						DbSet.Local.CollectionChanged += DbSetLocal_CollectionChanged;
						dbContext.RegisterAfterSaveChangesAction(DbContext_AfterSaveChangesAction);
						_dbSetLocalsDictionaryInitialized = true;
					}

					_dbSetLocalsDictionary = DbSet.Local.Where(EntityNotInAddedState).ToDictionary(entity => EntityHelper.GetEntityId(entity));
				}
				return _dbSetLocalsDictionary;
			}
		}
		private Dictionary<int, TEntity> _dbSetLocalsDictionary;
		private bool _dbSetLocalsDictionaryInitialized = false;

		/// <summary>
		/// DbSet, nad kterým je DbRepository postaven.
		/// </summary>
		protected DbSet<TEntity> DbSet { get; private set; }

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
		/// </summary>
		protected IQueryable<TEntity> Data
		{
			get { return DbSet.WhereNotDeleted(this.SoftDeleteManager); }
		}

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
		/// </summary>
		protected IQueryable<TEntity> DataWithDeleted
		{
			get { return DbSet.AsQueryable(); }
		}

		/// <summary>
		/// SoftDeleteManager používaný repository.
		/// </summary>
		protected ISoftDeleteManager SoftDeleteManager { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected DbRepository(IDbContext dbContext, IDataLoader dataLoader, IDataLoaderAsync dataLoaderAsync, ISoftDeleteManager softDeleteManager)
		{
			Contract.Requires<ArgumentException>(dbContext != null);
			Contract.Requires<ArgumentException>(softDeleteManager != null);

			DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

			this.dbContext = dbContext;
			this.dataLoader = dataLoader;
			this.dataLoaderAsync = dataLoaderAsync;
			this.SoftDeleteManager = softDeleteManager;
			this.DbSet = dbSet;
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
							e.NewItems.Cast<TEntity>().Where(EntityNotInAddedState).ToList().ForEach(entity => _dbSetLocalsDictionary.Add(EntityHelper.GetEntityId(entity), entity));
						}
						break;

					case NotifyCollectionChangedAction.Move:
						// NOOP
						break;

					case NotifyCollectionChangedAction.Remove:
						foreach (TEntity item in e.OldItems)
						{
							_dbSetLocalsDictionary.Remove(EntityHelper.GetEntityId(item));
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
			return EntityHelper.GetEntityId(entity) != default(int);
		}

		/// <summary>
		/// Vrací objekt dle Id.
		/// Objekt má načtené vlastnosti definované v metodě GetLoadReferences. 
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
		public TEntity GetObject(int id)
		{
			Contract.Requires<ArgumentException>(id != default(int));

			TEntity result = DbSet.Find(id);
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

			TEntity result = await DbSet.FindAsync(id);
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

			List<TEntity> result = new List<TEntity>();
			List<int> idsToLoad = new List<int>();

			foreach (int id in ids)
			{
				Contract.Assert<ArgumentException>(id != default(int));

				TEntity loadedEntity = DbSet.Local.FirstOrDefault(item => EntityHelper.GetEntityId(item) == id);
				if (loadedEntity != null)
				{
					result.Add(loadedEntity);
				}
				else
				{
					idsToLoad.Add(id);
				}
			}

			if (idsToLoad.Count > 0)
			{
				var query = GetInQuery(idsToLoad.ToArray());
				var loadedObjects = query.ToList();

				if (idsToLoad.Count != loadedObjects.Count)
				{
					int[] missingObjectIds = idsToLoad.Except(loadedObjects.Select(EntityHelper.GetEntityId)).ToArray();
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

			List<TEntity> result = new List<TEntity>();
			List<int> idsToLoad = new List<int>();

			foreach (int id in ids)
			{
				Contract.Assert<ArgumentException>(id != default(int));

				TEntity loadedEntity = DbSet.Local.FirstOrDefault(item => EntityHelper.GetEntityId(item) == id);
				if (loadedEntity != null)
				{
					result.Add(loadedEntity);
				}
				else
				{
					idsToLoad.Add(id);
				}
			}

			if (idsToLoad.Count > 0)
			{
				var query = GetInQuery(idsToLoad.ToArray());
				var loadedObjects = await query.ToListAsync();

				if (idsToLoad.Count != loadedObjects.Count)
				{
					int[] missingObjectIds = idsToLoad.Except(loadedObjects.Select(EntityHelper.GetEntityId)).ToArray();
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
			}
			return new List<TEntity>(_all);
		}

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