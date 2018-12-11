namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	public interface IDbInjectionSqlGenerator
	{
		string GenerateAlterSql(IDbInjection dbInjection);

		string GenerateDropSql(IDbInjection dbInjection);
	}
}