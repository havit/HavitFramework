using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Business.CodeMigrations.DbInjections.ExtendedProperties;
using Havit.Business.CodeMigrations.DbInjections.StoredProcedures;
using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Business.CodeMigrations.DbInjections
{
    public static class DbInjectionsExtensions
    {
        public static void UseDbInjections(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.Options.GetExtension<CompositeMigrationsAnnotationProviderExtension>().WithAnnotationProvider<DbInjectionsMigrationsAnnotationProvider>();
            optionsBuilder.Options.GetExtension<CompositeMigrationsSqlGeneratorExtension>().WithGeneratorType<DbInjectionMigrationOperationSqlGenerator>();

            IDbContextOptionsBuilderInfrastructure infrastructure = optionsBuilder;
            infrastructure.AddOrUpdateExtension(new DbInjectionsExtension()
                .WithAnnotationProvider<StoredProcedureAnnotationProvider>()
                .WithDropSqlGenerator<StoredProcedureDropSqlGenerator>()
                .WithAnnotationProvider<ExtendedPropertiesAnnotationProvider>()
				.WithAnnotationProvider<StoredProcedureAttachPropertyAnnotationProvider>()
			);
        }

        public static void ForDbInjections(this ModelBuilder modelBuilder, IDbInjectionAnnotationProvider dbInjectionAnnotationProvider, Assembly injectionsAssembly)
        {
            Type[] dbInjectorTypes = DbInjectionsTypeHelper.GetDbInjectors(injectionsAssembly).ToArray();

            foreach (Type dbInjectorType in dbInjectorTypes)
            {
                object dbInjector = Activator.CreateInstance(dbInjectorType);
                IEnumerable<MethodInfo> publicMethods = dbInjectorType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => typeof(IDbInjection).IsAssignableFrom(m.ReturnType));

                foreach (MethodInfo method in publicMethods)
                {
                    var dbInjection = (IDbInjection)method.Invoke(dbInjector, new object[0]);
                    List<IAnnotation> annotations = dbInjectionAnnotationProvider.GetAnnotations(dbInjection, method);
                    modelBuilder.Model.AddAnnotations(annotations);
                }
            }
        }
    }
}