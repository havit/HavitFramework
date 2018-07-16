using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Business.CodeMigrations
{
	internal class ExtendedPropertiesMigrationsAnnotationProvider : SqlServerMigrationsAnnotationProvider
	{
		public ExtendedPropertiesMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }
	}
}
