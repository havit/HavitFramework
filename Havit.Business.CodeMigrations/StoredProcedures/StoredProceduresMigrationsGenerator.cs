using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Business.CodeMigrations.StoredProcedures
{
	public class StoredProceduresMigrationsGenerator : IMigrationsSqlGenerator
	{
	    private readonly IRelationalCommandBuilderFactory relationalCommandBuilderFactory;

	    public StoredProceduresMigrationsGenerator(IRelationalCommandBuilderFactory relationalCommandBuilderFactory)
	    {
	        this.relationalCommandBuilderFactory = relationalCommandBuilderFactory;
	    }

	    public IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
	    {
	        MigrationCommandListBuilder builder = new MigrationCommandListBuilder(relationalCommandBuilderFactory);
	        AlterDatabaseOperation alterDatabaseOperation = operations.OfType<AlterDatabaseOperation>().FirstOrDefault();
	        if (alterDatabaseOperation != null)
	        {
	            Generate(alterDatabaseOperation, model, builder);
	        }

	        return builder.GetCommandList();
        }

	    private static void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
	        var newAnnotations = operation.GetAnnotations().Where(StoredProceduresAnnotationsHelper.IsStoredProcedureAnnotation).ToDictionary(x => x.Name, StoredProceduresAnnotationsHelper.Comparer);

	        foreach (var annotation in newAnnotations.Select(p => p.Value))
	        {
	            var sql = (string)annotation.Value;

	            builder.Append(sql).EndCommand();
	        }
	    }
	}
}