using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.ModelExtensions;

public class TestModelExtender : StoredProcedureModelExtender
{
	private static readonly string CreateSql = @"CREATE PROCEDURE [dbo].[DummyProcedure]
AS
BEGIN
	-- Dummy
END
";

	public StoredProcedureModelExtension DummyStoredProcedure()
	{
		return new StoredProcedureModelExtension
		{
			CreateSql = CreateSql,
			ProcedureName = ParseProcedureName("[dbo].[DummyProcedure]")
		};
	}
}