using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.QueryServices
{
	/// <summary>
	/// Bázový třída pro implementaci Query.
	/// Předepisuje metody Query k implementaci, která má vracet dotaz postavený nad datovým zdrojem.
	/// Poskytuje (protected) metody pro použití v potomcích pro snadnou implementaci.
	/// </summary>
	/// <typeparam name="TQueryResultItem">Typ záznamů, které vrací Query.</typeparam>
	public abstract class QueryBase<TQueryResultItem>
	{
		/// <summary>
		/// Definuje Query.
		/// </summary>
		protected abstract IQueryable<TQueryResultItem> Query();
		
		/// <summary>
		/// Vrátí všechny objekty dle Query.
		/// </summary>
		protected List<TQueryResultItem> Select()
		{
			return Query().ToList();
		}

		/// <summary>
		/// Vrátí všechny objekty dle Query.
		/// </summary>
		protected Task<List<TQueryResultItem>> SelectAsync(CancellationToken cancellationToken = default)
		{
			return Query().ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Vrátí danou stránku dané velikosti dle Query.
		/// </summary>
		protected List<TQueryResultItem> SelectPage(int pageIndex, int pageSize)
		{
			Contract.Requires<ArgumentOutOfRangeException>(pageIndex >= 0, nameof(pageIndex));
			Contract.Requires<ArgumentOutOfRangeException>(pageSize >= 0, nameof(pageSize));

			return Query().Skip(pageIndex * pageSize).Take(pageSize).ToList();
		}

		/// <summary>
		/// Vrátí danou stránku dané velikosti dle Query.
		/// </summary>
		protected Task<List<TQueryResultItem>> SelectPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentOutOfRangeException>(pageIndex >= 0, nameof(pageIndex));
			Contract.Requires<ArgumentOutOfRangeException>(pageSize >= 0, nameof(pageSize));

			return Query().Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);
		}
		
		/// <summary>
		/// Vrátí fragment dat dle Query.
		/// </summary>
		protected List<TQueryResultItem> SelectFragment(int startIndex, int? count)
		{
			Contract.Requires<ArgumentOutOfRangeException>(startIndex >= 0, nameof(startIndex));
			Contract.Requires<ArgumentOutOfRangeException>((count == null) || (count > 0), nameof(count));

			IQueryable<TQueryResultItem> query = Query();
			if (startIndex > 0)
			{
				query = query.Skip(startIndex);
			}
			if (count != null)
			{
				query = query.Take(count.Value);
			}

			return query.ToList();
		}

		/// <summary>
		/// Vrátí fragment dat dle Query.
		/// </summary>
		protected Task<List<TQueryResultItem>> SelectFragmentAsync(int startIndex, int? count, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentOutOfRangeException>(startIndex >= 0, nameof(startIndex));
			Contract.Requires<ArgumentOutOfRangeException>((count == null) || (count > 0), nameof(count));

			IQueryable<TQueryResultItem> query = Query();
			if (startIndex > 0)
			{
				query = query.Skip(startIndex);
			}
			if (count != null)
			{
				query = query.Take(count.Value);
			}

			return query.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Vrátí počet objektů odpovídajících Query.
		/// </summary>
		protected int Count()
		{
			return Query().Count();
		}

		/// <summary>
		/// Vrátí počet objektů odpovídajících Query.
		/// </summary>
		protected Task<int> CountAsync(CancellationToken cancellationToken = default)
		{
			return Query().CountAsync(cancellationToken);
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query.
		/// </exception>
		protected TQueryResultItem First()
		{
			return Query().First();
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query.
		/// </exception>
		protected Task<TQueryResultItem> FirstAsync(CancellationToken cancellationToken = default)
		{
			return Query().FirstAsync(cancellationToken);
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query. Neodpovídá-li Query žádný objekt, vrací null.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		protected TQueryResultItem FirstOrDefault()
		{
			return Query().FirstOrDefault();
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query. Neodpovídá-li Query žádný objekt, vrací null.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		protected Task<TQueryResultItem> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
		{
			return Query().FirstOrDefaultAsync(cancellationToken);
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query nebo jich odpovídá více.
		/// </exception>
		protected TQueryResultItem Single()
		{
			return Query().Single();
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query nebo jich odpovídá více.
		/// </exception>
		protected Task<TQueryResultItem> SingleAsync(CancellationToken cancellationToken = default)
		{
			return Query().SingleAsync(cancellationToken);
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query, pokud takový existuje.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Query odpovídá více objektů.
		/// </exception>
		protected TQueryResultItem SingleOrDefault()
		{
			return Query().SingleOrDefault();
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query, pokud takový existuje.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Query odpovídá více objektů.
		/// </exception>
		protected Task<TQueryResultItem> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
		{
			return Query().SingleOrDefaultAsync(cancellationToken);
		}

		/// <summary>
		/// Vrací true, pokud alespoň jeden objekt odpovídá Query, v opačném případě vrací false.
		/// </summary>
		protected bool Any()
		{
			return Query().Any();
		}

		/// <summary>
		/// Vrací true, pokud alespoň jeden objekt odpovídá Query, v opačném případě vrací false.
		/// </summary>
		protected Task<bool> AnyAsync(CancellationToken cancellationToken = default)
		{
			return Query().AnyAsync(cancellationToken);
		}
	}
}
