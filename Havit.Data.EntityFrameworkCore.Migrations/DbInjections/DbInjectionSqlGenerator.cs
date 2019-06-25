namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    /// Base implementation of <see cref="IDbInjectionSqlGenerator"/> that provides strongly-typed implementation of methods for inherited classes to use.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="IDbInjection"/> that this <see cref="IDbInjectionSqlGenerator"/> implementation should handle.</typeparam>
    public abstract class DbInjectionSqlGenerator<T> : IDbInjectionSqlGenerator
		where T : IDbInjection
	{
		string IDbInjectionSqlGenerator.GenerateDropSql(IDbInjection dbInjection) => IsMatchingDbInjection(dbInjection) ? GenerateDropSql((T)dbInjection) : null;

        string IDbInjectionSqlGenerator.GenerateAlterSql(IDbInjection dbInjection) => IsMatchingDbInjection(dbInjection) ? GenerateAlterSql((T)dbInjection) : null;

        /// <summary>
        /// Generates DROP SQL script for <see cref="IDbInjection"/> of type <typeparamref name="T"/>.
        /// </summary>
        protected abstract string GenerateDropSql(T dbInjection);

        /// <summary>
        /// Generates ALTER SQL script for <see cref="IDbInjection"/> of type <typeparamref name="T"/>.
        /// </summary>
        protected abstract string GenerateAlterSql(T dbInjection);

		private static bool IsMatchingDbInjection(IDbInjection dbInjection) => typeof(T).IsAssignableFrom(dbInjection?.GetType());
	}
}