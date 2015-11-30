using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.QueryServices
{
	/// <summary>
	/// Bázový třída pro implementaci Query.
	/// Předepisuje metody Query k implementaci, která má vracet dotaz postavený nad datovým zdrojem.
	/// Poskytuje (protected) metody pro použití v potomcích pro snadnou implementaci.
	/// </summary>
	/// <typeparam name="TQueryResult">Typ, který je vracen Query.</typeparam>
	public abstract class QueryBase<TQueryResult>
		where TQueryResult : class
	{
		/// <summary>
		/// Definuje Query.
		/// </summary>
		protected abstract IQueryable<TQueryResult> Query();
		
		/// <summary>
		/// Vrátí všechny objekty dle Query.
		/// </summary>
		protected List<TQueryResult> Select()
		{
			return Query().ToList();
		}

		/// <summary>
		/// Vrátí všechny objekty dle Query.
		/// </summary>
		protected Task<List<TQueryResult>> SelectAsync()
		{
			return Query().ToListAsync();
		}

		/// <summary>
		/// Vrátí danou stránku dané velikosti dle Query.
		/// </summary>
		protected List<TQueryResult> SelectPage(int pageIndex, int pageSize)
		{
			return Query().Skip(pageIndex * pageSize).Take(pageSize).ToList();
		}

		/// <summary>
		/// Vrátí danou stránku dané velikosti dle Query.
		/// </summary>
		protected Task<List<TQueryResult>> SelectPageAsync(int pageIndex, int pageSize)
		{
			return Query().Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
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
		protected Task<int> CountAsync()
		{
			return Query().CountAsync();
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query.
		/// </exception>
		protected TQueryResult First()
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
		protected Task<TQueryResult> FirstAsync()
		{
			return Query().FirstAsync();
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query. Neodpovídá-li Query žádný objekt, vrací null.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		protected TQueryResult FirstOrDefault()
		{
			return Query().FirstOrDefault();
		}

		/// <summary>
		/// Vrátí první objekt odpovídající Query. Neodpovídá-li Query žádný objekt, vrací null.
		/// Doporučeno je, aby Query definovalo pořadí.
		/// </summary>
		protected Task<TQueryResult> FirstOrDefaultAsync()
		{
			return Query().FirstOrDefaultAsync();
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query nebo jich odpovídá více.
		/// </exception>
		protected TQueryResult Single()
		{
			return Query().Single();
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Žádný objekt neodpovídá Query nebo jich odpovídá více.
		/// </exception>
		protected Task<TQueryResult> SingleAsync()
		{
			return Query().SingleAsync();
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query, pokud takový existuje.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Query odpovídá více objektů.
		/// </exception>
		protected TQueryResult SingleOrDefault()
		{
			return Query().SingleOrDefault();
		}

		/// <summary>
		/// Vrátí první a jediný objekt odpovídající Query, pokud takový existuje.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Query odpovídá více objektů.
		/// </exception>
		protected Task<TQueryResult> SingleOrDefaultAsync()
		{
			return Query().SingleOrDefaultAsync();
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
		protected Task<bool> AnyAsync()
		{
			return Query().AnyAsync();
		}
	}
}
