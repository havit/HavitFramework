using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataSources;
using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

/// <summary>
/// Fake implementace <see cref="IDataSource{TSource}" /> pro použití v unit testech. Jako datový zdroj používá data předané v konstruktoru.
/// </summary>
public abstract class FakeDataSource<TEntity> : IDataSource<TEntity>
{
	private readonly ISoftDeleteManager _softDeleteManager;
	private readonly TEntity[] _data;

	/// <summary>
	/// Data z datového zdroje jako IQueryable.
	/// </summary>
	public virtual IQueryable<TEntity> Data => DataIncludingDeleted.WhereNotDeleted(_softDeleteManager);

	/// <summary>
	/// Data z datového zdroje jako IQueryable.
	/// </summary>
	public virtual IQueryable<TEntity> DataIncludingDeleted => new MockQueryable.EntityFrameworkCore.TestAsyncEnumerableEfCore<TEntity>(_data);

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="data">Data, která budou intancí vracena.</param>
	protected FakeDataSource(params TEntity[] data)
		: this(data, null)
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="data">Data, která budou intancí vracena.</param>
	/// <param name="softDeleteManager">Pro podporu mazání příznakem.</param>
	protected FakeDataSource(IEnumerable<TEntity> data, ISoftDeleteManager softDeleteManager = null)
		: this(data.ToArray(), softDeleteManager)
	{
	}

	private FakeDataSource(TEntity[] data, ISoftDeleteManager softDeleteManager = null)
	{
		ArgumentNullException.ThrowIfNull(data);

		_softDeleteManager = softDeleteManager ?? new SoftDeleteManager(new ServerTimeService());
		_data = data.ToArray();
	}
}
