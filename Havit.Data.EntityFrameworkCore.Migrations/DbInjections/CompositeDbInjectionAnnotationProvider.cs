using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    /// Composite implementation of <see cref="IDbInjectionAnnotationProvider"/>.
    /// </summary>
    public class CompositeDbInjectionAnnotationProvider : IDbInjectionAnnotationProvider
    {
        private readonly IEnumerable<IDbInjectionAnnotationProvider> providers;

        /// <inheritdoc />
        public CompositeDbInjectionAnnotationProvider(IEnumerable<IDbInjectionAnnotationProvider> providers)
        {
            this.providers = providers;
        }

        /// <inheritdoc />
        public List<IAnnotation> GetAnnotations(IDbInjection dbAnnotation, MemberInfo memberInfo)
        {
            return providers.SelectMany(provider => provider.GetAnnotations(dbAnnotation, memberInfo)).ToList();
        }

        /// <inheritdoc />
        public List<IDbInjection> GetDbInjections(List<IAnnotation> annotations)
        {
            return providers.SelectMany(provider => provider.GetDbInjections(annotations)).ToList();
        }
    }
}