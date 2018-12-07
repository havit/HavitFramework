namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.Views
{
	public class ViewSqlGenerator : DbInjectionSqlGenerator<ViewDbInjection>
	{
		protected override string GenerateAlterSql(ViewDbInjection dbInjection)
		{
			return dbInjection.CreateSql.Replace("CREATE VIEW", "ALTER VIEW");
		}

		protected override string GenerateDropSql(ViewDbInjection dbInjection)
		{
			return $"DROP VIEW [{dbInjection.ViewName}]";
		}
	}
}