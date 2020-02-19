using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures
{
    /// <summary>
    /// Implementation of <see cref="IModelExtensionAnnotationProvider"/>, that handles <see cref="StoredProcedureModelExtension"/>s.
    /// </summary>
    /// <remarks>
    /// Warning! If annotation structure (name or value) is changed (as part of breaking change), don't forget to update/rewrite tests.
    /// </remarks>
    public class StoredProcedureAnnotationProvider : ModelExtensionAnnotationProvider<StoredProcedureModelExtension>
    {
        private const string AnnotationPrefix = "StoredProcedure:";

        /// <inheritdoc />
        protected override List<IAnnotation> GetAnnotations(StoredProcedureModelExtension dbAnnotation, MemberInfo memberInfo)
        {
            return new List<IAnnotation>
            {
                new Annotation($"{AnnotationPrefix}{memberInfo.Name}:{dbAnnotation.ProcedureName}", dbAnnotation.CreateSql)
            };
        }

        /// <inheritdoc />
        protected override List<StoredProcedureModelExtension> GetModelExtensions(List<IAnnotation> annotations)
        {
            var spAnnotations = annotations.Where(annotation => annotation.Name.StartsWith(AnnotationPrefix));

            return spAnnotations.Select(annotation => new StoredProcedureModelExtension
            {
                CreateSql = (string)annotation.Value,
                ProcedureName = ParseProcedureName(annotation)
            }).ToList();
        }

        private string ParseProcedureName(IAnnotation annotation)
        {
            return annotation.Name.Split(':').Last();
        }
    }
}