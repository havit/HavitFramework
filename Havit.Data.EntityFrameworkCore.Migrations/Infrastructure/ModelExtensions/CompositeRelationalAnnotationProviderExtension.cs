using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;

/// <summary>
/// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="CompositeRelationalAnnotationProvider"/>.
/// </summary>
public class CompositeRelationalAnnotationProviderExtension : IDbContextOptionsExtension
{
	private ImmutableHashSet<Type> _providers = ImmutableHashSet<Type>.Empty;

	private DbContextOptionsExtensionInfo _info;

	internal IImmutableSet<Type> Providers => _providers;

	/// <inheritdoc />
	public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CompositeRelationalAnnotationProviderExtension()
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected CompositeRelationalAnnotationProviderExtension(CompositeRelationalAnnotationProviderExtension copyFrom)
	{
		_providers = copyFrom._providers;
	}

	/// <summary>
	/// Clones this instance.
	/// </summary>
	protected virtual CompositeRelationalAnnotationProviderExtension Clone() => new CompositeRelationalAnnotationProviderExtension(this);

	/// <summary>
	/// Registers specified type of <see cref="IRelationalAnnotationProvider"/>.
	/// </summary>
	/// <typeparam name="T">Implementation of <see cref="IRelationalAnnotationProvider"/></typeparam>
	/// <returns>Clone of <see cref="CompositeRelationalAnnotationProviderExtension"/>.</returns>
	public CompositeRelationalAnnotationProviderExtension WithAnnotationProvider<T>()
		where T : IRelationalAnnotationProvider
	{
		var clone = Clone();
		clone._providers = _providers.Add(typeof(T));
		return clone;
	}

	/// <inheritdoc />
	public void ApplyServices(IServiceCollection services)
	{
		var currentProviderTypes = _providers.ToArray();
		CompositeRelationalAnnotationProvider Factory(IServiceProvider serviceProvider)
		{
			var providers = currentProviderTypes.Select(type => (IRelationalAnnotationProvider)serviceProvider.GetService(type)).ToArray();
			return new CompositeRelationalAnnotationProvider(serviceProvider.GetRequiredService<RelationalAnnotationProviderDependencies>(), providers);
		}

		services.Add(currentProviderTypes.Select(t => ServiceDescriptor.Singleton(t, t)));
		services.Replace(ServiceDescriptor.Singleton<IRelationalAnnotationProvider, CompositeRelationalAnnotationProvider>(Factory));
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