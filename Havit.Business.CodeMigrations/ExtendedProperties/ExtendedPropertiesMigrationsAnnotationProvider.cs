using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal class ExtendedPropertiesMigrationsAnnotationProvider : SqlServerMigrationsAnnotationProvider
	{
		public ExtendedPropertiesMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }
	}
}
