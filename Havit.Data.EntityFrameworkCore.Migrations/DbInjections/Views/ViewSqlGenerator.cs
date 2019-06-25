namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections.Views
{
    /// <summary>
    /// Implementation of <see cref="IDbInjectionSqlGenerator"/>, that handles <see cref="ViewDbInjection"/>s.
    /// </summary>
    public class ViewSqlGenerator : DbInjectionSqlGenerator<ViewDbInjection>
	{
        /// <inheritdoc />
        protected override string GenerateAlterSql(ViewDbInjection dbInjection)
		{
			return dbInjection.CreateSql.Replace("CREATE VIEW", "ALTER VIEW");
		}

        /// <inheritdoc />
        protected override string GenerateDropSql(ViewDbInjection dbInjection)
		{
			return $"DROP VIEW [{dbInjection.ViewName}]";
		}
	}
}