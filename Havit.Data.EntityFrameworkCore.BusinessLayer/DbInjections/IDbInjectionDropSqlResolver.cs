using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public interface IDbInjectionDropSqlResolver
    {
        List<string> ResolveSqlScripts(List<IDbInjection> dbInjections);
    }

    public class DbInjectionDropSqlResolver : IDbInjectionDropSqlResolver
    {
        private readonly IEnumerable<IDbInjectionDropSqlGenerator> dropSqlGenerators;

        public DbInjectionDropSqlResolver(IEnumerable<IDbInjectionDropSqlGenerator> dropSqlGenerators)
        {
            this.dropSqlGenerators = dropSqlGenerators;
        }

        public List<string> ResolveSqlScripts(List<IDbInjection> dbInjections)
        {
            var list = new List<string>();

            foreach (var dbInjection in dbInjections)
            {
                IEnumerable<string> sqlStatements = dropSqlGenerators
                    .Select(g => g.GenerateDropSql(dbInjection))
                    .Where(s => s != null);

                list.AddRange(sqlStatements);
            }

            return list;
        }
    }
}