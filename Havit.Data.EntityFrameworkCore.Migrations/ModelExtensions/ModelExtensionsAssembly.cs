using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
    /// <inheritdoc />
    public class ModelExtensionsAssembly : IModelExtensionsAssembly
    {
        private IReadOnlyCollection<TypeInfo> modelExtenders;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModelExtensionsAssembly(IDbContextOptions dbContextOptions)
        {
            Contract.Requires<ArgumentNullException>(dbContextOptions != null);

            Assembly = dbContextOptions.FindExtension<ModelExtensionsExtension>()?.ExtensionsAssembly;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<TypeInfo> ModelExtenders
        {
            get
            {
                IReadOnlyCollection<TypeInfo> Create()
                {
                    return Assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && t.GetInterface(nameof(IModelExtender)) != null).ToImmutableArray();
                }

                return modelExtenders ??= Create();
            }
        }

        /// <inheritdoc />
        public Assembly Assembly { get; }

        /// <inheritdoc />
        public IModelExtender CreateModelExtender(TypeInfo modelExtenderClass)
        {
            Contract.Requires<ArgumentNullException>(modelExtenderClass != null);

            return (IModelExtender)Activator.CreateInstance(modelExtenderClass.AsType());
        }
    }
}