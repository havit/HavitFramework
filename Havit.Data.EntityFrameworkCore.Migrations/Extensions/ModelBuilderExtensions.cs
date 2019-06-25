using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
// Správny namespace je Microsoft.EntityFrameworkCore! Konvencia Microsoftu.
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Havit.Data.EntityFrameworkCore.Migrations extension methods for <see cref="ModelBuilder"/>.
    /// </summary>
    public static class ModelBuilderExtensions
    {
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