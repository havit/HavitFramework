using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// Service for resolving all ALTER and DROP SQL scripts from list of <see cref="IModelExtension"/>s.
	///
	/// Basically composite for <see cref="IModelExtensionSqlGenerator"/>.
	/// </summary>
	public interface IModelExtensionSqlResolver
	{
		/// <summary>
		/// Resolves all ALTER SQL scripts from list of <see cref="IModelExtension"/>s.
		/// </summary>
		List<string> ResolveAlterSqlScripts(List<IModelExtension> modelExtensions);

		/// <summary>
		/// Resolves all DROP SQL scripts from list of <see cref="IModelExtension"/>s.
		/// </summary>
		List<string> ResolveDropSqlScripts(List<IModelExtension> modelExtensions);
	}
}