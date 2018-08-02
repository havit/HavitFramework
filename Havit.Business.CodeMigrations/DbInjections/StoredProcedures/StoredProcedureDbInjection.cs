﻿namespace Havit.Business.CodeMigrations.DbInjections.StoredProcedures
{
    public class StoredProcedureDbInjection : IDbInjection
    {
        public string CreateSql { get; set; }

        public string ProcedureName { get; set; }

        string IDbInjection.ObjectName => ProcedureName;
    }
}