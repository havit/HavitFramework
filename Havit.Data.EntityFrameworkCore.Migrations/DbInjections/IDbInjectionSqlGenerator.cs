namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	/// <summary>
	/// Service that generates ALTER and DROP SQL scripts for <see cref="IDbInjection"/>.
	/// </summary>
	public interface IDbInjectionSqlGenerator
	{
		/// <summary>
		/// Generates ALTER SQL script for <see cref="IDbInjection"/>.
		/// </summary>
		string GenerateAlterSql(IDbInjection dbInjection);

        /// <summary>
        /// Generates DROP SQL script for <see cref="IDbInjection"/>.
        /// </summary>
        string GenerateDropSql(IDbInjection dbInjection);
	}
}