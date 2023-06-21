using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions.Infrastructure;

/// <summary>
/// Installer IConventionSetPlugin do service collection.
/// </summary>
internal class ConventionSetPluginServiceInstallerExtension<TConventionSetPlugin> : ConventionSetPluginServiceInstallerExtension, IDbContextOptionsExtension
	where TConventionSetPlugin : class, IConventionSetPlugin
{
	private DbContextOptionsExtensionInfo _info;

	/// <inheritdoc />
	public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

	/// <inheritdoc />
	public override Type ConventionType => typeof(TConventionSetPlugin);

	/// <inheritdoc />
	public void ApplyServices(IServiceCollection services)
	{
		new EntityFrameworkServicesBuilder(services).TryAdd<IConventionSetPlugin, TConventionSetPlugin>();
	}

	/// <inheritdoc />
	public void Validate(IDbContextOptions options)
	{
		// NOOP
	}
}
