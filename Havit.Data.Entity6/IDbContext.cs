using System.Data.Entity;
using System.Threading.Tasks;

namespace Havit.Data.Entity
{
	/// <summary>
	/// Interface DbContextu.
	/// </summary>
	public interface IDbContext
	{
		/// <summary>
		/// Vrací DbSet pro danou entitu.
		/// </summary>
		DbSet<TEntity> Set<TEntity>()
			where TEntity : class;

		/// <summary>
		/// Vrací stav entity z change trackeru.
		///  Metoda primárně slouží pro možnost testovatelnosti (mockování). K dispozici je context.Entry(entity).State, avšak taková metoda nelze mockovat neboť
		/// a) DbEntityEntry nelze podědit, protože má interní konstruktor.
		/// b) DbEntityEntry.State není virtuální
		/// </summary>
		EntityState GetEntityState(object entity);

		/// <summary>
		/// Uloží změny.
		/// </summary>
		void SaveChanges();

		/// <summary>
		/// Uloží změny.
		/// </summary>
		Task SaveChangesAsync();		
	}
}
