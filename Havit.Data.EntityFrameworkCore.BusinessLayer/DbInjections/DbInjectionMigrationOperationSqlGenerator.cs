using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class DbInjectionMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
    {
        private readonly IDbInjectionAnnotationProvider dbInjectionAnnotationProvider;
        private readonly IDbInjectionDropSqlResolver dbInjectionDropSqlResolver;

        public DbInjectionMigrationOperationSqlGenerator(
            IDbInjectionAnnotationProvider dbInjectionAnnotationProvider,
            IDbInjectionDropSqlResolver dbInjectionDropSqlResolver)
        {
            this.dbInjectionAnnotationProvider = dbInjectionAnnotationProvider;
            this.dbInjectionDropSqlResolver = dbInjectionDropSqlResolver;
        }

        public override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
	        var oldAnnotations = GetAnnotations(operation.OldDatabase.GetAnnotations());
            var newAnnotations = GetAnnotations(operation.GetAnnotations());
            List<IAnnotation> deletedAnnotations = oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value).ToList<IAnnotation>();

            GenerateCreateAndUpdateCommands(newAnnotations.Values.ToList<IAnnotation>(), builder);
            GenerateDropCommands(deletedAnnotations, builder);
        }

	    private Dictionary<string, Annotation> GetAnnotations(IEnumerable<Annotation> annotations)
	    {
		    return annotations.Where(a => dbInjectionAnnotationProvider.GetDbInjections(new List<IAnnotation> { a }).Any())
			    .ToDictionary(a => a.Name);
	    }

	    private void GenerateDropCommands(List<IAnnotation> oldAnnotations, MigrationCommandListBuilder builder)
        {
            List<IDbInjection> dbInjections = dbInjectionAnnotationProvider.GetDbInjections(oldAnnotations);

            List<string> scripts = dbInjectionDropSqlResolver.ResolveSqlScripts(dbInjections);

            foreach (string sql in scripts)
            {
                builder.Append(sql).EndCommand();
            }
        }

        protected void GenerateCreateAndUpdateCommands(List<IAnnotation> newAnnotations, MigrationCommandListBuilder builder)
        {
            // Both for create and update
            foreach (var annotation in newAnnotations)
            {
                var sql = (string)annotation.Value;

                builder.Append(sql).EndCommand();
            }
        }
    }
}