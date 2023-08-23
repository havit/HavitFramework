using System.Linq.Expressions;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <inheritdoc />
public class RepositoryCompiledQueryBuilder : IRepositoryQueryBuilder
{
	// JK: Ačkoliv bych chtěl fieldy nechat cammelCase, SA1307 (The name of a public or internal field in C# does not begin with an upper-case letter) mě nutí k PascalCase.
	internal MethodInfo FirstOrDefaultMethod;
	internal MethodInfo WhereMethod;
	internal MethodInfo DbContextSetMethod;
	internal MethodInfo TagWithMethod;
	internal MethodInfo ContainsMethod;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public RepositoryCompiledQueryBuilder()
	{
		EnsureMethodInfos();
	}

	/// <inheritdoc />
	public Func<DbContext, int, TEntity> CreateGetObjectCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var idParameter = Expression.Parameter(typeof(int), "id");

		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		Expression<Func<DbContext, int, TEntity>> expression = Expression.Lambda<Func<DbContext, int, TEntity>>(
			body: GetGetObjectLambaBody(
					dbContextParameter,
					idParameter,
					QueryTagBuilder.CreateTag(repositoryType, nameof(Data.Patterns.Repositories.IRepository<object>.GetObject)),
					entityKeyAccessor),
				// parameters:
				dbContextParameter,
				idParameter
		);

