using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor.MsDependencyInjection;
using System;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.AspNetCore
{
    /// <summary>
    /// Extension metody k ComponentRegistration&lt;TService&gt;.
    /// </summary>
    public static class ComponentRegistrationExtensions
    {
        /// <summary>
        /// Lifestype per webový request pro ASP.NET Core.
        /// </summary>
        public static ComponentRegistration<TService> LifestylePerAspNetCoreRequest<TService>(this ComponentRegistration<TService> componentRegistration)
            where TService : class
        {
            return componentRegistration.LifeStyle.PerAspNetCoreRequest();
        }
    }

}
