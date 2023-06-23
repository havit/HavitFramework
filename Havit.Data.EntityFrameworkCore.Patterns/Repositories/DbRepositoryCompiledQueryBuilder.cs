using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

internal class DbRepositoryCompiledQueryBuilder
{
	public static DbRepositoryCompiledQueryBuilder Default { get; } = new DbRepositoryCompiledQueryBuilder();

	internal MethodInfo firstOrDefaultMethod;
	internal MethodInfo whereMethod;
	internal MethodInfo dbContextSetMethod;
	internal MethodInfo tagWithMethod;

	public Func<DbContext, int, TEntity> CreateCompiledGetObjectQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		EnsureMethodInfos();

		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var idParameter = Expression.Parameter(typeof(int), "id");

		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		// TODO: QueryTag!

		Expression<Func<DbContext, int, TEntity>> expression = Expression.Lambda<Func<DbContext, int, TEntity>>(
			body:
				// dbContext.Set<TEntity>().Where(item => item.Id = @id).FirstOrDefault()
				Expression.Call(null, firstOrDefaultMethod.MakeGenericMethod(typeof(TEntity)), // .FirstOrDefault
					Expression.Call(null, whereMethod.MakeGenericMethod(typeof(TEntity)), // Where(...)
						Expression.Call(null, tagWithMethod.MakeGenericMethod(typeof(TEntity)), // TagWith(...)
							Expression.Call(dbContextParameter, dbContextSetMethod.MakeGenericMethod(typeof(TEntity))), //dbContext.Set<TEntity>()
							Expression.Constant(QueryTagBuilder.CreateTag(repositoryType, nameof(Havit.Data.Patterns.Repositories.IRepository<object>.GetObject)))
						), // TagWith
						Expression.Lambda<Func<TEntity, bool>>( // item => item.Id == @id
							Expression.Equal( // ==
								Expression.Property(itemParameter, typeof(TEntity), entityKeyAccessor.GetEntityKeyPropertyName()), // item.Id
								idParameter // @id
							),
							itemParameter
						)
					)
				),
			// parameters:
				dbContextParameter,
				idParameter);

		return EF.CompileQuery<DbContext, int, TEntity>(expression);
	}

	public Func<DbContext, int, CancellationToken, Task<TEntity>> CreateCompiledGetObjectAsyncQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		EnsureMethodInfos();

		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
		var idParameter = Expression.Parameter(typeof(int), "id");

		ParameterExpression itemParameter = Expression.Parameter(typeof(TEntity), "item");

		Expression<Func<DbContext, int, CancellationToken, TEntity>> expression = Expression.Lambda<Func<DbContext, int, CancellationToken, TEntity>>(
			body:
				// dbContext.Set<TEntity>().TagWith(...).Where(item => item.Id = @id).FirstOrDefault()
				Expression.Call(null, firstOrDefaultMethod.MakeGenericMethod(typeof(TEntity)), // .FirstOrDefault
					Expression.Call(null, whereMethod.MakeGenericMethod(typeof(TEntity)), // Where(...)
						Expression.Call(null, tagWithMethod.MakeGenericMethod(typeof(TEntity)), // TagWith(...)
							Expression.Call(dbContextParameter, dbContextSetMethod.MakeGenericMethod(typeof(TEntity))), //dbContext.Set<TEntity>()
							Expression.Constant(QueryTagBuilder.CreateTag(repositoryType, nameof(Havit.Data.Patterns.Repositories.IRepository<object>.GetObject)))
						), // TagWith
						Expression.Lambda<Func<TEntity, bool>>( // item => item.Id == @id
							Expression.Equal( // ==
								Expression.Property(itemParameter, typeof(TEntity), entityKeyAccessor.GetEntityKeyPropertyName()), // item.Id
								idParameter // @id
							),
							itemParameter
						) // /Lambda
					) // /Where
				), // /FirstOrDefault
			// parameters:
			dbContextParameter,
			idParameter,
			cancellationTokenParameter
		); // /Lambda

		return EF.CompileAsyncQuery<DbContext, int, TEntity>(expression);
	}

	public Func<DbContext, IEnumerable<TEntity>> CreateCompiledGetAllQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return EF.CompileQuery<DbContext, IEnumerable<TEntity>>((DbContext dbContext) => dbContext.Set<TEntity>().TagWith(QueryTagBuilder.CreateTag(repositoryType, nameof(Havit.Data.Patterns.Repositories.IRepository<object>.GetAll))).WhereNotDeleted(softDeleteManager));
	}

	public Func<DbContext, IAsyncEnumerable<TEntity>> CreateCompiledGetAllAsyncQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return EF.CompileAsyncQuery<DbContext, TEntity>((DbContext dbContext) => dbContext.Set<TEntity>().TagWith(QueryTagBuilder.CreateTag(repositoryType, nameof(Havit.Data.Patterns.Repositories.IRepository<object>.GetAllAsync))).WhereNotDeleted(softDeleteManager));
	}

	internal void CreateCompiledAsyncQuery()
	{
		EnsureMethodInfos();

		var dbContextParameter = Expression.Parameter(typeof(DbContext), "dbContext");
		var idParameter = Expression.Parameter(typeof(int), "id");

	}

	internal void EnsureMethodInfos()
	{
		if (firstOrDefaultMethod == null)
		{
			lock (typeof(DbRepositoryCompiledQueryBuilder))
			{
				if (firstOrDefaultMethod == null)
				{
					firstOrDefaultMethod = typeof(Queryable).GetMethods()
						 .Where(method => method.Name == "FirstOrDefault")
						 .Select(method => new { Method = method, Parameters = method.GetParameters() })
						 .Where(item => item.Parameters.Length == 1
									 && item.Parameters[0].ParameterType.IsGenericType
									 && item.Parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
						 .Select(item => item.Method)
						 .SingleOrDefault();

					whereMethod = typeof(Queryable).GetMethods()
						 .Where(method => method.Name == "Where")
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
						 .SingleOrDefault();

					dbContextSetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), 1, BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);

					tagWithMethod = typeof(EntityFrameworkQueryableExtensions).GetMethod("TagWith");

				}
			}
		}
	}
}
