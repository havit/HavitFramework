namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures
{
	/// <summary>
	/// <see cref="IModelExtension"/> objekt pre uloženú procedúru.
	/// </summary>
    public class StoredProcedureModelExtension : IModelExtension
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
	    string IModelExtension.ObjectName => ProcedureName;
    }
}