namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.Views
{
    /// <summary>
    /// Implementation of <see cref="IModelExtensionSqlGenerator"/>, that handles <see cref="ViewModelExtension"/>s.
    /// </summary>
    public class ViewSqlGenerator : ModelExtensionSqlGenerator<ViewModelExtension>
	{
        /// <inheritdoc />
        protected override string GenerateAlterSql(ViewModelExtension modelExtension)
		{
			return modelExtension.CreateSql.Replace("CREATE VIEW", "ALTER VIEW");
		}

        /// <inheritdoc />
        protected override string GenerateDropSql(ViewModelExtension modelExtension)
		{
			return $"DROP VIEW [{modelExtension.ViewName}]";
		}
	}
}