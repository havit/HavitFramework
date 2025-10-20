using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataSources;
using Havit.Services.TimeServices;
using MockQueryable;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

/// <summary>
/// Fake implementace <see cref="IDataSource{TSource}" /> pro použití v unit testech. Jako datový zdroj používá data předané v konstruktoru.
/// </summary>
public abstract class FakeDataSource<TEntity> : IDataSource<TEntity>
	where TEntity : class
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
	/// <remarks>
	/// BuildMock v sobě obsahuje callback pro případné odstranění položky z podkladové kolekce.
	/// Z kolekce se odebírá při volání ExecuteDelete (https://github.com/romantitov/MockQueryable/blob/1ca6086df72f3e5f01979c0285e2f766212cf198/src/MockQueryable/MockQueryable.EntityFrameworkCore/TestQueryProviderEfCore.cs#L70C1-L77C14)
	/// Pro odstínění od zdrojových dat (a zamezení odebrání položky) izolujeme jednotlivá volání BuildMock na samostatná pole.	
	/// </remarks>
	public virtual IQueryable<TEntity> DataIncludingDeleted => _data.ToList().BuildMock();

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
