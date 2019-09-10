using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// Composite implementation of <see cref="IMigrationsAnnotationProvider"/>. Encapsulates <see cref="IMigrationsAnnotationProvider"/> components and concatenates annotations for various elements of the <see cref="IModel"/>.
    ///
    /// Annotations are used by EF Core Migrations and are defined on various elements of the <see cref="IModel" />.
    /// </summary>
    public class CompositeMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
	    private readonly IEnumerable<IMigrationsAnnotationProvider> providers;

        /// <summary>
		/// Konstruktor.
		/// </summary>
        public CompositeMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies, IEnumerable<IMigrationsAnnotationProvider> providers)
			: base(dependencies)
	    {
	        this.providers = providers;
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(IModel model)
	    {
		    return base.For(model).Concat(providers.SelectMany(provider => provider.For(model)));
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(IKey key)
	    {
	        return base.For(key).Concat(providers.SelectMany(provider => provider.For(key)));
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(IIndex index)
	    {
	        return base.For(index).Concat(providers.SelectMany(provider => provider.For(index)));
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(IForeignKey foreignKey)
	    {
	        return base.For(foreignKey).Concat(providers.SelectMany(provider => provider.For(foreignKey)));
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(IProperty property)
	    {
	        return base.For(property).Concat(providers.SelectMany(provider => provider.For(property)));
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(ISequence sequence)
	    {
	        return base.For(sequence).Concat(providers.SelectMany(provider => provider.For(sequence)));
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> For(IEntityType entityType)
	    {
	        return base.For(entityType).Concat(providers.SelectMany(provider => provider.For(entityType)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IModel model)
	    {
	        return base.ForRemove(model).Concat(providers.SelectMany(provider => provider.ForRemove(model)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IKey key)
	    {
	        return base.ForRemove(key).Concat(providers.SelectMany(provider => provider.ForRemove(key)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IIndex index)
	    {
	        return base.ForRemove(index).Concat(providers.SelectMany(provider => provider.ForRemove(index)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IForeignKey foreignKey)
	    {
	        return base.ForRemove(foreignKey).Concat(providers.SelectMany(provider => provider.ForRemove(foreignKey)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IProperty property)
	    {
	        return base.ForRemove(property).Concat(providers.SelectMany(provider => provider.ForRemove(property)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(ISequence sequence)
	    {
	        return base.ForRemove(sequence).Concat(providers.SelectMany(provider => provider.ForRemove(sequence)));
	    }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IEntityType entityType)
	    {
	        return base.ForRemove(entityType).Concat(providers.SelectMany(provider => provider.ForRemove(entityType)));
	    }
    }
}