namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <summary>
/// Service that generates ALTER and DROP SQL scripts for <see cref="IModelExtension"/>.
/// </summary>
public interface IModelExtensionSqlGenerator
{
	/// <summary>
	/// Generates ALTER SQL script for <see cref="IModelExtension"/>.
	/// </summary>
	string GenerateAlterSql(IModelExtension modelExtension);

	/// <summary>
	/// Generates DROP SQL script for <see cref="IModelExtension"/>.
	/// </summary>
	string GenerateDropSql(IModelExtension modelExtension);
}