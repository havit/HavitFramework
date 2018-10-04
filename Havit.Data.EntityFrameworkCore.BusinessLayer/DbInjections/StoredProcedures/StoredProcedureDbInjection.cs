namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures
{
	/// <summary>
	/// <see cref="IDbInjection"/> objekt pre uloženú procedúru.
	/// </summary>
    public class StoredProcedureDbInjection : IDbInjection
    {
		/// <summary>
		/// Create skript pre založenie uloženej procedúry.
		/// </summary>
        public string CreateSql { get; set; }

		/// <summary>
		/// Názov procedúry.
		/// </summary>
        public string ProcedureName { get; set; }

	    /// <inheritdoc />
	    string IDbInjection.ObjectName => ProcedureName;
    }
}