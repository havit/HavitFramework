using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.CastleWindsor
{
    /// <summary>
    /// Extension metody k IWindsorContainer.
    /// </summary>
    public static class WindsorContainerExtensions
    {
        /// <summary>
        /// Nainstaluje do WindsorContaineru třídy, které jsou v dané assembly, jsou ozvačeny attributem ServiceAttribute a nemají uveden žádný název profilu (resp. zůstal ServiceAttribute.DefaultProfile).
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
            InstallByServiceAttibute(container, assembly, ServiceAttribute.DefaultProfile, scopedLifetimeConfigurer);
        }

        /// <summary>
        /// Nainstaluje do WindsorContaineru třídy, které jsou v dané assembly, jsou ozvačeny attributem ServiceAttribute a uveden jeden z požadovaných názvů profilu.
        /// Třídy jsou instalovány pod služby pomocí WithServiceDefaultInterfaces().
        /// Lifestyle se řídí hodnotou Lifetime u attributu. Lifestyle pro hodnotu Scoped je možné zadat v parametru.
        /// </summary>
        /// <param name="container">WindsorContainer, do kterého jsou registrovány služby.</param>
        /// <param name="assembly">Assembly, ve které jsou hledány služby k registraci.</param>
        /// <param name="profiles">Názvy profilu, který musí mít služby nastaveny (resp. alespoň jeden z nich), aby byly nainstalovány. Lze použít i ServiceAttribute.DefaultProfile.</param>
        /// <param name="scopedLifetimeConfigurer">(Volitelné.) LifeStyle, který je použit pro ServiceLifetime.Scoped. Pokud není uveden, použije se LifestyleScoped().
        /// Pro scope per ASP.NET Core web request má hodnotu lf => lf.PerAspNetCoreWebRequest().
        /// </param>
        public static void InstallByServiceAttibute(this IWindsorContainer container, Assembly assembly, string[] profiles, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer = null)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            foreach (string profile in profiles)
            {
                InstallByServiceAttibute(container, assembly, profile, scopedLifetimeConfigurer);
            }
        }

        /// <summary>
        /// Nainstaluje do WindsorContaineru třídy, které jsou v dané assembly a jsou ozvačeny attributem ServiceAttribute.
        /// Třídy jsou instalovány pod služby pomocí WithServiceDefaultInterfaces().
        /// Lifestyle se řídí hodnotou Lifetime u attributu. Lifestyle pro hodnotu Scoped je možné zadat v parametru.
        /// </summary>
        /// <param name="container">WindsorContainer, do kterého jsou registrovány služby.</param>
        /// <param name="assembly">Assembly, ve které jsou hledány služby k registraci.</param>
        /// <param name="profile">Název profilu, který musí mít služby nastaveny, aby byly nainstalovány.</param>
        /// <param name="scopedLifetimeConfigurer">(Volitelné.) LifeStyle, který je použit pro ServiceLifetime.Scoped. Pokud není uveden, použije se LifestyleScoped().
        /// Pro scope per ASP.NET Core web request má hodnotu lf => lf.PerAspNetCoreWebRequest().
        /// </param>
        public static void InstallByServiceAttibute(this IWindsorContainer container, Assembly assembly, string profile, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var itemsToRegister = GetTypesWithServiceAttribute(assembly, profile);

            foreach (var itemToRegister in itemsToRegister)
            {
                BasedOnDescriptor descriptor = Classes.From(itemToRegister.Type).Pick().WithServiceDefaultInterfaces().ApplyLifestyle(itemToRegister.Lifetime, scopedLifetimeConfigurer);
                container.Register(descriptor);
            }
        }

        private static TypeAttributeInfo[] GetTypesWithServiceAttribute(Assembly assembly, string profile)
        {
            return (from type in assembly.GetTypes()
                    from serviceAttribute in type.GetCustomAttributes(typeof(ServiceAttribute), false).Cast<ServiceAttribute>()
                    where (serviceAttribute.Profile == profile)
                    select new TypeAttributeInfo { Type = type, Lifetime = serviceAttribute.Lifetime })                    
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

            public ServiceLifetime Lifetime { get; set; }
        }

    }
}
