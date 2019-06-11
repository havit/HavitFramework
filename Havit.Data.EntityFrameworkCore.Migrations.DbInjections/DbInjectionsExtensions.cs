using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
	/// <summary>
	/// Extension metódy pre konfiguráciu DB Injections.
	/// </summary>
	public static class DbInjectionsExtensions
	{
		/// <summary>
		/// Registruje služby používané podporou pre DB Injections. Je nutné, aby bola táto metóda volaná až po tom, ako boli zaregistrované infraštruktúrne služby pomocou <see cref="InfrastructureExtensions.UseCodeMigrationsInfrastructure"/>. Pomocou <paramref name="setupAction"/> je možné aktivovať rôzne funkčnosti DbInjections.
		/// </summary>
		public static DbContextOptionsBuilder UseDbInjections(this DbContextOptionsBuilder optionsBuilder, Action<DbInjectionsExtensionBuilder> setupAction = null)
        {
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

	        IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			var annotationProviderExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>();
			if (annotationProviderExtension == null)
			{
				throw new InvalidOperationException("Necessary extension (CompositeMigrationsAnnotationProviderExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
			}

	        builder.AddOrUpdateExtension(annotationProviderExtension.WithAnnotationProvider<DbInjectionsMigrationsAnnotationProvider>());

			var sqlGeneratorExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>();
			if (sqlGeneratorExtension == null)
			{
				throw new InvalidOperationException("Necessary extension (CompositeMigrationsSqlGeneratorExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
			}
			builder.AddOrUpdateExtension(sqlGeneratorExtension.WithGeneratorType<DbInjectionMigrationOperationSqlGenerator>());

	        setupAction?.Invoke(new DbInjectionsExtensionBuilder(optionsBuilder));

	        return optionsBuilder;
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

	        ForDbInjections(modelBuilder, dbInjectionAnnotationProvider, dbInjectorTypes);
        }

		/// <summary>
		/// Registruje DB Injections do modelu pomocou <paramref name="modelBuilder"/> priamym definovaním DbInjector typov. Pre účely testovania.
		/// </summary>
		internal static void ForDbInjections(this ModelBuilder modelBuilder, IDbInjectionAnnotationProvider dbInjectionAnnotationProvider, params Type[] dbInjectorTypes)
		{
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