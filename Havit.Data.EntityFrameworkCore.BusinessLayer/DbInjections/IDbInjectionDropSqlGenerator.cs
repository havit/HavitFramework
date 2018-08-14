namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public interface IDbInjectionDropSqlGenerator
    {
        string GenerateDropSql(IDbInjection dbInjection);
    }

    public abstract class DbInjectionDropSqlGenerator<T> : IDbInjectionDropSqlGenerator
        where T : IDbInjection
    {
        string IDbInjectionDropSqlGenerator.GenerateDropSql(IDbInjection dbInjection) => typeof(T).IsAssignableFrom(dbInjection?.GetType()) ? GenerateDropSql((T)dbInjection) : null;

        protected abstract string GenerateDropSql(T dbInjection);
    }
}