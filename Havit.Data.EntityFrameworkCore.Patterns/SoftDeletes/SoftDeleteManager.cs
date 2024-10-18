using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

/// <summary>
/// Zajišťuje podporu mazání příznakem.
/// Mazání příznakem je podporováno na typech mající vlastost Deleted typu DateTime?.
/// </summary>
public class SoftDeleteManager : ISoftDeleteManager
{
	private readonly ITimeService _timeService;
	private readonly ConcurrentDictionary<Type, bool> _supportedTypesDictionary = new ConcurrentDictionary<Type, bool>();
	private readonly ConcurrentDictionary<Type, object> _notDeletedExpressionLambdaDictionary = new ConcurrentDictionary<Type, object>();
	private readonly ConcurrentDictionary<Type, object> _notDeletedCompiledLambdaDictionary = new ConcurrentDictionary<Type, object>();

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="timeService">Služba pro práci s časem. Používá se pro získání času smazání objektu, který má být objektu nastaven.</param>
	public SoftDeleteManager(ITimeService timeService)
	{
		this._timeService = timeService;
	}

	/// <summary>
	/// Určuje, zda je na typu entityType podporováno mazání příznakem.
	/// </summary>
	public bool IsSoftDeleteSupported(Type entityType)
	{
		Contract.Requires<ArgumentNullException>(entityType != null);

		return _supportedTypesDictionary.GetOrAdd(entityType, _ =>
		{
			PropertyInfo deletedProperty = entityType.GetProperty("Deleted");
			return (deletedProperty != null) && deletedProperty.PropertyType == typeof(DateTime?);
		});
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
		Contract.Requires<NotSupportedException>(IsSoftDeleteSupported(typeof(TEntity)), String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));

		dynamic d = entity;
		if ((DateTime?)d.Deleted == null)
		{
			d.Deleted = _timeService.GetCurrentTime();
		}
	}

	/// <summary>
	/// Zruší příznak smazání, je-li nastaven.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public void SetNotDeleted<TEntity>(TEntity entity)
	{
		Contract.Requires<NotSupportedException>(IsSoftDeleteSupported(typeof(TEntity)), String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));

		dynamic d = entity;
		d.Deleted = null;
	}

	/// <summary>
	/// Vrací výraz (expression tree) pro filtrování objektů, které nemají nastaven příznak smazání.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public Expression<Func<TEntity, bool>> GetNotDeletedExpressionLambda<TEntity>()
	{
		Contract.Requires<NotSupportedException>(IsSoftDeleteSupported(typeof(TEntity)), String.Format("Soft Delete is not supported on type {0}.", typeof(TEntity).FullName));

		return (Expression<Func<TEntity, bool>>)_notDeletedExpressionLambdaDictionary.GetOrAdd(typeof(TEntity), _ =>
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "o");
			BinaryExpression equal = Expression.Equal(Expression.Property(parameter, "Deleted"), Expression.Constant(null));
			return Expression.Lambda(equal, parameter);
		});
	}

	/// <summary>
	/// Vrací zkompilovaný lambda výraz pro filtrování objektů, které nemají nastaven příznak smazání.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public Func<TEntity, bool> GetNotDeletedCompiledLambda<TEntity>()
	{
		return (Func<TEntity, bool>)_notDeletedCompiledLambdaDictionary.GetOrAdd(typeof(TEntity), _ => GetNotDeletedExpressionLambda<TEntity>().Compile());
	}

}
