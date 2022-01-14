using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure
{
	/// <summary>
	/// Pomocná bázová třída pro ConventionSetPluginServiceInstallerExtension 
	/// (publikujeme pouze ConventionType, který použije nested class ExtensionInfo).
	/// </summary>
	internal abstract class ConventionSetPluginServiceInstallerExtension
	{
		/// <summary>
		/// Typ instalované konvence.
		/// </summary>
		public abstract Type ConventionType { get; }

		protected class ExtensionInfo : DbContextOptionsExtensionInfo
		{
			private string _logFragment;

			public ExtensionInfo(IDbContextOptionsExtension extension)
				: base(extension)
			{
			}

			private new ConventionSetPluginServiceInstallerExtension Extension
			{
				get
				{
					return (ConventionSetPluginServiceInstallerExtension)base.Extension;
				}
			}

			public override bool IsDatabaseProvider => false;

			public override string LogFragment => _logFragment ??= "using convention " + Extension.ConventionType.FullName;

			public override int GetServiceProviderHashCode() => Extension.ConventionType.GetHashCode();

			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
			{
				debugInfo["ConventionType:" + nameof(Extension.ConventionType)] = (Extension.ConventionType.GetHashCode()).ToString(CultureInfo.InvariantCulture);
			}

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
				return (other is ExtensionInfo otherExtensionInfo) && (this.Extension.ConventionType == otherExtensionInfo.Extension.ConventionType);
            }
        }
	}
}
