using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Business.CodeMigrations.DbInjections.ExtendedProperties
{
    public class ExtendedPropertiesAnnotationProvider : IDbInjectionAnnotationProvider
    {
        public List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(DbInjectionExtendedPropertiesAttribute), false).Cast<DbInjectionExtendedPropertiesAttribute>();
            return attributes.SelectMany(attr =>
                attr.GetExtendedProperties(memberInfo).Select(p =>
                    new Annotation($"ExtendedProperty:{attr.ObjectType}:{dbAnnotation.ObjectName}:{p.Key}", p.Value)))
                .ToList<IAnnotation>();
        }

        public List<IDbInjection> GetDbInjections(List<IAnnotation> annotations)
        {
            return new List<IDbInjection>();
        }
    }
}