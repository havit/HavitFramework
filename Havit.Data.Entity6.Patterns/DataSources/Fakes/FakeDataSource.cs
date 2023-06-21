using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataSources;
using Havit.Diagnostics.Contracts;
using Havit.Services.TimeServices;

namespace Havit.Data.Entity.Patterns.DataSources.Fakes;

/// <summary>
/// Fake implementace <see cref="IDataSource{TSource}" /> pro použití v unit testech. Jako datový zdroj používá data předané v konstruktoru.
/// </summary>
public abstract class FakeDataSource<TEntity> : IDataSource<TEntity>
{
	private readonly ISoftDeleteManager softDeleteManager;
	private readonly TEntity[] data;

	/// <summary>
	/// Data z datového zdroje jako IQueryable.
	/// </summary>
	public virtual IQueryable<TEntity> Data
	{
		get
		{
			return new InternalFakeDbAsyncEnumerable<TEntity>(data.AsQueryable().WhereNotDeleted(softDeleteManager).ToList());
		}
	}

	/// <summary>
	/// Data z datového zdroje jako IQueryable.
	/// </summary>
	public virtual IQueryable<TEntity> DataIncludingDeleted
	{
		get
		{
			return new InternalFakeDbAsyncEnumerable<TEntity>(data);
		}
	}

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
		this.softDeleteManager = softDeleteManager ?? new SoftDeleteManager(new ServerTimeService());
		Contract.Requires(data != null);

		this.data = data.ToArray();
	}
}
