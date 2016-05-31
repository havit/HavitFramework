using System;
using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.SoftDeletes
{
	/// <summary>
	/// Zajišťuje podporu mazání příznakem.
	/// </summary>
	public interface ISoftDeleteManager
	{
		/// <summary>
		/// Určuje, zda je na typu entityType podporováno mazání příznakem.
		/// </summary>
		bool IsSoftDeleteSupported(Type entityType);

		/// <summary>
		/// Určuje, zda je na typu TEntity podporováno mazání příznakem.
		/// </summary>
		bool IsSoftDeleteSupported<TEntity>();

		/// <summary>
		/// Nastaví na dané instanci příznak smazání, není-li dosud nastaven.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		void SetDeleted<TEntity>(TEntity entity);

		/// <summary>
		/// Zruší příznak smazání, je-li nastaven.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		void SetNotDeleted<TEntity>(TEntity entity);

		/// <summary>
		/// Vrací výraz pro filtrování objektů, které nemají nastaven příznak smazání.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		Expression<Func<TEntity, bool>> GetNotDeletedExpression<TEntity>();
	}
}