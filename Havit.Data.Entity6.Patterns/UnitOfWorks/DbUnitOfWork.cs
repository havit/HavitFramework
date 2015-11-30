using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.UnitOfWorks
{
	/// <summary>
	/// Unit of Work postavená nad <see cref="DbContext" />.
	/// </summary>
	public class DbUnitOfWork : IUnitOfWork, IUnitOfWorkAsync
	{
		private readonly ISoftDeleteManager softDeleteManager;

		private bool commited = false;

		/// <summary>
		/// DbContext, nad kterým stojí Unit of Work.
		/// </summary>
		protected IDbContext DbContext { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbUnitOfWork(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
		{
			this.softDeleteManager = softDeleteManager;
			Contract.Assert(dbContext != null);
			this.DbContext = dbContext;			
		}

		/// <summary>
		/// Uloží změny registrované v Unit of Work.
		/// </summary>
		public void Commit()
		{
			VerifyNotCommited();
			DbContext.SaveChanges();
			commited = true;
		}

		/// <summary>
		/// Asynchronně uloží změny registrované v Unit of Work.
		/// </summary>
		public async Task CommitAsync()
		{
			VerifyNotCommited();
			await DbContext.SaveChangesAsync();
			commited = true;
		}

		/// <summary>
		/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
		/// </summary>
		public void AddForInsert<TEntity>(TEntity item)
			where TEntity : class
		{
			VerifyNotCommited();

			GetDbSet<TEntity>().Add(item);
		}

		/// <summary>
		/// Zajistí vložení objeků jako nové objekty (při uložení budou vloženy).
		/// </summary>
		public void AddRangeForInsert<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class
		{
			VerifyNotCommited();

			GetDbSet<TEntity>().AddRange(items);
		}

		/// <summary>
		/// Zajistí odstranění objeku (při uložení bude smazán).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		public void AddForDelete<TEntity>(TEntity item)
			where TEntity : class
		{
			VerifyNotCommited();

			if (softDeleteManager.IsSoftDeleteSupported<TEntity>())
			{
				softDeleteManager.SetDeleted(item);
			}
			else
			{
				GetDbSet<TEntity>().Remove(item);
			}
		}

		/// <summary>
		/// Zajistí odstranění objeků (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		public void AddRangeForDelete<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class
		{
			VerifyNotCommited();

			if (softDeleteManager.IsSoftDeleteSupported<TEntity>())
			{
				foreach (TEntity item in items)
				{
					softDeleteManager.SetDeleted<TEntity>(item);
				}
			}
			else
			{
				GetDbSet<TEntity>().RemoveRange(items);
			}
		}

		/// <summary>
		/// Vrátí DbSet pro daný typ.
		/// Pokud není nalezen, vyhazuje výjimku NotSupportedException.
		/// </summary>
		private DbSet GetDbSet<TEntity>()
			where TEntity : class
		{
			DbSet<TEntity> dbSet = DbContext.Set<TEntity>();
			Contract.Assert<NotSupportedException>(dbSet != null, $"{typeof(TEntity).FullName} type is not supported.");
			return dbSet;
		}

		private void VerifyNotCommited()
		{
			if (commited)
			{
				throw new InvalidOperationException("UnitOfWork has been already commited.");
			}
		}
	}
}
