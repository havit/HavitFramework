using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public interface IDbInjectionAnnotationProvider
    {
        List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo);

        List<IDbInjection> GetDbInjections(List<IAnnotation> annotations);
    }

    public abstract class DbInjectionAnnotationProvider<T> : IDbInjectionAnnotationProvider
        where T : IDbInjection
    {
        List<IAnnotation> IDbInjectionAnnotationProvider.GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo) => 
            dbAnnotation is T dbAnnotationT ? GetAnnotations(dbAnnotationT, memberInfo) : null;

        List<IDbInjection> IDbInjectionAnnotationProvider.GetDbInjections(List<IAnnotation> annotations) => 
            GetDbInjections(annotations).Cast<IDbInjection>().ToList();

        protected abstract List<IAnnotation> GetAnnotations(T dbAnnotation, MemberInfo memberInfo);

        protected abstract List<T> GetDbInjections(List<IAnnotation> annotations);
    }
}