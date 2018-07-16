using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Business.CodeMigrations
{
	internal class ExtendedPropertiesMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
	{
		public ExtendedPropertiesMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IMigrationsAnnotationProvider migrationsAnnotations)
			: base(dependencies, migrationsAnnotations)
		{ }
	}
}
