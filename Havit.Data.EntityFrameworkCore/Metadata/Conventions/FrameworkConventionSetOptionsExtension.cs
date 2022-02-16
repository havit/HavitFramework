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
	public class FrameworkConventionSetOptionsExtension : IDbContextOptionsExtension
    {
		private ExtensionInfo info;
		private bool useCascadeDeleteToRestrictConvention = true;
		private bool useCacheAttributeToAnnotationConvention = true;
		private bool useDataTypeAttributeConvention = true;
		private bool useStringPropertiesDefaultValueConvention = false;
		private bool useManyToManyEntityKeyDiscoveryConvention = true;

		/// <summary>
		/// Indikuje používání konvence CascadeDeleteToRestrictConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseCascadeDeleteToRestrictConvention => useCascadeDeleteToRestrictConvention;

		/// <summary>
		/// Indikuje používání konvence CacheAttributeToAnnotationConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseCacheAttributeToAnnotationConvention => useCacheAttributeToAnnotationConvention;

		/// <summary>
		/// Indikuje používání konvence DataTypeAttributeConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseDataTypeAttributeConvention => useDataTypeAttributeConvention;

		/// <summary>
		/// Indikuje používání konvence StringPropertiesDefaultValueConvention.
		/// Výchozí hodnota je false.
		/// </summary>
		public bool UseStringPropertiesDefaultValueConvention => useStringPropertiesDefaultValueConvention;

		/// <summary>
		/// Indikuje používání konvence ManyToManyEntityKeyDiscoveryConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseManyToManyEntityKeyDiscoveryConvention => useManyToManyEntityKeyDiscoveryConvention;
	
		/// <inheritdoc />
		public DbContextOptionsExtensionInfo Info => info ??= new ExtensionInfo(this);

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

						builder.Append("UseCacheAttributeToAnnotationConvention=").Append(Extension.UseCacheAttributeToAnnotationConvention).Append(' ');
						builder.Append("UseCascadeDeleteToRestrictConvention=").Append(Extension.UseCascadeDeleteToRestrictConvention).Append(' ');
						builder.Append("UseDataTypeAttributeConvention=").Append(Extension.UseDataTypeAttributeConvention).Append(' ');
						builder.Append("UseManyToManyEntityKeyDiscoveryConvention=").Append(Extension.UseManyToManyEntityKeyDiscoveryConvention).Append(' ');
						builder.Append("UseStringPropertiesDefaultValueConvention=").Append(Extension.UseStringPropertiesDefaultValueConvention).Append(' ');

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
					hashCode.Add(Extension.UseCacheAttributeToAnnotationConvention);
					hashCode.Add(Extension.UseCascadeDeleteToRestrictConvention);
					hashCode.Add(Extension.UseDataTypeAttributeConvention);
					hashCode.Add(Extension.UseManyToManyEntityKeyDiscoveryConvention);
					hashCode.Add(Extension.UseStringPropertiesDefaultValueConvention);
					_serviceProviderHash = hashCode.ToHashCode();
				}

				return _serviceProviderHash.Value;
			}

			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
			{
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.UseCacheAttributeToAnnotationConvention)] = Extension.UseCacheAttributeToAnnotationConvention.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.UseCascadeDeleteToRestrictConvention)] = Extension.UseCascadeDeleteToRestrictConvention.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.UseDataTypeAttributeConvention)] = Extension.UseDataTypeAttributeConvention.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.UseManyToManyEntityKeyDiscoveryConvention)] = Extension.UseManyToManyEntityKeyDiscoveryConvention.GetHashCode().ToString(CultureInfo.InvariantCulture);
				debugInfo["HFW:" + nameof(FrameworkConventionSetOptionsExtension.UseStringPropertiesDefaultValueConvention)] = Extension.UseStringPropertiesDefaultValueConvention.GetHashCode().ToString(CultureInfo.InvariantCulture);
			}

			public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
			{
				return (other is ExtensionInfo otherExtensionInfo)
					&& (this.Extension.UseCacheAttributeToAnnotationConvention == otherExtensionInfo.Extension.UseCacheAttributeToAnnotationConvention)
					&& (this.Extension.UseCascadeDeleteToRestrictConvention == otherExtensionInfo.Extension.UseCascadeDeleteToRestrictConvention)
					&& (this.Extension.UseDataTypeAttributeConvention == otherExtensionInfo.Extension.UseDataTypeAttributeConvention)
					&& (this.Extension.UseManyToManyEntityKeyDiscoveryConvention == otherExtensionInfo.Extension.UseManyToManyEntityKeyDiscoveryConvention)
					&& (this.Extension.UseStringPropertiesDefaultValueConvention == otherExtensionInfo.Extension.UseStringPropertiesDefaultValueConvention);
			}
		}
    }
}