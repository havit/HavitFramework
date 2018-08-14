using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class CompositeDbInjectionAnnotationProvider : IDbInjectionAnnotationProvider
    {
        private readonly IEnumerable<IDbInjectionAnnotationProvider> providers;

        public CompositeDbInjectionAnnotationProvider(IEnumerable<IDbInjectionAnnotationProvider> providers)
        {
            this.providers = providers;
        }

        public List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo)
        {
            return providers.SelectMany(provider => provider.GetAnnotations(dbAnnotation, memberInfo)).ToList();
        }

        public List<IDbInjection> GetDbInjections(List<IAnnotation> annotations)
        {
            return providers.SelectMany(provider => provider.GetDbInjections(annotations)).ToList();
        }
    }
}