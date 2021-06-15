using System;
using System.Linq;
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

        /// <summary>
        /// Typ, pod kterým bude služba zaregistrována.
        /// Nelze kombinovat s <see cref="ServiceTypes"/>.
        /// </summary>
        public Type ServiceType
        {
            get
            {
                return _serviceType;
            }
            set
            {
                _serviceType = value;
                CheckServiceTypeAndServiceTypes();
            }
        }

        private Type _serviceType;

        /// <summary>
        /// Typy, pod kterým bude služba zaregistrována.
        /// Nelze kombinovat s <see cref="ServiceType"/>.
        /// </summary>
        public Type[] ServiceTypes
        {
            get { return _serviceTypes; }
            set
            {
                if ((value != null) && (value.Length == 0))
                {
                    throw new ArgumentException($"Cannot set an empty array to {ServiceTypes} property.");
                }

                _serviceTypes = value;
                CheckServiceTypeAndServiceTypes();
            }
        }
        private Type[] _serviceTypes;

        private void CheckServiceTypeAndServiceTypes()
		{
            if ((_serviceType != null) && (_serviceTypes != null))
			{
                throw new InvalidOperationException($"Properties { nameof(ServiceType) } and { nameof(ServiceTypes) } are mutual exclusive. Use { nameof(ServiceType) } or { nameof(ServiceTypes) }, not both.");
			}
		}

        /// <summary>
        /// Vrátí služby, pod které má být služba zaregistrována.
        /// </summary>
        public Type[] GetServiceTypes()
		{
            if (_serviceType != null)
			{
                return new Type[] { _serviceType };
			}

            return _serviceTypes; // can be null
		}
        
        /// <inheritdoc />
		public override string ToString()
		{
            return $"{ nameof(Profile) } = \"{Profile}\", ServiceTypes = {{ {String.Join(", ", GetServiceTypes().Select(type => type.FullName)) } }}, Lifetime = { Lifetime.ToString() }";
		}
	}
}
