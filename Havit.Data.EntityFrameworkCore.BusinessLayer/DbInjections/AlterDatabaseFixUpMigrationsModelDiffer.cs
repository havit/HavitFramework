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
	public class AlterDatabaseFixUpMigrationsModelDiffer : MigrationsModelDiffer
	{
		public AlterDatabaseFixUpMigrationsModelDiffer(
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
				else
				{
					yield return migrationOperation;
				}
			}
		}

		private MigrationOperation FixAlterDatabaseOperation(AlterDatabaseOperation originalOperation)
		{
			var alterDatabaseOperation = new AlterDatabaseOperation();

			var annotations = originalOperation.GetAnnotations().ToDictionary(a => (a.Name, a.Value));
			var oldAnnotations = originalOperation.OldDatabase.GetAnnotations().ToDictionary(a => (a.Name, a.Value));

			foreach (Annotation annotation in annotations.Values.ToArray())
			{
				if (oldAnnotations.ContainsKey((annotation.Name, annotation.Value)) &&
				    annotations.ContainsKey((annotation.Name, annotation.Value)))
				{
					annotations.Remove((annotation.Name, annotation.Value));
					oldAnnotations.Remove((annotation.Name, annotation.Value));
				}
			}

			alterDatabaseOperation.AddAnnotations(annotations.Values);
			alterDatabaseOperation.OldDatabase.AddAnnotations(oldAnnotations.Values);

			return alterDatabaseOperation;
		}
	}
}