namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.Views
{
	/// <summary>
	/// <see cref="IDbInjection"/> objekt pre view.
	/// </summary>
    public class ViewDbInjection : IDbInjection
    {
		/// <summary>
		/// Create skript pre založenie viewu.
		/// </summary>
        public string CreateSql { get; set; }

		/// <summary>
		/// Názov procedúry.
		/// </summary>
        public string ViewName { get; set; }

	    /// <inheritdoc />
	    string IDbInjection.ObjectName => ViewName;
    }
}