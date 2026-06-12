using System.Linq.Expressions;
using System.Reflection;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Exceptions;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataEntries;

/// <summary>
/// Zajišťuje mapování párovacích symbolů a identifikátorů objektů, resp. získání identifikátoru (primárního klíče) na základě symbolu.
/// </summary>
public class DataEntrySymbolService<TEntity, TKey> : IDataEntrySymbolService<TEntity, TKey>
	where TEntity : class
{
	private readonly IDataEntrySymbolStorage<TEntity, TKey> _dataEntrySymbolStorage;
	private readonly IDataSource<TEntity> _dataSource; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbDataSource.

	// PERF: Příznak úspěšně provedené validace typu TEntity (per uzavřený generický typ).
	private static bool s_entityTypeValidated;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DataEntrySymbolService(IDataEntrySymbolStorage<TEntity, TKey> dataEntrySymbolStorage, IDataSource<TEntity> dataSource)
	{
		// PERF: Služba bývá registrována jako transientní, validace (reflexe + String.Format hlášek i v úspěšné větvi) by tak probíhala při každé konstrukci.
		// Výsledek validace závisí jen na typu TEntity, proto ji provádíme jen jednou per uzavřený generický typ.
		// (Případný souběh vláken při prvních konstrukcích není problém, validace je idempotentní.)
		if (!s_entityTypeValidated)
		{
			ValidateEntityType();
			s_entityTypeValidated = true;
		}

		this._dataEntrySymbolStorage = dataEntrySymbolStorage;
		this._dataSource = dataSource;
	}

	/// <summary>
	/// Ověří, že typ TEntity je podporován (má vlastnost Symbol typu string).
	/// </summary>
	/// <exception cref="NotSupportedException">Typ TEntity není podporován.</exception>
	private static void ValidateEntityType()
	{
		PropertyInfo symbolProperty = typeof(TEntity).GetProperty("Symbol");
		Contract.Assert<NotSupportedException>(symbolProperty != null, String.Format("DataEntrySymbolService is not supported on type {0} - missing property 'Symbol'.", typeof(TEntity).Name));
		Contract.Assert<NotSupportedException>(symbolProperty.PropertyType == typeof(string), String.Format("DbDataEntrySymbolService is not supported on type {0} - property 'Symbol' must be of type string.", typeof(TEntity).Name));
	}

	/// <summary>
	/// Vrací hodnotu identifikátoru (primárního klíče) na základě symbolu.
	/// </summary>
	/// <param name="entry">"Symbol".</param>
	public TKey GetEntryId(Enum entry)
	{
		Dictionary<string, TKey> identifiersByEntry = GetIdentifiersByEntry();
		return GetEntryIdFromDictionary(identifiersByEntry, entry);
	}

	/// <summary>
	/// Vrací hodnotu identifikátoru (primárního klíče) na základě symbolu.
	/// </summary>
	/// <param name="entry">"Symbol".</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async ValueTask<TKey> GetEntryIdAsync(Enum entry, CancellationToken cancellationToken = default)
	{
		Dictionary<string, TKey> identifiersByEntry = await GetIdentifiersByEntryAsync(cancellationToken).ConfigureAwait(false);
		return GetEntryIdFromDictionary(identifiersByEntry, entry);
	}

	private static TKey GetEntryIdFromDictionary(Dictionary<string, TKey> identifiersByEntry, Enum entry)
	{
		TKey id;
		if (identifiersByEntry.TryGetValue(entry.ToString(), out id))
		{
			return id;
		}
		else
		{
			throw new ObjectNotFoundException(String.Format("Identifier for entry {0} in {1} was not found.", entry.ToString(), typeof(TEntity).Name));
		}
	}

	private Dictionary<string, TKey> GetIdentifiersByEntry()
	{
		Dictionary<string, TKey> identifiersByEntry = _dataEntrySymbolStorage.Value;
		if (identifiersByEntry == null)
		{
			lock (_dataEntrySymbolStorage)
			{
				identifiersByEntry = _dataEntrySymbolStorage.Value;
				if (identifiersByEntry == null)
				{
					identifiersByEntry = GetStorageData();
					_dataEntrySymbolStorage.Value = identifiersByEntry;
				}
			}
		}
		return identifiersByEntry;
	}

	private async ValueTask<Dictionary<string, TKey>> GetIdentifiersByEntryAsync(CancellationToken cancellationToken)
	{
		Dictionary<string, TKey> identifiersByEntry = _dataEntrySymbolStorage.Value;
		if (identifiersByEntry == null)
		{
			// Zámek nelze držet přes await, data proto načítáme mimo zámek.
			// Při souběhu prvních volání tak může dojít k opakovanému (idempotentnímu) načtení dat, použije se první zapsaný výsledek.
			Dictionary<string, TKey> storageData = await GetStorageDataAsync(cancellationToken).ConfigureAwait(false);
			lock (_dataEntrySymbolStorage)
			{
				identifiersByEntry = _dataEntrySymbolStorage.Value;
				if (identifiersByEntry == null)
				{
					_dataEntrySymbolStorage.Value = storageData;
					identifiersByEntry = storageData;
				}
			}
		}
		return identifiersByEntry;
	}

	private Dictionary<string, TKey> GetStorageData()
	{
		return GetStorageDataQuery().ToDictionary(item => item.Symbol, item => item.Id);
	}

	private async ValueTask<Dictionary<string, TKey>> GetStorageDataAsync(CancellationToken cancellationToken)
	{
		IQueryable<EntryIdentification<TKey>> query = GetStorageDataQuery();
		if (query is IAsyncEnumerable<EntryIdentification<TKey>> asyncQuery)
		{
			Dictionary<string, TKey> result = new Dictionary<string, TKey>();
			await foreach (EntryIdentification<TKey> item in asyncQuery.WithCancellation(cancellationToken).ConfigureAwait(false))
			{
				result.Add(item.Symbol, item.Id);
			}
			return result;
		}
		else
		{
			// Datový zdroj nepodporuje asynchronní enumeraci (typicky EF6 či fake pro testy), data načteme synchronně.
			return query.ToDictionary(item => item.Symbol, item => item.Id);
		}
	}

	private IQueryable<EntryIdentification<TKey>> GetStorageDataQuery()
	{
		ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");

		// item => !String.IsNullOrEmpty(item.Symbol)
		Expression<Func<TEntity, bool>> whereExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.Not(Expression.Call(null, typeof(String).GetMethod(nameof(String.IsNullOrEmpty)), Expression.Property(parameter, "Symbol"))), parameter);

		// item => new EntryIdentification { Id = item.Id, Symbol = item.Symbol }
		Expression<Func<TEntity, EntryIdentification<TKey>>> projectionExpression = (Expression<Func<TEntity, EntryIdentification<TKey>>>)Expression.Lambda(
			Expression.MemberInit(
				Expression.New(typeof(EntryIdentification<TKey>)),
				Expression.Bind(typeof(EntryIdentification<TKey>).GetProperty("Id"), Expression.Property(parameter, "Id")),
				Expression.Bind(typeof(EntryIdentification<TKey>).GetProperty("Symbol"), Expression.Property(parameter, "Symbol"))
			),
			parameter);

		return _dataSource.DataIncludingDeleted.Where(whereExpression).Select(projectionExpression);
	}
}
