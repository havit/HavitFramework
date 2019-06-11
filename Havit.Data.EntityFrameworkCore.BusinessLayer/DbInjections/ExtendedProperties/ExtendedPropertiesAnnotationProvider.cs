using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties
{
    public class ExtendedPropertiesAnnotationProvider : IDbInjectionAnnotationProvider
    {
        public List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(DbInjectionExtendedPropertiesAttribute), false).Cast<DbInjectionExtendedPropertiesAttribute>();
			return attributes.SelectMany(attr => ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(attr.GetExtendedProperties(memberInfo), attr.ObjectType, dbAnnotation.ObjectName))
                .ToList();
        }

        public List<IDbInjection> GetDbInjections(List<IAnnotation> annotations)
        {
            return new List<IDbInjection>();
        }
    }
}