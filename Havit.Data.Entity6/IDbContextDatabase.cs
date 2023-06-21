using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Havit.Data.Entity;

/// <summary>
/// Zpřístupňuje metody pro práci přímo s databází (např. spouštění SQL dotazů).
/// </summary>
public interface IDbContextDatabase
{
	/// <summary>
	/// Returns the connection being used by this context. This may cause the connection to be created if it does not already exist.
	/// </summary>
	DbConnection Connection { get; }

	/// <summary>
	/// Gets or sets the timeout value, in seconds, for all context operations. The default
	/// value is null, where null indicates that the default value of the underlying
	/// provider will be used.
	/// </summary>		
	int? CommandTimeout { get; set; }

	/// <summary>
	/// Gets the transaction the underlying store connection is enlisted in. May be null.
	/// </summary>
	DbContextTransaction CurrentTransaction { get; }

	/// <summary>
	/// Begins a transaction on the underlying store connection
	/// </summary>
	DbContextTransaction BeginTransaction();

	/// <summary>
	///  Begins a transaction on the underlying store connection using the specified isolation level.
	/// </summary>
	DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

	/// <summary>
	///  Runs the registered System.Data.Entity.IDatabaseInitializer`1 on this context.
	///  If "force" is set to true, then the initializer is run regardless of whether
	///  or not it has been run before. This can be useful if a database is deleted while
	///  an app is running and needs to be reinitialized. If "force" is set to false,
	///  then the initializer is only run if it has not already been run for this context,
	///  model, and connection in this app domain. This method is typically used when
	///  it is necessary to ensure that the database has been created and seeded before
	///  starting some operation where doing so lazily will cause issues, such as when
	///  the operation is part of a transaction.
	/// </summary>
	void Initialize(bool force = false);

	/// <summary>
	/// Executes the given DDL/DML command against the database. As with any API that
	/// accepts SQL it is important to parameterize any user input to protect against
	/// a SQL injection attack. You can include parameter place holders in the SQL query
	/// string and then supply parameter values as additional arguments. Any parameter
	/// values you supply will automatically be converted to a DbParameter. context.Database.ExecuteSqlCommand("UPDATE
	/// dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor); Alternatively,
	/// you can also construct a DbParameter and supply it to SqlQuery. This allows you
	/// to use named parameters in the SQL query string. context.Database.ExecuteSqlCommand("UPDATE
	/// dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author",
	/// userSuppliedAuthor));
	/// </summary>
	int ExecuteSqlCommand(string sql, params object[] parameters);

	/// <summary>
	/// Executes the given DDL/DML command against the database. As with any API that
	/// accepts SQL it is important to parameterize any user input to protect against
	/// a SQL injection attack. You can include parameter place holders in the SQL query
	/// string and then supply parameter values as additional arguments. Any parameter
	/// values you supply will automatically be converted to a DbParameter. context.Database.ExecuteSqlCommand("UPDATE
	/// dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor); Alternatively,
	/// you can also construct a DbParameter and supply it to SqlQuery. This allows you
	/// to use named parameters in the SQL query string. context.Database.ExecuteSqlCommand("UPDATE
	/// dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author",
	/// userSuppliedAuthor));
	/// </summary>
	int ExecuteSqlCommand(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters);
	
	/// <summary>
	/// Creates a raw SQL query that will return elements of the given generic type. The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type. Use the SqlQuery method to return entities that are tracked by the context. As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter. context.Database.SqlQuery&amp;lt;Post&amp;gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor); Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string. context.Database.SqlQuery&amp;lt;Post&amp;gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
	/// </summary>
	/// <remarks>
	/// Pro volání stored procedury lze její název uvést do parametru sql.
	/// </remarks>
	/// <see cref="Database.SqlQuery{TElement}(String, Object[])" />
	DbRawSqlQuery<TElement> SqlQueryRaw<TElement>(string sql, params object[] parameters);

	/// <summary>
	/// Creates a raw SQL query that will return entities in this set.By default, the entities returned are tracked by the context; this can be changed by calling AsNoTracking on the DbRawSqlQuery returned.Note that the entities returned are always of the type for this set and never of a derived type.If the table or tables queried may contain data for other entity types, then the SQL query must be written appropriately to ensure that only entities of the correct type are returned.As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack.You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter. context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor); Alternatively, you can also construct a DbParameter and supply it to SqlQuery.This allows you to use named parameters in the SQL query string. context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
	/// </summary>
	/// <remarks>
	/// Pro volání stored procedury lze její název uvést do parametru sql.
	/// </remarks>
	/// <see cref="DbSet{TEntity}.SqlQuery(String, Object[])" />
	DbSqlQuery<TElement> SqlQueryEntity<TElement>(string sql, params object[] parameters)
		where TElement : class;
}