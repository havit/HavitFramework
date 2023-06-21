using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;
using Havit.Diagnostics.Contracts;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Extensions.DependencyInjection.Scanners;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.CastleWindsor;

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
        /// <param name="componentRegistrationConfigurer">(Volitelné.) Custom configurace pro registrované služby.</param>
        public static void InstallByServiceAttribute(this IWindsorContainer container, Assembly assembly, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer = null, Action<ComponentRegistration> componentRegistrationConfigurer = null)
        {
            InstallByServiceAttribute(container, assembly, ServiceAttribute.DefaultProfile, scopedLifetimeConfigurer, componentRegistrationConfigurer);
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
        /// <param name="componentRegistrationConfigurer">(Volitelné.) Custom configurace pro registrované služby.</param>
        public static void InstallByServiceAttribute(this IWindsorContainer container, Assembly assembly, string[] profiles, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer = null, Action<ComponentRegistration> componentRegistrationConfigurer = null)
        {
		Contract.Requires<ArgumentNullException>(profiles != null, nameof(profiles));

            foreach (string profile in profiles)
            {
                InstallByServiceAttribute(container, assembly, profile, scopedLifetimeConfigurer, componentRegistrationConfigurer);
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
        /// <param name="componentRegistrationConfigurer">(Volitelné.) Custom configurace pro registrované služby.</param>
        public static void InstallByServiceAttribute(this IWindsorContainer container, Assembly assembly, string profile, Func<LifestyleGroup<object>, ComponentRegistration<object>> scopedLifetimeConfigurer = null, Action<ComponentRegistration> componentRegistrationConfigurer = null)
        {
		Contract.Requires<ArgumentNullException>(container != null, nameof(container));
		Contract.Requires<ArgumentNullException>(assembly != null, nameof(assembly));

		TypeServiceAttributeInfo[] servicesToRegister = AssemblyScanner.GetTypesWithServiceAttribute(assembly, profile);

            foreach (var serviceToRegister in servicesToRegister)
            {
                Type[] serviceTypes = serviceToRegister.ServiceAttribute.GetServiceTypes();
                BasedOnDescriptor descriptor = Classes.From(serviceToRegister.Type).Pick();
                if (serviceTypes == null)
                {
                    descriptor = descriptor.WithServiceDefaultInterfaces();
                }
                else
			{
                    descriptor = descriptor.WithServices(serviceTypes);
                }
                descriptor = descriptor.ApplyLifestyle(serviceToRegister.ServiceAttribute.Lifetime, scopedLifetimeConfigurer);
                if (componentRegistrationConfigurer != null)
			{
                    descriptor = descriptor.Configure(componentRegistrationConfigurer);
                }
                container.Register(descriptor);
            }
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
    }
