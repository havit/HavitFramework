using System.Collections.Generic;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Unit of Work s podporou asynchronního zpracování.
	/// </summary>
	public interface IUnitOfWorkAsync
	{
		/// <summary>
		/// Asynchronně uloží změny registrované v Unit of Work.
		/// </summary>
		Task CommitAsync();

		/// <summary>
		/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
		/// </summary>
		void AddForInsert<TEntity>(TEntity item)
			where TEntity : class;

		/// <summary>
		/// Zajistí vložení objeků jako nové objekty (při uložení budou vloženy).
		/// </summary>
		void AddRangeForInsert<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class;

		/// <summary>
		/// Zajistí odstranění objeku (při uložení bude smazán).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		void AddForDelete<TEntity>(TEntity item)
			where TEntity : class;

		/// <summary>
		/// Zajistí odstranění objeků (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		void AddRangeForDelete<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class;
	}
}
