using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.TestHelpers.Fakes
{
    public class FakeModelExtensionsAssembly : IModelExtensionsAssembly
    {
        public IReadOnlyCollection<TypeInfo> ModelExtenders { get; }

        public Assembly Assembly => typeof(FakeModelExtensionsAssembly).Assembly;

        public FakeModelExtensionsAssembly(IEnumerable<TypeInfo> modelExtenders)
        {
            ModelExtenders = modelExtenders.ToImmutableArray();
        }

        public IModelExtender CreateModelExtender(TypeInfo modelExtenderClass)
        {
            return (IModelExtender)Activator.CreateInstance(modelExtenderClass);
        }
    }
}