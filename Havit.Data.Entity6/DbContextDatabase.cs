using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Havit.Data.Entity
{
	/// <summary>
	/// Zpřístupňuje metody pro práci přímo s databází (např. spouštění SQL dotazů).
	/// </summary>
	internal class DbContextDatabase : IDbContextDatabase
	{
		private readonly DbContext dbContext;

		/// <inheritdoc />
		public DbConnection Connection => dbContext.Database.Connection;

		/// <inheritdoc />
		public int? CommandTimeout
		{
			get
			{
				return dbContext.Database.CommandTimeout;
			}
			set
			{
				dbContext.Database.CommandTimeout = value;
			}
		}

		/// <inheritdoc />
		public DbContextTransaction CurrentTransaction => dbContext.Database.CurrentTransaction;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbContextDatabase(DbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <inheritdoc />
		public DbContextTransaction BeginTransaction() => dbContext.Database.BeginTransaction();
		
		/// <inheritdoc />
		public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel) => dbContext.Database.BeginTransaction(isolationLevel);

		/// <inheritdoc />
		public int ExecuteSqlCommand(string sql, params object[] parameters) => dbContext.Database.ExecuteSqlCommand(sql, parameters);

		/// <inheritdoc />
		public int ExecuteSqlCommand(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters) => dbContext.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

		/// <summary>
		/// Creates a raw SQL query that will return elements of the given generic type. The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type. Use the SqlQuery method to return entities that are tracked by the context. As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter. context.Database.SqlQuery&amp;lt;Post&amp;gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor); Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string. context.Database.SqlQuery&amp;lt;Post&amp;gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
		/// </summary>
		/// <remarks>
		/// Pro volání stored procedury lze její název uvést do parametru sql.
		/// </remarks>
		/// <see cref="Database.SqlQuery{TElement}(String, Object[])" />
		public DbRawSqlQuery<TElement> SqlQueryRaw<TElement>(string sql, params object[] parameters)
		{
			return dbContext.Database.SqlQuery<TElement>(sql, parameters);
		}

		/// <summary>
		/// Creates a raw SQL query that will return entities in this set.By default, the entities returned are tracked by the context; this can be changed by calling AsNoTracking on the DbRawSqlQuery returned.Note that the entities returned are always of the type for this set and never of a derived type.If the table or tables queried may contain data for other entity types, then the SQL query must be written appropriately to ensure that only entities of the correct type are returned.As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack.You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter. context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor); Alternatively, you can also construct a DbParameter and supply it to SqlQuery.This allows you to use named parameters in the SQL query string. context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
		/// </summary>
		/// <remarks>
		/// Pro volání stored procedury lze její název uvést do parametru sql.
		/// </remarks>
		/// <see cref="DbSet{TEntity}.SqlQuery(String, Object[])" />
		public DbSqlQuery<TElement> SqlQueryEntity<TElement>(string sql, params object[] parameters)
			where TElement : class
		{
			return dbContext.Set<TElement>().SqlQuery(sql, parameters);
		}
	}
}