using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection
{
    /// <summary>
    /// Slouží k označení tříd, které mají být automaticky zaregistrovány do IoC containeru.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Lifetime (lifestyle) s jakým má být služba zaregistrována.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }
}
