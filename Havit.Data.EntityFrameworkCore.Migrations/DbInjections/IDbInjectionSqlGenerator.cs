namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	public interface IDbInjectionSqlGenerator
	{
		string GenerateAlterSql(IDbInjection dbInjection);

		string GenerateDropSql(IDbInjection dbInjection);
	}
}