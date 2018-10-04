using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	public class MigrationOperationSqlGenerator : IMigrationOperationSqlGenerator
	{
		public virtual void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
		}

		public virtual void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
		}

		public virtual void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
		}

		public virtual void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
		}

		public virtual void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
		}

		public virtual void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
		}
	}
}