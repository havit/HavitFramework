namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections.StoredProcedures
{
	public class StoredProcedureSqlGenerator : DbInjectionSqlGenerator<StoredProcedureDbInjection>
	{
		protected override string GenerateAlterSql(StoredProcedureDbInjection dbInjection)
		{
			return dbInjection.CreateSql.Replace("CREATE PROCEDURE", "ALTER PROCEDURE");
		}

		protected override string GenerateDropSql(StoredProcedureDbInjection dbInjection)
		{
			return $"DROP PROCEDURE [{dbInjection.ProcedureName}]";
		}
	}
}