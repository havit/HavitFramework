using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
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
        /// Registruje Model Extensions do modelu pomocou <paramref name="modelBuilder"/> z <paramref name="extensionsAssembly"/>
        /// </summary>
        /// <param name="modelBuilder"><see cref="ModelBuilder"/> pomocou ktorého sa zaregistrujú <see cref="IModelExtension"/> do modelu.</param>
        /// <param name="modelExtensionAnnotationProvider">Infraštruktúrna služba <see cref="IModelExtensionAnnotationProvider"/>.</param>
        /// <param name="extensionsAssembly"><see cref="Assembly"/>, v ktorej sú definované <see cref="IModelExtender"/>.</param>
        public static void ForModelExtensions(this ModelBuilder modelBuilder, IModelExtensionAnnotationProvider modelExtensionAnnotationProvider, Assembly extensionsAssembly)
        {
            Type[] modelExtenderTypes = ModelExtensionsTypeHelper.GetModelExtenders(extensionsAssembly).ToArray();

            ForModelExtensions(modelBuilder, modelExtensionAnnotationProvider, modelExtenderTypes);
        }

        /// <summary>
        /// Registruje DB Injections do modelu pomocou <paramref name="modelBuilder"/> priamym definovaním <see cref="IModelExtension"/> typov. Pre účely testovania.
        /// </summary>
        internal static void ForModelExtensions(this ModelBuilder modelBuilder, IModelExtensionAnnotationProvider modelExtensionAnnotationProvider, params Type[] modelExtenderTypes)
        {
            foreach (Type modelExtenderType in modelExtenderTypes)
            {
                object modelExtender = Activator.CreateInstance(modelExtenderType);
                IEnumerable<MethodInfo> publicMethods = modelExtenderType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => typeof(IModelExtension).IsAssignableFrom(m.ReturnType));

                foreach (MethodInfo method in publicMethods)
                {
                    var modelExtension = (IModelExtension)method.Invoke(modelExtender, new object[0]);
                    List<IAnnotation> annotations = modelExtensionAnnotationProvider.GetAnnotations(modelExtension, method);
                    modelBuilder.Model.AddAnnotations(annotations);
                }
            }
        }
    }
}