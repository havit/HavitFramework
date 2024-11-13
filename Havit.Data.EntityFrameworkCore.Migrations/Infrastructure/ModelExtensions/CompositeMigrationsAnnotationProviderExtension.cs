﻿using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;

/// <summary>
/// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="CompositeMigrationsAnnotationProvider"/>.
/// </summary>
public class CompositeMigrationsAnnotationProviderExtension : IDbContextOptionsExtension
{
	private ImmutableHashSet<Type> _providers = ImmutableHashSet<Type>.Empty;

	private DbContextOptionsExtensionInfo _info;

	internal IImmutableSet<Type> Providers => _providers;

	/// <inheritdoc />
	public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CompositeMigrationsAnnotationProviderExtension()
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected CompositeMigrationsAnnotationProviderExtension(CompositeMigrationsAnnotationProviderExtension copyFrom)
	{
		_providers = copyFrom._providers;
	}

	/// <summary>
	/// Clones this instance.
	/// </summary>
	protected virtual CompositeMigrationsAnnotationProviderExtension Clone() => new CompositeMigrationsAnnotationProviderExtension(this);

	/// <summary>
	/// Registers specified type of <see cref="IMigrationsAnnotationProvider"/>.
	/// </summary>
	/// <typeparam name="T">Implementation of <see cref="IMigrationsAnnotationProvider"/></typeparam>
	/// <returns>Clone of <see cref="CompositeMigrationsAnnotationProviderExtension"/>.</returns>
	public CompositeMigrationsAnnotationProviderExtension WithAnnotationProvider<T>()
		where T : IMigrationsAnnotationProvider
	{
		var clone = Clone();
		clone._providers = _providers.Add(typeof(T));
		return clone;
	}

	/// <inheritdoc />
	public void ApplyServices(IServiceCollection services)
	{
		var currentProviderTypes = _providers.ToArray();
		CompositeMigrationsAnnotationProvider Factory(IServiceProvider serviceProvider)
		{
			var providers = currentProviderTypes.Select(type => (IMigrationsAnnotationProvider)serviceProvider.GetService(type)).ToArray();
			return new CompositeMigrationsAnnotationProvider(serviceProvider.GetRequiredService<MigrationsAnnotationProviderDependencies>(), providers);
		}

		services.Add(currentProviderTypes.Select(t => ServiceDescriptor.Singleton(t, t)));
		services.Replace(ServiceDescriptor.Singleton<IMigrationsAnnotationProvider, CompositeMigrationsAnnotationProvider>(Factory));
	}

	/// <inheritdoc />
	public void Validate(IDbContextOptions options)
	{
		// no validation
	}

	private class ExtensionInfo : DbContextOptionsExtensionInfo
	{
		public override bool IsDatabaseProvider => false;

		public override string LogFragment => "";

		public ExtensionInfo(IDbContextOptionsExtension dbContextOptionsExtension) : base(dbContextOptionsExtension)
		{
		}

		public override int GetServiceProviderHashCode() => 0xA5B6;

		public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ExtensionInfo;

		public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
		{
			// NOOP
		}
	}

}