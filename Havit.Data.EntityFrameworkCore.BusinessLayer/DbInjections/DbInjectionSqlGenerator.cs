namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	public abstract class DbInjectionSqlGenerator<T> : IDbInjectionSqlGenerator
		where T : IDbInjection
	{
		string IDbInjectionSqlGenerator.GenerateDropSql(IDbInjection dbInjection) => IsMatchingDbInjection(dbInjection) ? GenerateDropSql((T)dbInjection) : null;

		string IDbInjectionSqlGenerator.GenerateAlterSql(IDbInjection dbInjection) => IsMatchingDbInjection(dbInjection) ? GenerateAlterSql((T)dbInjection) : null;

		protected abstract string GenerateDropSql(T dbInjection);

		protected abstract string GenerateAlterSql(T dbInjection);

		private static bool IsMatchingDbInjection(IDbInjection dbInjection) => typeof(T).IsAssignableFrom(dbInjection?.GetType());
	}
}