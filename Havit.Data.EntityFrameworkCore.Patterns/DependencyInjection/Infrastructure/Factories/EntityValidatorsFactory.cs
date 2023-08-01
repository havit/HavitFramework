using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories;

internal class EntityValidatorsFactory : IEntityValidatorsFactory
{
	private readonly IServiceProvider serviceProvider;

	public EntityValidatorsFactory(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public IEnumerable<IEntityValidator<TEntity>> Create<TEntity>()
		where TEntity : class
	{
		return serviceProvider.GetRequiredService<IEnumerable<IEntityValidator<TEntity>>>();
	}
}
