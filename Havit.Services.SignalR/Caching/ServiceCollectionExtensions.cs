using Havit.Services.Caching;
using Havit.Services.SignalR.Caching.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Havit.Services.SignalR.Caching;

/// <summary>
/// Extension metody k <see cref="IServiceCollection"/>. Slouží k registraci distributed cache invalidation do DI containeru.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Zaregistruce služby pro distribuovanou invalidaci cache do DI containeru.
	/// Zajišťuje registraci <see cref="ICacheService"/>.
	/// </summary>
	public static IServiceCollection AddDistributedCacheInvalidation(this IServiceCollection services, Action<DistributedCacheInvalidationOptions> setupAction)
	{
		DistributedCacheInvalidationOptions options = new DistributedCacheInvalidationOptions();
		if (setupAction != null)
		{
			setupAction.Invoke(options);
		}

		if (!options.IsSingleInstanceHubHost && String.IsNullOrEmpty(options.HubUrl))
		{
			throw new InvalidOperationException($"{nameof(options.HubUrl)} must be set when IS NOT a single instance hub host.");
		}

		if (options.IsSingleInstanceHubHost && !String.IsNullOrEmpty(options.HubUrl))
		{
			throw new InvalidOperationException($"{nameof(options.HubUrl)} must NOT be set when IS a single instance hub host.");
		}

		// V každém případě chceme zaregistrovat ICacheService (byť pro scénář samostatně hostovaného hubu nemusí mít význam).
		// Pokud odesíláme, chceme zaregistrovat DistributedCacheInvalidationCacheService, pak potřebujeme ještě službu nad "lokální cache".
		// Pokud přijímáme, potřebujeme službu pro "lokální cache", kterou budeme invalidovat.
		// Pokud ani nepřijímáme a ani neodesíláme (jsme jen hubem), neřešíme žádné speciality okolo cache.

		if (options.SendCacheInvalidations)
		{
			services.TryAddKeyedSingleton(typeof(ICacheService), DistributedCacheInvalidationCacheService.LocalCacheServiceKey, options.LocalCacheServiceType);
			// Dále viz níže: TryAddSingleton<ICacheService, DistributedCacheInvalidationCacheService>()			
		}
		else if (options.ReceiveCacheInvalidations)
		{
			// přijímáme, ale neodesíláme
			services.TryAddSingleton(typeof(ICacheService), options.LocalCacheServiceType);
			services.TryAddKeyedSingleton(typeof(ICacheService), DistributedCacheInvalidationCacheService.LocalCacheServiceKey, (sp, _) => sp.GetService<ICacheService>());
		}
		else
		{
			// nepotřebujeme DistributedCacheInvalidationCacheService.LocalCacheServiceKey
			if (options.LocalCacheServiceType != null)
			{
				services.TryAddSingleton(typeof(ICacheService), options.LocalCacheServiceType);
			}
		}

		if (options.SendCacheInvalidations) // Chceme posílat data k invalidaci cache ostatním.
		{
			// Zaregistruje ICacheService pro distribuovanou cache invalidation.
			// třída DistributedCacheInvalidationCacheService deleguje požadavky
			// a) na ICacheService starající se o lokální cache
			// b) na IDistributedCacheInvalidationStorageService, která se stará o evidenci požadavků k invalidaci
			services.TryAddSingleton<ICacheService, DistributedCacheInvalidationCacheService>();

			// IDistributedCacheInvalidationStorageService se stará o evidenci požadavků k invalidaci.
			services.TryAddSingleton<IDistributedCacheInvalidationStorageService, DistributedCacheInvalidationStorageService>();

			// Zaregistrujeme background service pro odesílání dat.
			services.AddHostedService<DistributedCacheInvalidationSenderBackgroundService>();

			if (options.IsSingleInstanceHubHost)
			{
				// Pokud jsme (jediným) serverem, který obsahuje SignalR Hub, budeme zapisovat události přímo pomocí HubContextu.
				services.TryAddSingleton<IDistributedCacheInvalidationSenderService, DistributedCacheInvalidationInAppHostedHubContextSenderService>();
			}
			else
			{
				// Budeme odesílat data pomocí SignalR.
				services.TryAddSingleton<IDistributedCacheInvalidationSenderService, DistributedCacheInvalidationHubSenderService>();

				// Budeme používat SignalR spojení (závislost DistributedCacheInvalidationHubSenderService).
				services.TryAddSingleton<ISignalRConnectionProvider, SignalRConnectionProvider>();
			}
		}

		if (options.ReceiveCacheInvalidations) // Chceme přijímat data k invalidaci cache.
		{
			if (options.IsSingleInstanceHubHost)
			{
				// NOOP - invalidaci provádí přímo hub
			}
			else
			{
				// Pokud nejsme (jediným) serverem, který obsahuje SignalR Hub, budeme příchozí zpracovávat data pomocí SignalR spojení.

				// Zaregistrujeme background job pro příjem zpráv.
				services.AddHostedService<DistributedCacheInvalidationReceiverBackgroundService>();

				// Budeme používat SignalR spojení (závislost DistributedCacheInvalidationReceiverBackgroundService).
				services.TryAddSingleton<ISignalRConnectionProvider, SignalRConnectionProvider>();
			}
		}

		services.TryAddSingleton(Options.Create(options));

		return services;
	}
}
