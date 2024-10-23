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
	public virtual IQueryable<TEntity> Data => new FakeAsyncEnumerable<TEntity>(_data.AsQueryable().WhereNotDeleted(_softDeleteManager).ToList());

	/// <summary>
	/// Data z datového zdroje jako IQueryable.
	/// </summary>
	public virtual IQueryable<TEntity> DataIncludingDeleted => new FakeAsyncEnumerable<TEntity>(_data);

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="data">Data, která budou intancí vracena.</param>
	protected FakeDataSource(params TEntity[] data)
		: this(data.AsEnumerable(), null)
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="data">Data, která budou intancí vracena.</param>
	/// <param name="softDeleteManager">Pro podporu mazání příznakem.</param>
	protected FakeDataSource(IEnumerable<TEntity> data, ISoftDeleteManager softDeleteManager = null)
	{
		ArgumentNullException.ThrowIfNull(data);

		_softDeleteManager = softDeleteManager ?? new SoftDeleteManager(new ServerTimeService());
		_data = data.ToArray();
	}
}
