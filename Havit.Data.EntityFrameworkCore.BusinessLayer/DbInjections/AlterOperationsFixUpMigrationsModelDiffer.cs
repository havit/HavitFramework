using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	public class AlterOperationsFixUpMigrationsModelDiffer : MigrationsModelDiffer
	{
		public AlterOperationsFixUpMigrationsModelDiffer(
			IRelationalTypeMappingSource typeMappingSource,
			IMigrationsAnnotationProvider migrationsAnnotations,
			IChangeDetector changeDetector,
			StateManagerDependencies stateManagerDependencies,
			CommandBatchPreparerDependencies commandBatchPreparerDependencies) 
			: base(typeMappingSource,
			migrationsAnnotations,
			changeDetector,
			stateManagerDependencies,
			commandBatchPreparerDependencies)
		{
		}

		protected override IEnumerable<MigrationOperation> Diff(IModel source, IModel target, DiffContext diffContext)
		{
			foreach (MigrationOperation migrationOperation in base.Diff(source, target, diffContext))
			{
				if (migrationOperation is AlterDatabaseOperation alterDatabaseOperation)
				{
					yield return FixAlterDatabaseOperation(alterDatabaseOperation);
				}
                else if (migrationOperation is AlterTableOperation alterTableOperation)
                {
                    yield return FixAlterTableOperation(alterTableOperation);
                }
                else
				{
					yield return migrationOperation;
				}
			}
		}

        private MigrationOperation FixAlterTableOperation(AlterTableOperation originalOperation)
        {
            (IEnumerable<Annotation> currentAnnotations, IEnumerable<Annotation> oldAnnotations) = RemoveDuplicateAnnotations(originalOperation, originalOperation.OldTable);
            AlterTableOperation alterTableOperation = new AlterTableOperation
            {
                Name = originalOperation.Name,
                Schema = originalOperation.Schema,
                IsDestructiveChange = originalOperation.IsDestructiveChange
            };
            alterTableOperation.AddAnnotations(currentAnnotations);
            alterTableOperation.OldTable.AddAnnotations(oldAnnotations);

            return alterTableOperation;
        }

        private MigrationOperation FixAlterDatabaseOperation(AlterDatabaseOperation originalOperation)
		{
            (IEnumerable<Annotation> currentAnnotations, IEnumerable<Annotation> oldAnnotations) = RemoveDuplicateAnnotations(originalOperation, originalOperation.OldDatabase);
			var alterDatabaseOperation = new AlterDatabaseOperation();
            alterDatabaseOperation.AddAnnotations(currentAnnotations);
            alterDatabaseOperation.OldDatabase.AddAnnotations(oldAnnotations);

            return alterDatabaseOperation;
		}

        private static (IEnumerable<Annotation>, IEnumerable<Annotation>) RemoveDuplicateAnnotations(Annotatable currentAnnotatable, Annotatable oldAnnotatable)
        {
            // leverages ValueTuples and GetHashCode/Equals implementation for removing duplicates
            var annotations = currentAnnotatable.GetAnnotations().ToDictionary(a => (a.Name, a.Value));
            var oldAnnotations = oldAnnotatable.GetAnnotations().ToDictionary(a => (a.Name, a.Value));

            foreach (Annotation annotation in annotations.Values.ToArray())
            {
                if (oldAnnotations.ContainsKey((annotation.Name, annotation.Value)) &&
                    annotations.ContainsKey((annotation.Name, annotation.Value)))
                {
                    annotations.Remove((annotation.Name, annotation.Value));
                    oldAnnotations.Remove((annotation.Name, annotation.Value));
                }
            }

            return (annotations.Values, oldAnnotations.Values);
        }
    }
}