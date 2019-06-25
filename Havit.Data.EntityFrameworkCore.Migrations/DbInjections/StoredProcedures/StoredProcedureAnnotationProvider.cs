using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections.StoredProcedures
{
    /// <summary>
    /// Implementation of <see cref="IDbInjectionAnnotationProvider"/>, that handles <see cref="StoredProcedureDbInjection"/>s.
    /// </summary>
    public class StoredProcedureAnnotationProvider : DbInjectionAnnotationProvider<StoredProcedureDbInjection>
    {
        private const string AnnotationPrefix = "StoredProcedure:";

        /// <inheritdoc />
        protected override List<IAnnotation> GetAnnotations(StoredProcedureDbInjection dbAnnotation, MemberInfo memberInfo)
        {
            return new List<IAnnotation>
            {
                new Annotation($"{AnnotationPrefix}{memberInfo.Name}:{dbAnnotation.ProcedureName}", dbAnnotation.CreateSql)
            };
        }

        /// <inheritdoc />
        protected override List<StoredProcedureDbInjection> GetDbInjections(List<IAnnotation> annotations)
        {
            var spAnnotations = annotations.Where(annotation => annotation.Name.StartsWith(AnnotationPrefix));

            return spAnnotations.Select(annotation => new StoredProcedureDbInjection
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