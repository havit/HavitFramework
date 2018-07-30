using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Business.CodeMigrations.DbInjections
{
    public class StoredProcedureAnnotationProvider : DbInjectionAnnotationProvider<StoredProcedureDbInjection>
    {
        protected override List<IAnnotation> GetAnnotations(StoredProcedureDbInjection dbAnnotation, MemberInfo memberInfo)
        {
            return new List<IAnnotation>
            {
                new Annotation($"StoredProcedure:{memberInfo.Name}:{dbAnnotation.ProcedureName}", dbAnnotation.CreateSql)
            };
        }

        protected override List<StoredProcedureDbInjection> GetDbInjections(List<IAnnotation> annotations)
        {
            var spAnnotations = annotations.Where(annotation => annotation.Name.StartsWith("StoredProcedure:"));

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