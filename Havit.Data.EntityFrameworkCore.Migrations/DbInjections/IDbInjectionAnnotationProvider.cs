using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    public interface IDbInjectionAnnotationProvider
    {
        List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo);

        List<IDbInjection> GetDbInjections(List<IAnnotation> annotations);
    }
}