using System.Collections.Generic;
using System.Linq;
using Havit.Business.CodeMigrations.StoredProcedures;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Business.CodeMigrations.DbInjections
{
    public class DbInjectionMigrationsGenerator : IMigrationsSqlGenerator
    {
        private readonly IDbInjectionAnnotationProvider dbInjectionAnnotationProvider;
        private readonly IDbInjectionDropSqlResolver injectionDropSqlResolver;

        public MigrationsSqlGeneratorDependencies Dependencies { get; }

        public DbInjectionMigrationsGenerator(MigrationsSqlGeneratorDependencies dependencies)
        {
            Dependencies = dependencies;
            dbInjectionAnnotationProvider = new CompositeDbInjectionAnnotationProvider(new[]
            {
                new StoredProcedureAnnotationProvider()
            });
            injectionDropSqlResolver = new DbInjectionDropSqlResolver(new[]
            {
                new StoredProcedureDropSqlGenerator()
            });
            
        }

        public IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
        {
            MigrationCommandListBuilder builder = new MigrationCommandListBuilder(Dependencies.CommandBuilderFactory);
            AlterDatabaseOperation alterDatabaseOperation = operations.OfType<AlterDatabaseOperation>().FirstOrDefault();
            if (alterDatabaseOperation != null)
            {
                var oldAnnotations = alterDatabaseOperation.OldDatabase.GetAnnotations().Where(StoredProceduresAnnotationsHelper.IsStoredProcedureAnnotation).ToDictionary(x => x.Name, StoredProceduresAnnotationsHelper.Comparer);
                var newAnnotations = alterDatabaseOperation.GetAnnotations().Where(StoredProceduresAnnotationsHelper.IsStoredProcedureAnnotation).ToDictionary(x => x.Name, StoredProceduresAnnotationsHelper.Comparer);
                List<IAnnotation> deletedAnnotations = oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value).ToList<IAnnotation>();

                GenerateCreateAndUpdateCommands(newAnnotations.Values.ToList<IAnnotation>(), builder);
                GenerateDropCommands(deletedAnnotations, builder);
            }

            return builder.GetCommandList();
        }

        private void GenerateDropCommands(List<IAnnotation> oldAnnotations, MigrationCommandListBuilder builder)
        {
            List<IDbInjection> dbInjections = dbInjectionAnnotationProvider.GetDbInjections(oldAnnotations);

            List<string> scripts = injectionDropSqlResolver.ResolveSqlScripts(dbInjections);

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