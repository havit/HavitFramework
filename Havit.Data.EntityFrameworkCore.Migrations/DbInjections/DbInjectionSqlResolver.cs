using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	public class DbInjectionSqlResolver : IDbInjectionSqlResolver
	{
		private readonly IEnumerable<IDbInjectionSqlGenerator> sqlGenerators;

		public DbInjectionSqlResolver(IEnumerable<IDbInjectionSqlGenerator> sqlGenerators)
		{
			this.sqlGenerators = sqlGenerators;
		}

		public List<string> ResolveAlterSqlScripts(List<IDbInjection> dbInjections)
		{
			return CollectSqlScripts(dbInjections, ((generator, injection) => generator.GenerateAlterSql(injection)));
		}

		public List<string> ResolveDropSqlScripts(List<IDbInjection> dbInjections)
		{
			return CollectSqlScripts(dbInjections, ((generator, injection) => generator.GenerateDropSql(injection)));
		}

		private List<string> CollectSqlScripts(List<IDbInjection> dbInjections, Func<IDbInjectionSqlGenerator, IDbInjection, string> sqlProvider)
		{
			var list = new List<string>();

			foreach (IDbInjection dbInjection in dbInjections)
			{
				IEnumerable<string> sqlStatements = sqlGenerators
					.Select(g => sqlProvider(g, dbInjection))
					.Where(s => s != null);

				list.AddRange(sqlStatements);
			}

			return list;
		}
	}
}