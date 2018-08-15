﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	public class CompositeMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly IEnumerable<IMigrationOperationSqlGenerator> operationGenerators;

	    public CompositeMigrationsSqlGenerator(
		    MigrationsSqlGeneratorDependencies dependencies,
		    IMigrationsAnnotationProvider migrationsAnnotations,
		    IEnumerable<IMigrationOperationSqlGenerator> operationGenerators) 
		    : base(dependencies, migrationsAnnotations)
	    {
		    this.operationGenerators = operationGenerators;
	    }

        protected override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
		    base.Generate(operation, model, builder);

		    RunOnGenerators(generator => generator.Generate(operation, model, builder));
	    }

	    protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
		    base.Generate(operation, model, builder);

		    RunOnGenerators(generator => generator.Generate(operation, model, builder));
		}

	    protected override void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
		    base.Generate(operation, model, builder);

		    RunOnGenerators(generator => generator.Generate(operation, model, builder));
		}

	    protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
		    base.Generate(operation, model, builder);

		    RunOnGenerators(generator => generator.Generate(operation, model, builder));
		}

	    protected override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
		    base.Generate(operation, model, builder);

		    RunOnGenerators(generator => generator.Generate(operation, model, builder));
		}

	    protected override void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
	    {
		    base.Generate(operation, model, builder);

		    RunOnGenerators(generator => generator.Generate(operation, model, builder));
		}

	    private void RunOnGenerators(Action<IMigrationOperationSqlGenerator> action)
	    {
		    foreach (IMigrationOperationSqlGenerator generator in operationGenerators)
		    {
			    action(generator);
		    }
	    }
    }
}