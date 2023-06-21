namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;

/// <summary>
/// Implementation of <see cref="IModelExtensionSqlGenerator"/>, that handles <see cref="StoredProcedureModelExtension"/>s.
/// </summary>
public class StoredProcedureSqlGenerator : ModelExtensionSqlGenerator<StoredProcedureModelExtension>
{
	/// <inheritdoc />
	protected override string GenerateAlterSql(StoredProcedureModelExtension modelExtension)
	{
		return modelExtension.CreateSql.Replace("CREATE PROCEDURE", "ALTER PROCEDURE");
	}

	/// <inheritdoc />
	protected override string GenerateDropSql(StoredProcedureModelExtension modelExtension)
	{
		return $"DROP PROCEDURE [{modelExtension.ProcedureName}]";
	}
}