﻿using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;

/// <summary>
/// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="CompositeMigrationsSqlGenerator"/>.
/// </summary>
public class CompositeMigrationsSqlGeneratorExtension : IDbContextOptionsExtension
{
	private ImmutableList<Type> _generatorTypes = ImmutableList<Type>.Empty;

	private DbContextOptionsExtensionInfo _info;

	internal IImmutableList<Type> GeneratorTypes => _generatorTypes;

	/// <inheritdoc />
	public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CompositeMigrationsSqlGeneratorExtension()
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected CompositeMigrationsSqlGeneratorExtension(CompositeMigrationsSqlGeneratorExtension copyFrom)
	{
		_generatorTypes = copyFrom._generatorTypes;
	}

	/// <summary>
	/// Clones this instance.
	/// </summary>
	protected virtual CompositeMigrationsSqlGeneratorExtension Clone() => new CompositeMigrationsSqlGeneratorExtension(this);

	/// <summary>
	/// Registers specified type of <see cref="IMigrationOperationSqlGenerator"/>.
	/// </summary>
	/// <typeparam name="T">Implementation of <see cref="IMigrationOperationSqlGenerator"/></typeparam>
	/// <returns>Clone of <see cref="CompositeMigrationsSqlGeneratorExtension"/>.</returns>
	public CompositeMigrationsSqlGeneratorExtension WithGeneratorType<T>()
		where T : IMigrationOperationSqlGenerator
	{
		var clone = Clone();
		if (!clone._generatorTypes.Contains(typeof(T)))
		{
			clone._generatorTypes = _generatorTypes.Add(typeof(T));
		}

		return clone;
	}

	/// <inheritdoc />
	public void ApplyServices(IServiceCollection services)
	{
		var currentProviderTypes = _generatorTypes.ToArray();

		// since EF.Core 3.0, IMigrationsSqlGenerator is registered with scoped lifetime
		// here we'll use exact lifetime of IMigrationsSqlGenerator defined in EF Core
		// (because implementations of IMigrationOperationSqlGenerator will always be used inside CompositeMigrationsSqlGenerator)
		services.Add(currentProviderTypes.Select(generatorType => ServiceDescriptor.Describe(
			typeof(IMigrationOperationSqlGenerator),
			generatorType,
			EntityFrameworkRelationalServicesBuilder.RelationalServices[typeof(IMigrationsSqlGenerator)].Lifetime)));
		services.Replace(ServiceDescriptor.Scoped<IMigrationsSqlGenerator, CompositeMigrationsSqlGenerator>());
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

		public override int GetServiceProviderHashCode() => 0x581B;

		public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ExtensionInfo;

		public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
		{
			// NOOP
		}
	}
}