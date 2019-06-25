using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	/// <summary>
	/// Service for resolving all ALTER and DROP SQL scripts from list of <see cref="IDbInjection"/>s.
	///
	/// Basically composite for <see cref="IDbInjectionSqlGenerator"/>.
	/// </summary>
	public interface IDbInjectionSqlResolver
	{
        /// <summary>
        /// Resolves all ALTER SQL scripts from list of <see cref="IDbInjection"/>s.
        /// </summary>
        List<string> ResolveAlterSqlScripts(List<IDbInjection> dbInjections);

        /// <summary>
        /// Resolves all DROP SQL scripts from list of <see cref="IDbInjection"/>s.
        /// </summary>
        List<string> ResolveDropSqlScripts(List<IDbInjection> dbInjections);
	}
}