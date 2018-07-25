using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Business.CodeMigrations.Infrastructure
{
    public class CompositeMigrationsSqlGenerator : IMigrationsSqlGenerator
    {
        private readonly IEnumerable<IMigrationsSqlGenerator> generators;

        public CompositeMigrationsSqlGenerator(IEnumerable<IMigrationsSqlGenerator> generators)
        {
            this.generators = generators;
        }

        public IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
        {
            var commands = new List<MigrationCommand>();

            foreach (IMigrationsSqlGenerator migrationsSqlGenerator in generators)
            {
                commands.AddRange(migrationsSqlGenerator.Generate(operations, model));
            }

            return commands;
        }
    }
}