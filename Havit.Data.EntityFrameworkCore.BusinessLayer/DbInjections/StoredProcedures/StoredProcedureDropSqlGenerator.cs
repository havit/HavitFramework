namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures
{
    public class StoredProcedureDropSqlGenerator : DbInjectionDropSqlGenerator<StoredProcedureDbInjection>
    {
        protected override string GenerateDropSql(StoredProcedureDbInjection dbInjection)
        {
            return $"DROP PROCEDURE [{dbInjection.ProcedureName}]";
        }
    }
}