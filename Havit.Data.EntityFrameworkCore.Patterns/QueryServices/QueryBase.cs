using Microsoft.EntityFrameworkCore;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.QueryServices;

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
	protected internal abstract IQueryable<TQueryResultItem> Query();

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
	[Obsolete($"Replaced by {nameof(GetPage)} method which contains optimized use of {nameof(Count)} method.")]
	protected List<TQueryResultItem> SelectPage(int pageIndex, int pageSize)
	{
		Contract.Requires<ArgumentOutOfRangeException>(pageIndex >= 0);
		Contract.Requires<ArgumentOutOfRangeException>(pageSize >= 0);

		return Query().Skip(pageIndex * pageSize).Take(pageSize).ToList();
	}

	/// <summary>
	/// Vrátí danou stránku dané velikosti dle Query.
	/// </summary>
	[Obsolete($"Replaced by {nameof(GetPageAsync)} method which contains optimized use of {nameof(Count)} method.")]
	protected Task<List<TQueryResultItem>> SelectPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentOutOfRangeException>(pageIndex >= 0);
		Contract.Requires<ArgumentOutOfRangeException>(pageSize >= 0);

		return Query().Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);
	}

	/// <summary>
	/// Vrátí danou stránku dat a počet záznamů dle Query.
	/// </summary>
	protected DataFragment<TQueryResultItem> GetPage(int pageIndex, int pageSize)
	{
		Contract.Requires<ArgumentOutOfRangeException>(pageIndex >= 0);
		Contract.Requires<ArgumentOutOfRangeException>(pageSize >= 0);

		return GetDataFragment(pageIndex * pageSize, pageSize);
	}

	/// <summary>
	/// Vrátí danou stránku dat a počet záznamů dle Query.
	/// </summary>
	protected async Task<DataFragment<TQueryResultItem>> GetPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentOutOfRangeException>(pageIndex >= 0);
		Contract.Requires<ArgumentOutOfRangeException>(pageSize >= 0);

		return await GetDataFragmentAsync(pageIndex * pageSize, pageSize, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Vrátí fragment dat dle Query.
	/// </summary>
	[Obsolete($"Replaced by {nameof(GetDataFragment)} method which contains optimized use of {nameof(Count)} method.")]
	protected List<TQueryResultItem> SelectDataFragment(int startIndex, int? count)
	{
		Contract.Requires<ArgumentOutOfRangeException>(startIndex >= 0);
		Contract.Requires<ArgumentOutOfRangeException>((count == null) || (count >= 0));

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
	[Obsolete($"Replaced by {nameof(GetDataFragment)} method which contains optimized use of {nameof(Count)} method.")]
	protected Task<List<TQueryResultItem>> SelectDataFragmentAsync(int startIndex, int? count, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentOutOfRangeException>(startIndex >= 0);
		Contract.Requires<ArgumentOutOfRangeException>((count == null) || (count >= 0));

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
	/// Vrátí fragment dat a počet záznamů dle Query.
	/// </summary>
	protected internal DataFragment<TQueryResultItem> GetDataFragment(int startIndex, int? count)
	{
		Contract.Requires<ArgumentOutOfRangeException>((count == null) || (count >= 0));

		IQueryable<TQueryResultItem> originalQuery = Query();
		IQueryable<TQueryResultItem> fragmentQuery = originalQuery;

		if (startIndex > 0)
		{
			fragmentQuery = fragmentQuery.Skip(startIndex);
		}
		if (count != null)
		{
			fragmentQuery = fragmentQuery.Take(count.Value);
		}

		List<TQueryResultItem> data = fragmentQuery.ToList();
		int totalCount = IsCallCountRequired(startIndex, count, data.Count)
				? originalQuery.Count()
				: (startIndex + data.Count); // výkonová zkratka - pokud víme, kolik záznamů je celkem, nemusíme volat Count do databáze

		return new DataFragment<TQueryResultItem>()
		{
			Data = data,
			TotalCount = totalCount
		};
	}

	/// <summary>
	/// Vrátí fragment dat a počet záznamů dle Query.
	/// </summary>
	protected internal async Task<DataFragment<TQueryResultItem>> GetDataFragmentAsync(int startIndex, int? count, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentOutOfRangeException>((count == null) || (count >= 0));

		IQueryable<TQueryResultItem> originalQuery = Query();
		IQueryable<TQueryResultItem> fragmentQuery = originalQuery;
		if (startIndex > 0)
		{
			fragmentQuery = fragmentQuery.Skip(startIndex);
		}
		if (count != null)
		{
			fragmentQuery = fragmentQuery.Take(count.Value);
		}

		List<TQueryResultItem> data = await fragmentQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
		int totalCount = IsCallCountRequired(startIndex, count, data.Count)
			? await originalQuery.CountAsync(cancellationToken).ConfigureAwait(false)
			: (startIndex + data.Count); // výkonová zkratka - pokud víme, kolik záznamů je celkem, nemusíme volat Count do databáze

		return new DataFragment<TQueryResultItem>()
		{
			Data = data,
			TotalCount = totalCount
		};
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

	// Spočítáme určit celkový počet záznamů, avšak ideálně omezit volání CountAsync, pokud není nezbytné.
	// Vycházíme z tohoto
	// - Pokud jsme neomezili počet záznamů k načtení (count == 0), pak máme v data načtena všechna data (od startIndexu dále).
	// - Pokud jsme se omezili na počet záznamů (jen určitou stránku) a zároveň se nám načetlo méně záznamů, než je velikost stránky,
	//   pak jsme právě načetli poslední stránku (poslední segment dat od startIndexu).
	// - A zároveň, pokud jsme v tomto případě nenačetli žádný záznam a zároveň jsme za první stranou (startIndex > 0),
	//   pak se ptáme na stránku (segment) "někde za koncem" a nevíme o počtu záznamů nic.
	//   (Pokud je startIndex nulový i data.Count nulové, pak nejsou k dispozici žádná data.)
	internal static bool IsCallCountRequired(int startIndex, int? count, int dataCount)
	{
		bool canUseDataCount = ((count == null) || ((count != null) && (dataCount < count) && ((dataCount > 0) || (startIndex == 0))));
		return !canUseDataCount;
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
