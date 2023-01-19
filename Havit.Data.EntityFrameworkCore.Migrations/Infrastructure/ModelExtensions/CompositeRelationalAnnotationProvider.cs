using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions
{
	/// <summary>
	/// Composite implementation of <see cref="IRelationalAnnotationProvider"/>. Encapsulates <see cref="IRelationalAnnotationProvider"/> components and concatenates annotations for various elements of the <see cref="IRelationalModel"/>.
	///
	/// Annotations are used by EF Core Migrations and are defined on various elements of the <see cref="IModel" />. <see cref="IModel"/> is transformed to <see cref="IRelationalModel"/> when finalizing <see cref="IModel"/> creation.
	/// </summary>
	public class CompositeRelationalAnnotationProvider : RelationalAnnotationProvider
	{
		private readonly IEnumerable<IRelationalAnnotationProvider> providers;

		/// <summary>
		/// Constructor
		/// </summary>
		public CompositeRelationalAnnotationProvider(
			RelationalAnnotationProviderDependencies dependencies,
			IEnumerable<IRelationalAnnotationProvider> providers)
			: base(dependencies)
		{
			this.providers = providers;
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IRelationalModel model, bool designTime)
		{
			return base.For(model, designTime).Concat(providers.SelectMany(provider => provider.For(model, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ITable table, bool designTime)
		{
			return base.For(table, designTime).Concat(providers.SelectMany(provider => provider.For(table, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
		{
			return base.For(column, designTime).Concat(providers.SelectMany(provider => provider.For(column, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IView view, bool designTime)
		{
			return base.For(view, designTime).Concat(providers.SelectMany(provider => provider.For(view, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IViewColumn column, bool designTime)
		{
			return base.For(column, designTime).Concat(providers.SelectMany(provider => provider.For(column, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ISqlQuery sqlQuery, bool designTime)
		{
			return base.For(sqlQuery, designTime).Concat(providers.SelectMany(provider => provider.For(sqlQuery, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ISqlQueryColumn column, bool designTime)
		{
			return base.For(column, designTime).Concat(providers.SelectMany(provider => provider.For(column, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IStoreFunction function, bool designTime)
		{
			return base.For(function, designTime).Concat(providers.SelectMany(provider => provider.For(function, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IFunctionColumn column, bool designTime)
		{
			return base.For(column, designTime).Concat(providers.SelectMany(provider => provider.For(column, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IForeignKeyConstraint foreignKey, bool designTime)
		{
			return base.For(foreignKey, designTime).Concat(providers.SelectMany(provider => provider.For(foreignKey, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ITableIndex index, bool designTime)
		{
			return base.For(index, designTime).Concat(providers.SelectMany(provider => provider.For(index, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IUniqueConstraint constraint, bool designTime)
		{
			return base.For(constraint, designTime).Concat(providers.SelectMany(provider => provider.For(constraint, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ISequence sequence, bool designTime)
		{
			return base.For(sequence, designTime).Concat(providers.SelectMany(provider => provider.For(sequence, designTime)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ICheckConstraint checkConstraint, bool designTime)
		{
			return base.For(checkConstraint, designTime).Concat(providers.SelectMany(provider => provider.For(checkConstraint, designTime)));
		}
	}
}