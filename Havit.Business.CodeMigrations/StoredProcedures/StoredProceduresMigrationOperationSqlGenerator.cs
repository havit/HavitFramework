using System.Linq;
using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Business.CodeMigrations.StoredProcedures
{
    public class StoredProceduresMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
	{
		public MigrationsSqlGeneratorDependencies Dependencies { get; }

		public StoredProceduresMigrationOperationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies)
		{
			Dependencies = dependencies;
		}

		public override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			var oldAnnotations = operation.OldDatabase.GetAnnotations().Where(StoredProceduresAnnotationsHelper.IsStoredProcedureAnnotation).ToDictionary(x => x.Name, StoredProceduresAnnotationsHelper.Comparer);
			var newAnnotations = operation.GetAnnotations().Where(StoredProceduresAnnotationsHelper.IsStoredProcedureAnnotation).ToDictionary(x => x.Name, StoredProceduresAnnotationsHelper.Comparer);

		    foreach (var annotation in oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
		    {
			    var name = StoredProceduresAnnotationsHelper.ParseAnnotationName(annotation);
			    DropStoredProcedure(model.Relational().DefaultSchema ?? "dbo", name, builder);
		    }

			// Both for create and update
			foreach (var annotation in newAnnotations.Select(p => p.Value))
	        {
	            var sql = (string)annotation.Value;

	            builder.Append(sql).EndCommand();
	        }
	    }

		private void DropStoredProcedure(string schema, string name, MigrationCommandListBuilder builder)
		{
			builder
				.Append($"DROP PROCEDURE [{schema}].[{name}]")
				.EndCommand();
		}
	}
}