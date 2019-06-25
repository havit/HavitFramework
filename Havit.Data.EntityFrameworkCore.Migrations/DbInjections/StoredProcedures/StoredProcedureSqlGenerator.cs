namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections.StoredProcedures
{
    /// <summary>
    /// Implementation of <see cref="IDbInjectionSqlGenerator"/>, that handles <see cref="StoredProcedureDbInjection"/>s.
    /// </summary>
	public class StoredProcedureSqlGenerator : DbInjectionSqlGenerator<StoredProcedureDbInjection>
	{
        /// <inheritdoc />
        protected override string GenerateAlterSql(StoredProcedureDbInjection dbInjection)
		{
			return dbInjection.CreateSql.Replace("CREATE PROCEDURE", "ALTER PROCEDURE");
		}

        /// <inheritdoc />
        protected override string GenerateDropSql(StoredProcedureDbInjection dbInjection)
		{
			return $"DROP PROCEDURE [{dbInjection.ProcedureName}]";
		}
	}
}