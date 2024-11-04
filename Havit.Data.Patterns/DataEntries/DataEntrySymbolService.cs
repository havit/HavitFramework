using System.Linq.Expressions;
using System.Reflection;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Exceptions;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataEntries;

/// <summary>
/// Zajišťuje mapování párovacích symbolů a identifikátorů objektů, resp. získání identifikátoru (primárního klíče) na základě symbolu.
/// </summary>
public class DataEntrySymbolService<TEntity> : IDataEntrySymbolService<TEntity>
	where TEntity : class
{
	private readonly IDataEntrySymbolStorage<TEntity> _dataEntrySymbolStorage;
	private readonly IDataSource<TEntity> _dataSource; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbDataSource.

	/// <summary>
	/// Konstructor.
	/// </summary>
	public DataEntrySymbolService(IDataEntrySymbolStorage<TEntity> dataEntrySymbolStorage, IDataSource<TEntity> dataSource)
	{
		PropertyInfo symbolProperty = typeof(TEntity).GetProperty("Symbol");
		Contract.Assert<NotSupportedException>(symbolProperty != null, String.Format("DbDataEntrySymbolService is not supported on type {0} - missing property 'Symbol'.", typeof(TEntity).Name));
		Contract.Assert<NotSupportedException>(symbolProperty.PropertyType == typeof(string), String.Format("DbDataEntrySymbolService is not supported on type {0} - property 'Symbol' must be of type string.", typeof(TEntity).Name));

		this._dataEntrySymbolStorage = dataEntrySymbolStorage;
		this._dataSource = dataSource;
	}

	/// <summary>
	/// Vrací hodnotu identifikátoru (primárního klíče) na základě symbolu.
	/// </summary>
	/// <param name="entry">"Symbol".</param>
	public int GetEntryId(Enum entry)
	{
		Dictionary<string, int> identifiersByEntry = GetIdentifiersByEntry();

		int id;
		if (identifiersByEntry.TryGetValue(entry.ToString(), out id))
		{
			return id;
		}
		else
		{
			throw new ObjectNotFoundException(String.Format("Identifier for entry {0} in {1} was not found.", entry.ToString(), typeof(TEntity).Name));
		}
	}

	private Dictionary<string, int> GetIdentifiersByEntry()
	{
		if (_dataEntrySymbolStorage.Value == null)
		{
			lock (_dataEntrySymbolStorage)
			{
				if (_dataEntrySymbolStorage.Value == null)
				{
					_dataEntrySymbolStorage.Value = GetStorageData();
				}
			}
		}
		return _dataEntrySymbolStorage.Value;
	}

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

		Dictionary<string, int> result;
		result = _dataSource.DataIncludingDeleted.Where(whereExpression).Select(projectionExpression).ToDictionary(item => item.Symbol, item => item.Id);

		return result;
	}
}
