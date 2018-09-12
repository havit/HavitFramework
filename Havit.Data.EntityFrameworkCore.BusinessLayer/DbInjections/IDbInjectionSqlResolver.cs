using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	public interface IDbInjectionSqlResolver
	{
		List<string> ResolveAlterSqlScripts(List<IDbInjection> dbInjections);

		List<string> ResolveDropSqlScripts(List<IDbInjection> dbInjections);
	}
}