using System.Globalization;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions.Infrastructure;

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
