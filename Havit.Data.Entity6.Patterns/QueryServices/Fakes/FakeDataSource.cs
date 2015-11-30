using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.QueryServices;
using Havit.Diagnostics.Contracts;
using Havit.Services.TimeServices;

namespace Havit.Data.Entity.Patterns.QueryServices.Fakes
{
	/// <summary>
	/// Fake implementace <see cref="IDataSource{TEntity}" /> pro použití v unit testech. Jako datový zdroj používá data předané v konstruktoru.
	/// </summary>
	public abstract class FakeDataSource<TEntity> : IDataSource<TEntity>
	{
		private readonly ISoftDeleteManager softDeleteManager;
		private readonly TEntity[] data;

		/// <summary>
		/// Indikuje, zda mají být vráceny i (příznakem) smazané záznamy. Výchozí hodnota je false.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// IncludeDeleted je nastaveno na true, ale na datovém typu není podporováno mazání příznakem.
		/// </exception>
		public bool IncludeDeleted
		{
			get
			{
				return _includeDeleted;
			}
			set
			{
				if (value && !softDeleteManager.IsSoftDeleteSupported<TEntity>())
				{
					throw new InvalidOperationException("Nastavení IncludeDeleted na true na tomto typu není podporován.");
				}

				_includeDeleted = value;
			}
		}
		private bool _includeDeleted = false;

		/// <summary>
		/// Data z datového zdroje jako IQueryable.
		/// </summary>
		public virtual IQueryable<TEntity> Data
		{
			get
			{
				if (!this.IncludeDeleted && softDeleteManager.IsSoftDeleteSupported<TEntity>())
				{
					return new InternalFakeDbAsyncEnumerable<TEntity>(data.Where(softDeleteManager.GetNotDeletedExpression<TEntity>().Compile()).ToList());
				}
				else
				{
					return new InternalFakeDbAsyncEnumerable<TEntity>(data);
				}
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

			this.IncludeDeleted = false;
		}
	}
}
