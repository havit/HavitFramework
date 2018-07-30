namespace Havit.Business.CodeMigrations.DbInjections
{
    public class StoredProcedureDbInjection : IDbInjection
    {
        public string CreateSql { get; set; }

        public string ProcedureName { get; set; }
    }
}