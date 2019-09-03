using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// A service for generating <see cref="MigrationCommand" /> objects that can then be executed or scripted from a list of <see cref="MigrationOperation" />s.
    ///
    /// <para>Each <see cref="MigrationOperation"/> has separate method.</para>
    /// </summary>
    public interface IMigrationOperationSqlGenerator
    {
        /// <summary>
        /// Builds commands for the given <see cref="CreateTableOperation" /> by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
		void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder);

        /// <summary>
        /// Builds commands for the given <see cref="AddColumnOperation" /> by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder);

        /// <summary>
        /// Builds commands for the given <see cref="AlterTableOperation" /> by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder);

        /// <summary>
        /// Builds commands for the given <see cref="AlterColumnOperation" /> by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder);

        /// <summary>
        /// Builds commands for the given <see cref="AlterDatabaseOperation" /> by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder);

        /// <summary>
        /// Builds commands for the given <see cref="SqlServerCreateDatabaseOperation" /> by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder);
	}
}