using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.Views;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	/// <summary>
	/// Extension metódy pre konfiguráciu DB Injections.
	/// </summary>
	public static class DbInjectionsExtensions
	{
		/// <summary>
		/// Registruje služby používané podporou pre DB Injections. Je nutné, aby bola táto metóda volaná až po tom, ako boli zaregistrované infraštruktúrne služby pomocou <see cref="InfrastructureExtensions.UseCodeMigrationsInfrastructure"/>.
		/// </summary>
		public static void UseDbInjections(this DbContextOptionsBuilder optionsBuilder, Action<DbInjectionsOptions> setupAction = null)
        {
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

	        IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(
				optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>()
			        .WithAnnotationProvider<DbInjectionsMigrationsAnnotationProvider>());
	        builder.AddOrUpdateExtension(
		        optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
			        .WithGeneratorType<DbInjectionMigrationOperationSqlGenerator>());

	        var options = new DbInjectionsOptions();
	        setupAction?.Invoke(options);

	        builder.AddOrUpdateExtension(new DbInjectionsExtension()
                .WithAnnotationProvider<StoredProcedureAnnotationProvider>()
                .WithSqlGenerator<StoredProcedureSqlGenerator>()
                .WithAnnotationProvider<ExtendedPropertiesAnnotationProvider>()
				.WithAnnotationProvider<StoredProcedureAttachPropertyAnnotationProvider>()
				.WithAnnotationProvider<StoredProcedureMsDescriptionPropertyAnnotationProvider>()
		        .WithAnnotationProvider<ViewAnnotationProvider>()
		        .WithSqlGenerator<ViewSqlGenerator>()
		        .WithOptions(options)
			);
        }

		/// <summary>
		/// Registruje DB Injections do modelu pomocou <paramref name="modelBuilder"/> z <paramref name="injectionsAssembly"/>
		/// </summary>
		/// <param name="modelBuilder"><see cref="ModelBuilder"/> pomocou ktorého sa zaregistrujú DB Injections do modelu.</param>
		/// <param name="dbInjectionAnnotationProvider">Infraštruktúrna služba <see cref="IDbInjectionAnnotationProvider"/>.</param>
		/// <param name="injectionsAssembly"><see cref="Assembly"/>, v ktorej sú definované DB Injectory.</param>
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