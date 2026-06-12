using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

/// <summary>
/// Zajišťuje podporu mazání příznakem.
/// Mazání příznakem je podporováno na typech mající vlastost Deleted typu DateTime?.
/// </summary>
public class SoftDeleteManager : ISoftDeleteManager
{
	private readonly ITimeService _timeService;
	private readonly ConcurrentDictionary<Type, SoftDeleteTypeInfo> _typeInfoDictionary = new ConcurrentDictionary<Type, SoftDeleteTypeInfo>();

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="timeService">Služba pro práci s časem. Používá se pro získání času smazání objektu, který má být objektu nastaven.</param>
	public SoftDeleteManager(ITimeService timeService)
	{
		_timeService = timeService;
	}

	/// <summary>
	/// Určuje, zda je na typu entityType podporováno mazání příznakem.
	/// </summary>
	public bool IsSoftDeleteSupported(Type entityType)
	{
		return GetTypeInfo(entityType).IsSupported;
	}

	/// <summary>
	/// Určuje, zda je na typu TEntity podporováno mazání příznakem.
	/// </summary>
	public bool IsSoftDeleteSupported<TEntity>()
	{
		return GetTypeInfo(typeof(TEntity)).IsSupported;
	}

	/// <summary>
	/// Nastaví na dané instanci příznak smazání, není-li dosud nastaven.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public void SetDeleted<TEntity>(TEntity entity)
	{
		SoftDeleteTypeInfo typeInfo = GetSupportedTypeInfoOrThrow(typeof(TEntity));

		if (typeInfo.DeletedGetter(entity) == null)
		{
			typeInfo.DeletedSetter(entity, _timeService.GetCurrentTime());
		}
	}

	/// <summary>
	/// Zruší příznak smazání, je-li nastaven.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public void SetNotDeleted<TEntity>(TEntity entity)
	{
		GetSupportedTypeInfoOrThrow(typeof(TEntity)).DeletedSetter(entity, null);
	}

	/// <summary>
	/// Vrací výraz (expression tree) pro filtrování objektů, které nemají nastaven příznak smazání.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public Expression<Func<TEntity, bool>> GetNotDeletedExpressionLambda<TEntity>()
	{
		return (Expression<Func<TEntity, bool>>)GetSupportedTypeInfoOrThrow(typeof(TEntity)).NotDeletedExpressionLambda;
	}

	/// <summary>
	/// Vrací zkompilovaný lambda výraz pro filtrování objektů, které nemají nastaven příznak smazání.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu TEntity není podporováno mazání příznakem.</exception>
	public Func<TEntity, bool> GetNotDeletedCompiledLambda<TEntity>()
	{
		return (Func<TEntity, bool>)GetSupportedTypeInfoOrThrow(typeof(TEntity)).NotDeletedCompiledLambda;
	}

	/// <summary>
	/// Vrátí (a nacachuje) informace o podpoře mazání příznakem pro daný typ, vč. zkompilovaných přístupů k vlastnosti Deleted.
	/// </summary>
	private SoftDeleteTypeInfo GetTypeInfo(Type entityType)
	{
		return _typeInfoDictionary.GetOrAdd(entityType, static type =>
		{
			PropertyInfo deletedProperty = type.GetProperty("Deleted");
			if ((deletedProperty == null) || (deletedProperty.PropertyType != typeof(DateTime?)))
			{
				return SoftDeleteTypeInfo.NotSupported;
			}

			ParameterExpression entityParameter = Expression.Parameter(typeof(object), "entity");
			MemberExpression deletedPropertyAccess = Expression.Property(Expression.Convert(entityParameter, type), deletedProperty);
			ParameterExpression valueParameter = Expression.Parameter(typeof(DateTime?), "value");

			ParameterExpression lambdaParameter = Expression.Parameter(type, "o");
			// Expression.Constant musí být typovaný (DateTime?), viz workaround k https://github.com/dotnet/efcore/issues/35059.
			LambdaExpression notDeletedExpressionLambda = Expression.Lambda(
				Expression.Equal(Expression.Property(lambdaParameter, deletedProperty), Expression.Constant(null, typeof(DateTime?))),
				lambdaParameter);

			return new SoftDeleteTypeInfo
			{
				IsSupported = true,
				DeletedGetter = Expression.Lambda<Func<object, DateTime?>>(deletedPropertyAccess, entityParameter).Compile(),
				DeletedSetter = Expression.Lambda<Action<object, DateTime?>>(Expression.Assign(deletedPropertyAccess, valueParameter), entityParameter, valueParameter).Compile(),
				NotDeletedExpressionLambda = notDeletedExpressionLambda,
				NotDeletedCompiledLambda = notDeletedExpressionLambda.Compile()
			};
		});
	}

	/// <summary>
	/// Vrátí informace o podpoře mazání příznakem pro daný typ. Není-li na typu mazání příznakem podporováno, vyhazuje NotSupportedException.
	/// </summary>
	/// <exception cref="NotSupportedException">Na typu entityType není podporováno mazání příznakem.</exception>
	private SoftDeleteTypeInfo GetSupportedTypeInfoOrThrow(Type entityType)
	{
		SoftDeleteTypeInfo typeSupport = GetTypeInfo(entityType);
		if (!typeSupport.IsSupported)
		{
			ThrowSoftDeleteNotSupported(entityType);
		}
		return typeSupport;
	}

	/// <summary>
	/// Vyhazuje NotSupportedException. Samostatná metoda (throw helper) odkládá alokaci chybové zprávy až do okamžiku selhání.
	/// </summary>
	[DoesNotReturn]
	private static void ThrowSoftDeleteNotSupported(Type entityType)
	{
		throw new NotSupportedException($"Soft Delete is not supported on type {entityType.FullName}.");
	}

	/// <summary>
	/// Nacachované informace o podpoře mazání příznakem pro jeden entitní typ.
	/// </summary>
	private sealed class SoftDeleteTypeInfo
	{
		/// <summary>
		/// Sdílená instance pro typy, na kterých není mazání příznakem podporováno.
		/// </summary>
		public static readonly SoftDeleteTypeInfo NotSupported = new SoftDeleteTypeInfo();

		public bool IsSupported { get; init; }
		public Func<object, DateTime?> DeletedGetter { get; init; }
		public Action<object, DateTime?> DeletedSetter { get; init; }
		public LambdaExpression NotDeletedExpressionLambda { get; init; }
		public Delegate NotDeletedCompiledLambda { get; init; }
	}
}
