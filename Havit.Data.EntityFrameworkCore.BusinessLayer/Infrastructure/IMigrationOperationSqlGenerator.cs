using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	public interface IMigrationOperationSqlGenerator
	{
		void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder);

		void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder);

		void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder);

		void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder);

		void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder);

		void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder);
	}
}