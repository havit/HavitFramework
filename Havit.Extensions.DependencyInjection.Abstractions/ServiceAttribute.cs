using System;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.Abstractions
{
    /// <summary>
    /// Slouží k označení tříd, které mají být automaticky zaregistrovány do IoC containeru.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Výchozí název profilu (pokud není specifikován).
        /// </summary>
        public const string DefaultProfile = "@DefaultProfile";

        /// <summary>
        /// Lifetime (lifestyle) s jakým má být služba zaregistrována.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// Název profilu. Při registraci jsou registrovány jen služby požadovaných profilů.
        /// </summary>
        public string Profile { get; set; } = DefaultProfile;
    }
}
