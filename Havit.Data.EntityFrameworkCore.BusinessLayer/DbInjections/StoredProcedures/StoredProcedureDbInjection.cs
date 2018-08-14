namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures
{
    public class StoredProcedureDbInjection : IDbInjection
    {
        public string CreateSql { get; set; }

        public string ProcedureName { get; set; }

        string IDbInjection.ObjectName => ProcedureName;
    }
}