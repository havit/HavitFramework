using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Konfigurace použití konvencí ve frameworku.
	/// </summary>
	/// <remarks>
	/// Třída je sealed pro jednodušší implementaci metody Clone.
	/// </remarks>
	public sealed class FrameworkConventionSetOptionsExtension : IDbContextOptionsExtension
    {
		private ExtensionInfo info;

		/// <summary>
		/// Indikuje používání konvence CascadeDeleteToRestrictConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool CascadeDeleteToRestrictConventionEnabled { get; private set; } = true;

		/// <summary>
		/// Indikuje používání konvence CacheAttributeToAnnotationConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool CacheAttributeToAnnotationConventionEnabled { get; private set; } = true;

		/// <summary>
		/// Indikuje používání konvence DataTypeAttributeConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool DataTypeAttributeConventionEnabled { get; private set; } = true;

		/// <summary>
		/// Indikuje používání konvence StringPropertiesDefaultValueConvention.
		/// Výchozí hodnota je false.
		/// </summary>
		public bool StringPropertiesDefaultValueConventionEnabled { get; private set; } = false;

		/// <summary>
		/// Indikuje používání konvence ManyToManyEntityKeyDiscoveryConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool ManyToManyEntityKeyDiscoveryConventionEnabled { get; private set; } = true;

		/// <inheritdoc />
		public DbContextOptionsExtensionInfo Info => info ??= new ExtensionInfo(this);

		/// <summary>
		/// Konstruktor.
		/// </summary>
        public FrameworkConventionSetOptionsExtension()
        {
        }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public FrameworkConventionSetOptionsExtension(FrameworkConventionSetOptionsExtension original)
        {
			this.CacheAttributeToAnnotationConventionEnabled = original.CacheAttributeToAnnotationConventionEnabled;
			this.CascadeDeleteToRestrictConventionEnabled = original.CascadeDeleteToRestrictConventionEnabled;
			this.DataTypeAttributeConventionEnabled = original.DataTypeAttributeConventionEnabled;
			this.StringPropertiesDefaultValueConventionEnabled = original.StringPropertiesDefaultValueConventionEnabled;
			this.ManyToManyEntityKeyDiscoveryConventionEnabled = original.ManyToManyEntityKeyDiscoveryConventionEnabled;
		}

		/// <summary>
		/// Získá kopii objektu se shodným nastavením.
		/// </summary>
		/// <returns></returns>
		public FrameworkConventionSetOptionsExtension Clone()
        {
			return new FrameworkConventionSetOptionsExtension(this);
		}

		public FrameworkConventionSetOptionsExtension WithCacheAttributeToAnnotationConvention(bool enabled)
		{
			var clone = Clone();
			clone.CacheAttributeToAnnotationConventionEnabled = enabled;
			return clone;
		}

		public FrameworkConventionSetOptionsExtension WithCascadeDeleteToRestrictConvention(bool enabled)
		{
			var clone = Clone();
			clone.CascadeDeleteToRestrictConventionEnabled = enabled;
			return clone;
		}

		public FrameworkConventionSetOptionsExtension WithDataTypeAttributeConvention(bool enabled)
		{
			var clone = Clone();
			clone.DataTypeAttributeConventionEnabled = enabled;
			return clone;
		}

		public FrameworkConventionSetOptionsExtension WithStringPropertiesDefaultValueConvention(bool enabled)
		{
			var clone = Clone();
			clone.StringPropertiesDefaultValueConventionEnabled = enabled;
			return clone;
		}

		public FrameworkConventionSetOptionsExtension WithManyToManyEntityKeyDiscoveryConvention(bool enabled)
		{
			var clone = Clone();
			clone.ManyToManyEntityKeyDiscoveryConventionEnabled = enabled;
			return clone;
		}

		/// <inheritdoc />
		public void ApplyServices(IServiceCollection services)
        {
			services.AddTransient<IConventionSetPlugin, FrameworkConventionSetPlugin>();
        }

		/// <inheritdoc />
		public void Validate(IDbContextOptions options)
        {
        }

		private class ExtensionInfo : DbContextOptionsExtensionInfo
		{
			private string _logFragment;
			private int? _serviceProviderHash;

			public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
			{
			}

			public new virtual FrameworkConventionSetOptionsExtension Extension => (FrameworkConventionSetOptionsExtension)base.Extension;

			public override bool IsDatabaseProvider => false;

			public override string LogFragment
			{
				get
				{
					if (_logFragment == null)
					{
						var builder = new StringBuilder();

						builder.Append(nameof(CacheAttributeToAnnotationConventionEnabled)).Append("=").Append(Extension.CacheAttributeToAnnotationConventionEnabled).Append(' ');
						builder.Append(nameof(CascadeDeleteToRestrictConventionEnabled)).Append("=").Append(Extension.CascadeDeleteToRestrictConventionEnabled).Append(' ');
						builder.Append(nameof(DataTypeAttributeConventionEnabled)).Append("=").Append(Extension.DataTypeAttributeConventionEnabled).Append(' ');
						builder.Append(nameof(ManyToManyEntityKeyDiscoveryConventionEnabled)).Append("=").Append(Extension.ManyToManyEntityKeyDiscoveryConventionEnabled).Append(' ');
						builder.Append(nameof(StringPropertiesDefaultValueConventionEnabled)).Append("=").Append(Extension.StringPropertiesDefaultValueConventionEnabled).Append(' ');

						_logFragment = builder.ToString();
					}

					return _logFragment;
				}
			}

			public override int GetServiceProviderHashCode()
			{
				if (_serviceProviderHash == null)
				{
					var hashCode = new HashCode();
					hashCode.Add(Extension.CacheAttributeToAnnotationConventionEnabled);
					hashCode.Add(Extension.CascadeDeleteToRestrictConventionEnabled);
					hashCode.Add(Extension.DataTypeAttributeConventionEnabled);
					hashCode.Add(Extension.ManyToManyEntityKeyDiscoveryConventionEnabled);
					hashCode.Add(Extension.StringPropertiesDefaultValueConventionEnabled);
					_serviceProviderHash = hashCode.ToHashCode();
				}

				return _serviceProviderHash.Value;
			}

			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
			{
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.CacheAttributeToAnnotationConventionEnabled)] = Extension.CacheAttributeToAnnotationConventionEnabled.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.CascadeDeleteToRestrictConventionEnabled)] = Extension.CascadeDeleteToRestrictConventionEnabled.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.DataTypeAttributeConventionEnabled)] = Extension.DataTypeAttributeConventionEnabled.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.ManyToManyEntityKeyDiscoveryConventionEnabled)] = Extension.ManyToManyEntityKeyDiscoveryConventionEnabled.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.StringPropertiesDefaultValueConventionEnabled)] = Extension.StringPropertiesDefaultValueConventionEnabled.GetHashCode().ToString(CultureInfo.InvariantCulture);
			}

			public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
			{
				return (other is ExtensionInfo otherExtensionInfo)
					&& (this.Extension.CacheAttributeToAnnotationConventionEnabled == otherExtensionInfo.Extension.CacheAttributeToAnnotationConventionEnabled)
					&& (this.Extension.CascadeDeleteToRestrictConventionEnabled == otherExtensionInfo.Extension.CascadeDeleteToRestrictConventionEnabled)
					&& (this.Extension.DataTypeAttributeConventionEnabled == otherExtensionInfo.Extension.DataTypeAttributeConventionEnabled)
					&& (this.Extension.ManyToManyEntityKeyDiscoveryConventionEnabled == otherExtensionInfo.Extension.ManyToManyEntityKeyDiscoveryConventionEnabled)
					&& (this.Extension.StringPropertiesDefaultValueConventionEnabled == otherExtensionInfo.Extension.StringPropertiesDefaultValueConventionEnabled);
			}
		}
    }
}