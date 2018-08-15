using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes
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
		/// Vrací výraz (expression tree) pro filtrování objektů, které nemají nastaven příznak smazání.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		public Expression<Func<TEntity, bool>> GetNotDeletedExpressionLambda<TEntity>()
		{
			if (!IsSoftDeleteSupported<TEntity>())
			{
				throw new NotSupportedException(String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));
			}
			
			if (_getNotDeletedExpressionLambdaDictionary.TryGetValue(typeof(TEntity), out var result))
			{
				return (Expression<Func<TEntity, bool>>)result;
			}

			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "o");
			BinaryExpression equal = Expression.Equal(Expression.Property(parameter, "Deleted"), Expression.Constant(null));
			result = Expression.Lambda(equal, parameter);
			_getNotDeletedExpressionLambdaDictionary.TryAdd(typeof(TEntity), result);

			return (Expression<Func<TEntity, bool>>)result;
		}
		private readonly ConcurrentDictionary<Type, object> _getNotDeletedExpressionLambdaDictionary = new ConcurrentDictionary<Type, object>();

		/// <summary>
		/// Vrací zkompilovaný lambda výraz pro filtrování objektů, které nemají nastaven příznak smazání.
		/// </summary>
		/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
		public Func<TEntity, bool> GetNotDeletedCompiledLambda<TEntity>()
		{
			if (_getNotDeletedCompiledLambdaDictionary.TryGetValue(typeof(TEntity), out var result))
			{
				return (Func<TEntity, bool>)result;
			}

			result = GetNotDeletedExpressionLambda<TEntity>().Compile();
			_getNotDeletedCompiledLambdaDictionary.TryAdd(typeof(TEntity), result);

			return (Func<TEntity, bool>)result;
		}
		private readonly ConcurrentDictionary<Type, object> _getNotDeletedCompiledLambdaDictionary = new ConcurrentDictionary<Type, object>();

	}
}
