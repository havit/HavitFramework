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
	/// Annotations are used by EF Core Migrations and are defined on various elements of the <see cref="IModel" />. <see cref="IModel"/> is transformed to <see cref="IRelationalModel"/> when finalizing <see cref="IModel"/> creation (<see cref="RelationalModelConvention"/>).
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
		public override IEnumerable<IAnnotation> For(IRelationalModel model)
		{
			return base.For(model).Concat(providers.SelectMany(provider => provider.For(model)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ITable table)
		{
			return base.For(table).Concat(providers.SelectMany(provider => provider.For(table)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IColumn column)
		{
			return base.For(column).Concat(providers.SelectMany(provider => provider.For(column)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IView view)
		{
			return base.For(view).Concat(providers.SelectMany(provider => provider.For(view)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IViewColumn column)
		{
			return base.For(column).Concat(providers.SelectMany(provider => provider.For(column)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ISqlQuery sqlQuery)
		{
			return base.For(sqlQuery).Concat(providers.SelectMany(provider => provider.For(sqlQuery)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ISqlQueryColumn column)
		{
			return base.For(column).Concat(providers.SelectMany(provider => provider.For(column)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IStoreFunction function)
		{
			return base.For(function).Concat(providers.SelectMany(provider => provider.For(function)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IFunctionColumn column)
		{
			return base.For(column).Concat(providers.SelectMany(provider => provider.For(column)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IForeignKeyConstraint foreignKey)
		{
			return base.For(foreignKey).Concat(providers.SelectMany(provider => provider.For(foreignKey)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ITableIndex index)
		{
			return base.For(index).Concat(providers.SelectMany(provider => provider.For(index)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(IUniqueConstraint constraint)
		{
			return base.For(constraint).Concat(providers.SelectMany(provider => provider.For(constraint)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ISequence sequence)
		{
			return base.For(sequence).Concat(providers.SelectMany(provider => provider.For(sequence)));
		}

		/// <inheritdoc />
		public override IEnumerable<IAnnotation> For(ICheckConstraint checkConstraint)
		{
			return base.For(checkConstraint).Concat(providers.SelectMany(provider => provider.For(checkConstraint)));
		}
	}
}