using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Havit.Services.TimeServices;

namespace Havit.Data.Entity.Patterns.SoftDeletes
{
	/// <summary>
	/// Zajišťuje podporu mazání příznakem.
	/// Mazání příznakem je podporováno na typech mající vlastost Deleted typu DateTime?.
	/// </summary>
	public class SoftDeleteManager : ISoftDeleteManager
	{
		private readonly ITimeService timeService;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="timeService">Služba pro práci s časem. Používá se pro získání času smazání objektu, který má být objektu nastaven.</param>
		public SoftDeleteManager(ITimeService timeService)
		{
			this.timeService = timeService;
		}

		/// <summary>
		/// Určuje, zda je na typu entityType podporováno mazání příznakem.
		/// </summary>
		public bool IsSoftDeleteSupported(Type entityType)
		{
			Contract.Requires(entityType != null);

			PropertyInfo deletedProperty = entityType.GetProperty("Deleted");
			return (deletedProperty != null) && deletedProperty.PropertyType == typeof(DateTime?);
		}

		/// <summary>
		/// Určuje, zda je na typu TEntity podporováno mazání příznakem.
		/// </summary>
		public bool IsSoftDeleteSupported<TEntity>()
		{
			return IsSoftDeleteSupported(typeof(TEntity));
		}

		/// <summary>
		/// Nastaví na dané instanci příznak smazání, není-li dosud nastaven.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		public void SetDeleted<TEntity>(TEntity entity)
		{
			if (!IsSoftDeleteSupported<TEntity>())
			{
				throw new NotSupportedException(String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));
			}

			dynamic d = entity;
			if ((DateTime?)d.Deleted == null)
			{
				d.Deleted = timeService.GetCurrentTime();
			}
		}

		/// <summary>
		/// Zruší příznak smazání, je-li nastaven.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		public void SetNotDeleted<TEntity>(TEntity entity)
		{
			if (!IsSoftDeleteSupported<TEntity>())
			{
				throw new NotSupportedException(String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));
			}

			dynamic d = entity;
			d.Deleted = null;
		}

		/// <summary>
		/// Vrací výraz pro filtrování objektů, které nemají nastaven příznak smazání.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		public Expression<Func<TEntity, bool>> GetNotDeletedExpression<TEntity>()
		{
			if (!IsSoftDeleteSupported<TEntity>())
			{
				throw new NotSupportedException(String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));
			}

			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "o");
			BinaryExpression equal = Expression.Equal(Expression.Property(parameter, "Deleted"), Expression.Constant(null));
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(equal, parameter);
		}
	}
}