		return EF.CompileQuery(expression);
	}

	/// <inheritdoc />
	public Func<DbContext, int, CancellationToken, Task<TEntity>> CreateGetObjectAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
		var idParameter = Expression.Parameter(typeof(int), "id");


		Expression<Func<DbContext, int, CancellationToken, TEntity>> expression = Expression.Lambda<Func<DbContext, int, CancellationToken, TEntity>>(
			body: GetGetObjectLambaBody(
					dbContextParameter,
					idParameter,
					QueryTagBuilder.CreateTag(repositoryType, nameof(Data.Patterns.Repositories.IRepository<object>.GetObjectAsync)),
					entityKeyAccessor),

			// parameters:
			dbContextParameter,
			idParameter,
			cancellationTokenParameter
		);

		return EF.CompileAsyncQuery(expression);
	}

	/// <inheritdoc />
	private Expression GetGetObjectLambaBody<TEntity>(ParameterExpression dbContextParameter, ParameterExpression idParameter, string tagName, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		// dbContext.Set<TEntity>().TagWith(...).Where(item => item.Id = @id).FirstOrDefault()
		return Expression.Call(null, FirstOrDefaultMethod.MakeGenericMethod(typeof(TEntity)), // .FirstOrDefault
			Expression.Call(null, WhereMethod.MakeGenericMethod(typeof(TEntity)), // Where(...)
				Expression.Call(null, TagWithMethod.MakeGenericMethod(typeof(TEntity)), // TagWith(...)
					Expression.Call(dbContextParameter, DbContextSetMethod.MakeGenericMethod(typeof(TEntity))), //dbContext.Set<TEntity>()
					Expression.Constant(tagName)
				), // TagWith
				Expression.Lambda<Func<TEntity, bool>>( // item => item.Id == @id
					Expression.Equal( // ==
						Expression.Property(itemParameter, typeof(TEntity), entityKeyAccessor.GetEntityKeyPropertyName()), // item.Id
						idParameter // @id
					),
					itemParameter
				) // /Lambda
			) // /Where
		); // /FirstOrDefault
	}

	/// <inheritdoc />
	public Func<DbContext, int[], IEnumerable<TEntity>> CreateGetObjectsCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
	where TEntity : class
	{
		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var idsParameter = Expression.Parameter(typeof(int[]), "ids");

		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		// dbContext.Set<TEntity>().Where(item => @ids.Contains(item.Id)
		Expression<Func<DbContext, int[], IEnumerable<TEntity>>> expression = Expression.Lambda<Func<DbContext, int[], IEnumerable<TEntity>>>(
			body: GetGetObjectsLambdaBody(
				dbContextParameter,
				idsParameter,
				QueryTagBuilder.CreateTag(repositoryType, nameof(Data.Patterns.Repositories.IRepository<object>.GetObjects)),
				entityKeyAccessor
			),
			// parameters:
			dbContextParameter,
			idsParameter
		);

		return EF.CompileQuery(expression);
	}

	/// <inheritdoc />
	public Func<DbContext, int[], IAsyncEnumerable<TEntity>> CreateGetObjectsAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var idsParameter = Expression.Parameter(typeof(int[]), "ids");

		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		// dbContext.Set<TEntity>().Where(item => @ids.Contains(item.Id)
		Expression<Func<DbContext, int[], IQueryable<TEntity>>> expression = Expression.Lambda<Func<DbContext, int[], IQueryable<TEntity>>>(
			body: GetGetObjectsLambdaBody(
				dbContextParameter,
				idsParameter,
				QueryTagBuilder.CreateTag(repositoryType, nameof(Data.Patterns.Repositories.IRepository<object>.GetObjectsAsync)),
				entityKeyAccessor
			),
			// parameters:
			dbContextParameter,
			idsParameter
		);

		return EF.CompileAsyncQuery(expression);
	}

	/// <inheritdoc />
	private Expression GetGetObjectsLambdaBody<TEntity>(ParameterExpression dbContextParameter, ParameterExpression idsParameter, string tagName, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		return Expression.Call(null, WhereMethod.MakeGenericMethod(typeof(TEntity)), // Where(...)
			Expression.Call(null, TagWithMethod.MakeGenericMethod(typeof(TEntity)), // TagWith(...)
				Expression.Call(dbContextParameter, DbContextSetMethod.MakeGenericMethod(typeof(TEntity))), //dbContext.Set<TEntity>()
				Expression.Constant(tagName)
			), // TagWith
			Expression.Lambda<Func<TEntity, bool>>( // @ids.Contains(item => item.Id)
				Expression.Call(null, ContainsMethod.MakeGenericMethod(typeof(int)),
					idsParameter,
					Expression.Property(itemParameter, typeof(TEntity), entityKeyAccessor.GetEntityKeyPropertyName())
				),
				itemParameter
			)
		);
	}

	/// <inheritdoc />
	public Func<DbContext, IEnumerable<TEntity>> CreateGetAllCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return EF.CompileQuery(GetAllQueryExpression<TEntity>(QueryTagBuilder.CreateTag(repositoryType, nameof(Data.Patterns.Repositories.IRepository<object>.GetAll)), softDeleteManager));
	}

	/// <inheritdoc />
	public Func<DbContext, IAsyncEnumerable<TEntity>> CreateGetAllAsyncCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return EF.CompileAsyncQuery(GetAllQueryExpression<TEntity>(QueryTagBuilder.CreateTag(repositoryType, nameof(Data.Patterns.Repositories.IRepository<object>.GetAllAsync)), softDeleteManager));
	}

	private Expression<Func<DbContext, IQueryable<TEntity>>> GetAllQueryExpression<TEntity>(string tagName, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return (dbContext) => dbContext.Set<TEntity>().TagWith(tagName).WhereNotDeleted(softDeleteManager);
	}

	private void EnsureMethodInfos()
	{
		if (FirstOrDefaultMethod == null)
		{
			lock (typeof(RepositoryCompiledQueryBuilder))
			{
				if (FirstOrDefaultMethod == null)
				{
					FirstOrDefaultMethod = typeof(Queryable).GetMethods()
						 .Where(method => method.Name == "FirstOrDefault")
						 .Select(method => new { Method = method, Parameters = method.GetParameters() })
						 .Where(item => item.Parameters.Length == 1
									 && item.Parameters[0].ParameterType.IsGenericType
									 && item.Parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
						 .Select(item => item.Method)
						 .Single();

					WhereMethod = typeof(Queryable).GetMethods()
						 .Where(method => method.Name == nameof(Queryable.Where))
						 .Select(method => new { Method = method, Parameters = method.GetParameters() })
						 .Where(item => item.Parameters.Length == 2
									 && item.Parameters[0].ParameterType.IsGenericType
									 && item.Parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
									 && item.Parameters[1].ParameterType.IsGenericType
									 && item.Parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
						 .Select(item => new { item.Method, Arguments = item.Parameters[1].ParameterType.GetGenericArguments() })
						 .Where(item => item.Arguments[0].IsGenericType
									 && item.Arguments[0].GetGenericTypeDefinition() == typeof(Func<,>))
						 .Select(item => new { item.Method, Arguments = item.Arguments[0].GetGenericArguments() })
						 .Where(item => item.Arguments[0].IsGenericParameter
									 && item.Arguments[1] == typeof(bool))
						 .Select(item => item.Method)
						 .Single();

					ContainsMethod = typeof(Enumerable)
						.GetMethods(BindingFlags.Public | BindingFlags.Static)
							.Where(m => m.Name == nameof(Enumerable.Contains))
							.Select(m => new
							{
								Method = m,
								Params = m.GetParameters(),
								Args = m.GetGenericArguments()
							})
							.Where(x => x.Params.Length == 2)
							.Select(x => x.Method)
							.Single();

					DbContextSetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), 1, BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);

					TagWithMethod = typeof(EntityFrameworkQueryableExtensions).GetMethod("TagWith");
				}
			}
		}
	}
}
