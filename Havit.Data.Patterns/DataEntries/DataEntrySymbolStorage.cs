using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Data.Patterns.Exceptions;
using Havit.Data.Patterns.QueryServices;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataEntries
{
	/// <summary>
	/// Zajišťuje mapování párovacích symbolů a identifikátorů objektů, resp. získání identifikátoru (primárního klíče) na základě symbolu.
	/// </summary>
	public class DataEntrySymbolStorage<TEntity> : IDataEntrySymbolStorage<TEntity>
		where TEntity : class
	{
		private readonly IDataSourceFactory<TEntity> dataSourceFactory;
		private Dictionary<string, int> storage;

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dataSourceFactory">Factory pro získání a uvolnění IDataSource.</param>
		public DataEntrySymbolStorage(IDataSourceFactory<TEntity> dataSourceFactory)
		{
			Contract.Requires(dataSourceFactory != null);

			PropertyInfo symbolProperty = typeof(TEntity).GetProperty("Symbol");

			Contract.Assert<NotSupportedException>(symbolProperty != null, String.Format("DbDataEntrySymbolStorage is not supported on type {0} - missing property 'Symbol'.", typeof(TEntity).Name));
			Contract.Assert<NotSupportedException>(symbolProperty.PropertyType == typeof(string), String.Format("DbDataEntrySymbolStorage is not supported on type {0} - property 'Symbol' must be of type string.", typeof(TEntity).Name));

			this.dataSourceFactory = dataSourceFactory;
		}

		/// <summary>
		/// Vrací hodnotu identifikátoru (primárního klíče) na základě symbolu.
		/// </summary>
		/// <param name="entry">"Symbol".</param>
		public int GetEntryId(Enum entry)
		{
			EnsureStorage();

			int id;
			if (storage.TryGetValue(entry.ToString(), out id))
			{
				return id;
			}
			else
			{
				throw new ObjectNotFoundException(String.Format("Identifier for entry {0} in {1} was not found.", entry.ToString(), typeof(TEntity).Name));
			}
		}

		private void EnsureStorage()
		{
			if (storage == null)
			{
				lock (_ensureStorageLock)
				{
					if (storage == null)
					{
						storage = GetStorageData();						
					}
				}
			}
		}
		private readonly object _ensureStorageLock = new object();

		private Dictionary<string, int> GetStorageData()
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");

			// item => !String.IsNullOrEmpty(item.Symbol)
			Expression<Func<TEntity, bool>> whereExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.Not(Expression.Call(null, typeof(String).GetMethod(nameof(String.IsNullOrEmpty)), Expression.Property(parameter, "Symbol"))), parameter);

			// item => new EntryIdentification { Id = item.Id, Symbol = item.Symbol }
			Expression<Func<TEntity, EntryIdentification>> projectionExpression = (Expression<Func<TEntity, EntryIdentification>>)Expression.Lambda(
				Expression.MemberInit(
					Expression.New(typeof(EntryIdentification)),
					Expression.Bind(typeof(EntryIdentification).GetProperty("Id"), Expression.Property(parameter, "Id")),
					Expression.Bind(typeof(EntryIdentification).GetProperty("Symbol"), Expression.Property(parameter, "Symbol"))
				),
				parameter);

			IDataSource<TEntity> dataSource = dataSourceFactory.Create();

			Dictionary<string, int> result;
			try
			{
				result = dataSource.DataWithDeleted.Where(whereExpression).Select(projectionExpression).ToDictionary(item => item.Symbol, item => item.Id);
			}
			finally
			{
				dataSourceFactory.Release(dataSource);
			}

			return result;
		}
	}
}
