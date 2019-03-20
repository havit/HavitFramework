using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor.MsDependencyInjection;
using System;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.AspNetCore
{
    /// <summary>
    /// Extension metody k LifestyleGroup&lt;TService&gt;.
    /// </summary>
    public static class LifestyleGroupExtensions
    {
        /// <summary>
        /// Lifestype per webový request pro ASP.NET Core.
        /// </summary>
        public static ComponentRegistration<TService> PerAspNetCoreRequest<TService>(this LifestyleGroup<TService> lifestyleGroup)
            where TService : class
        {
            return lifestyleGroup.Custom<MsScopedLifestyleManager>();
        }
    }
}
