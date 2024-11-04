﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;

/// <summary>
/// Composite implementation of <see cref="IMigrationsAnnotationProvider"/>. Encapsulates <see cref="IMigrationsAnnotationProvider"/> components and concatenates annotations for various elements of the <see cref="IModel"/>.
///
/// Annotations are used by EF Core Migrations and are defined on various elements of the <see cref="IModel" />.
/// </summary>
public class CompositeMigrationsAnnotationProvider : MigrationsAnnotationProvider
{
	private readonly IEnumerable<IMigrationsAnnotationProvider> _providers;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CompositeMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies, IEnumerable<IMigrationsAnnotationProvider> providers)
		: base(dependencies)
	{
		this._providers = providers;
	}


	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IRelationalModel model)
	{
		return base.ForRemove(model).Concat(_providers.SelectMany(provider => provider.ForRemove(model)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(ITable table)
	{
		return base.ForRemove(table).Concat(_providers.SelectMany(provider => provider.ForRemove(table)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IColumn column)
	{
		return base.ForRemove(column).Concat(_providers.SelectMany(provider => provider.ForRemove(column)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IView view)
	{
		return base.ForRemove(view).Concat(_providers.SelectMany(provider => provider.ForRemove(view)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IViewColumn column)
	{
		return base.ForRemove(column).Concat(_providers.SelectMany(provider => provider.ForRemove(column)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IUniqueConstraint constraint)
	{
		return base.ForRemove(constraint).Concat(_providers.SelectMany(provider => provider.ForRemove(constraint)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(ITableIndex index)
	{
		return base.ForRemove(index).Concat(_providers.SelectMany(provider => provider.ForRemove(index)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IForeignKeyConstraint foreignKey)
	{
		return base.ForRemove(foreignKey).Concat(_providers.SelectMany(provider => provider.ForRemove(foreignKey)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(ISequence sequence)
	{
		return base.ForRemove(sequence).Concat(_providers.SelectMany(provider => provider.ForRemove(sequence)));
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(ICheckConstraint checkConstraint)
	{
		return base.ForRemove(checkConstraint).Concat(_providers.SelectMany(provider => provider.ForRemove(checkConstraint)));
	}
}