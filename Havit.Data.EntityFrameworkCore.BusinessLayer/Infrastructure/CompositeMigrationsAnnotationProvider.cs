using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
    public class CompositeMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
	    private readonly IEnumerable<IMigrationsAnnotationProvider> providers;

	    public CompositeMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies, IEnumerable<IMigrationsAnnotationProvider> providers)
			: base(dependencies)
	    {
	        this.providers = providers;
	    }

	    public override IEnumerable<IAnnotation> For(IModel model)
	    {
		    return base.For(model).Concat(providers.SelectMany(provider => provider.For(model)));
        }

	    public override IEnumerable<IAnnotation> For(IKey key)
	    {
	        return base.For(key).Concat(providers.SelectMany(provider => provider.For(key)));
        }

	    public override IEnumerable<IAnnotation> For(IIndex index)
	    {
	        return base.For(index).Concat(providers.SelectMany(provider => provider.For(index)));
        }

	    public override IEnumerable<IAnnotation> For(IForeignKey foreignKey)
	    {
	        return base.For(foreignKey).Concat(providers.SelectMany(provider => provider.For(foreignKey)));
        }

	    public override IEnumerable<IAnnotation> For(IProperty property)
	    {
	        return base.For(property).Concat(providers.SelectMany(provider => provider.For(property)));
        }

	    public override IEnumerable<IAnnotation> For(ISequence sequence)
	    {
	        return base.For(sequence).Concat(providers.SelectMany(provider => provider.For(sequence)));
        }

	    public override IEnumerable<IAnnotation> For(IEntityType entityType)
	    {
	        return base.For(entityType).Concat(providers.SelectMany(provider => provider.For(entityType)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(IModel model)
	    {
	        return base.ForRemove(model).Concat(providers.SelectMany(provider => provider.ForRemove(model)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(IKey key)
	    {
	        return base.ForRemove(key).Concat(providers.SelectMany(provider => provider.ForRemove(key)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(IIndex index)
	    {
	        return base.ForRemove(index).Concat(providers.SelectMany(provider => provider.ForRemove(index)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(IForeignKey foreignKey)
	    {
	        return base.ForRemove(foreignKey).Concat(providers.SelectMany(provider => provider.ForRemove(foreignKey)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(IProperty property)
	    {
	        return base.ForRemove(property).Concat(providers.SelectMany(provider => provider.ForRemove(property)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(ISequence sequence)
	    {
	        return base.ForRemove(sequence).Concat(providers.SelectMany(provider => provider.ForRemove(sequence)));
	    }

	    public override IEnumerable<IAnnotation> ForRemove(IEntityType entityType)
	    {
	        return base.ForRemove(entityType).Concat(providers.SelectMany(provider => provider.ForRemove(entityType)));
	    }
    }
}