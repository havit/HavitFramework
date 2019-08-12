namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.Views
{
	/// <summary>
	/// <see cref="IModelExtension"/> objekt pre view.
	/// </summary>
    public class ViewModelExtension : IModelExtension
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
	    string IModelExtension.ObjectName => ViewName;
    }
}