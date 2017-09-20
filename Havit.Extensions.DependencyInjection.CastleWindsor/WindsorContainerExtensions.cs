using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.CastleWindsor
{
    public static class WindsorContainerExtensions
    {
        /// <summary>
        /// Nainstaluje do WindsorContaineru třídy, které jsou v dané assembly a jsou ozvačeny attributem ServiceAttribute.
        /// Třídy jsou instalovány pod služby pomocí WithServiceDefaultInterfaces().
        /// Lifestyle se řídí hodnotou Lifetime u attributu. Lifestyle pro hodnotu Scoped je možné zadat v parametru.
        /// </summary>
        /// <param name="container">WindsorContainer, do kterého jsou registrovány služby.</param>
        /// <param name="assembly">Assembly, ve které jsou hledány služby k registraci.</param>
        /// <param name="scopedLifetimeConfigurer">(Volitelné.) LifeStyle, který je použit pro ServiceLifetime.Scoped. Pokud není uveden, použije se LifestyleScoped().
        /// Pro scope per ASP.NET Core web request má hodnotu lf => lf.PerAspNetCoreWebRequest().
        /// </param>
        public static void InstallByServiceAttibute(this IWindsorContainer container, Assembly assembly, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

                var itemsToRegister = GetTypesWithServiceAttribute(assembly);

                foreach (var itemToRegister in itemsToRegister)
                {
                    BasedOnDescriptor descriptor = Classes.From(itemToRegister.Type).Pick().WithServiceDefaultInterfaces().ApplyLifestyle(itemToRegister.Attribute.Lifetime, scopedLifetimeConfigurer);
                    container.Register(descriptor);
                }
            }

            private static TypeAttributeInfo[] GetTypesWithServiceAttribute(Assembly assembly)
            {
                return assembly
                    .GetTypes()
                    .Where(type => type.IsDefined(typeof(ServiceAttribute)))
                    .Select(type => new TypeAttributeInfo { Type = type, Attribute = ((ServiceAttribute)type.GetCustomAttributes(typeof(ServiceAttribute), false).Single()) })
                    .ToArray();
            }

            private static BasedOnDescriptor ApplyLifestyle(this BasedOnDescriptor descriptor, ServiceLifetime lifetime, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Transient:
                        return descriptor.LifestyleTransient();

                    case ServiceLifetime.Singleton:
                        return descriptor.LifestyleSingleton();

                    case ServiceLifetime.Scoped:
                        return (scopedLifetimeConfigurer != null)
                            ? descriptor.Configure(configurer => scopedLifetimeConfigurer(configurer.LifeStyle))
                            : descriptor.LifestyleScoped();

                    default:
                        throw new NotSupportedException(lifetime.ToString());
                }
            }

        [DebuggerDisplay("Type={Type}, Attribute = {Attribute}")]
        private class TypeAttributeInfo
        {
            public Type Type { get; set; }

            public ServiceAttribute Attribute { get; set; }
        }

    }
}
