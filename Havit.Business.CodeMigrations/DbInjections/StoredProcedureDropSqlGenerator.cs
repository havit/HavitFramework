namespace Havit.Business.CodeMigrations.DbInjections
{
    public class StoredProcedureDropSqlGenerator : DbInjectionDropSqlGenerator<StoredProcedureDbInjection>
    {
        protected override string GenerateDropSql(StoredProcedureDbInjection dbInjection)
        {
            return $"DROP PROCEDURE [{dbInjection.ProcedureName}]";
        }
    }
}