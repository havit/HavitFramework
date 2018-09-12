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
        private readonly IDbInjectionSqlResolver dbInjectionSqlResolver;

        public DbInjectionMigrationOperationSqlGenerator(
            IDbInjectionAnnotationProvider dbInjectionAnnotationProvider,
            IDbInjectionSqlResolver dbInjectionSqlResolver)
        {
            this.dbInjectionAnnotationProvider = dbInjectionAnnotationProvider;
            this.dbInjectionSqlResolver = dbInjectionSqlResolver;
        }

        public override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
	        var oldAnnotations = GetAnnotations(operation.OldDatabase.GetAnnotations());
            var currentAnnotations = GetAnnotations(operation.GetAnnotations());
	        var newAnnotations = currentAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value).ToList<IAnnotation>();
            List<IAnnotation> deletedAnnotations = oldAnnotations.Where(x => !currentAnnotations.ContainsKey(x.Key)).Select(x => x.Value).ToList<IAnnotation>();
            List<IAnnotation> existingAnnotations = oldAnnotations.Where(x => currentAnnotations.ContainsKey(x.Key)).Select(x => currentAnnotations[x.Key]).ToList<IAnnotation>();

            GenerateCreateCommands(newAnnotations, builder);
			GenerateAlterCommands(existingAnnotations, builder);
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

            List<string> scripts = dbInjectionSqlResolver.ResolveDropSqlScripts(dbInjections);

            foreach (string sql in scripts)
            {
                builder.Append(sql).EndCommand();
            }
        }

	    private void GenerateAlterCommands(List<IAnnotation> existingAnnotations, MigrationCommandListBuilder builder)
        {
            List<IDbInjection> dbInjections = dbInjectionAnnotationProvider.GetDbInjections(existingAnnotations);

            List<string> scripts = dbInjectionSqlResolver.ResolveAlterSqlScripts(dbInjections);

            foreach (string sql in scripts)
            {
                builder.Append(sql).EndCommand();
            }
        }

        protected void GenerateCreateCommands(List<IAnnotation> newAnnotations, MigrationCommandListBuilder builder)
        {
            foreach (var annotation in newAnnotations)
            {
                var sql = (string)annotation.Value;

                builder.Append(sql).EndCommand();
            }
        }
    }
}