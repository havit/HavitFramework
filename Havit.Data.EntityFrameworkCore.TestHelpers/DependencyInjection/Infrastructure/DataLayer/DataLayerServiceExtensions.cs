using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;

public static class DataLayerServiceExtensions
{
	public static IServiceCollection AddDataLayerServices(this IServiceCollection services, ComponentRegistrationOptions options = null)
	{
		services.AddDataLayerCoreServices(options);

		services.TryAddTransient<ILanguageDataSource, LanguageDataSource>();
		services.TryAddTransient<IDataSource<Language>, LanguageDataSource>();

		services.TryAddScoped<ILanguageRepository, LanguageRepository>();
		services.TryAddScoped<IRepository<Language>>(sp => sp.GetRequiredService<ILanguageRepository>());
		services.TryAddScoped<IRepository<Language, int>>(sp => sp.GetRequiredService<ILanguageRepository>());
		services.TryAddScoped<IRepositoryQueryProvider<Language, int>, LanguageRepositoryQueryProvider>();

		services.TryAddScoped<IEntityKeyAccessor<Language, int>, DbEntityKeyAccessor<Language, int>>();

		return services;
	}
}
