
using Havit.Services.Caching;
using Microsoft.AspNetCore.Http.Connections.Client;

namespace Havit.Services.SignalR.Caching;

/// <summary>
/// Nastavení pro konfiguraci distributed cache invalidation.
/// </summary>
public class DistributedCacheInvalidationOptions
{
	/// <summary>
	/// Typ <see cref="ICacheService"/> pro registraci jako lokální cache service. Výchozí hodnotou je <see cref="MemoryCacheService"/>.
	/// </summary>
	public Type LocalCacheServiceType { get; set; } = typeof(MemoryCacheService);

	/// <summary>
	/// Indikuje, zda jde o režim a běh na "jediném aplikačním webovém serveru, který je zároveň Hub hostem.
	/// </summary>
	public bool IsSingleInstanceHubHost { get; set; } = false;

	/// <summary>
	/// Indikuje, zde mají být zaregistrovány služby pro odeslání dat pro invalidaci na SignalR hub.
	/// </summary>
	public bool SendCacheInvalidations { get; set; } = true;

	/// <summary>
	/// Indikuje, zde mají být zaregistrovány služby přijímající data pro invalidaci ze SignalR hub.
	/// </summary>
	public bool ReceiveCacheInvalidations { get; set; } = true;

	/// <summary>
	/// Url hubu, ke kterému se budou služby distribuvané invalidace cache připojovat.
	/// </summary>
	public string HubUrl { get; set; }

	/// <summary>
	/// Indikuje, zda má při připojení SignalR spojení dojít k vyčištění cache.
	/// Má sloužit k situaci, kdy máme klienta, který má svou cache, ale vypadne mu SignalR spojení
	/// takže nedostává zprávy k invalidaci.
	/// Může tak mít v cache hodnoty, které již nejsou platné.
	/// Po připojení se můžeme volitelně data z cache odstranit, čímž budeme mít jistotu,
	/// že cache neobsahuje potenciálně nevalidní položky.
	/// </summary>
	public bool ClearLocalCacheWhenReceiverConnects { get; set; } = true;

	/// <summary>
	/// The delegate that configures the <see cref="HttpConnection"/>.
	/// </summary>
	public Action<HttpConnectionOptions> ConfigureHttpConnection { get; set; }

	/// <summary>
	/// Pokud jsou nějaká data k invalidaci počkáme chvilku, zda nebude nasbíráno více dat, která budou odeslána najednou.
	/// Čas v milisekundách.
	/// </summary>
	public int SenderWaitTimeToBufferMessages { get; set; } = 20;
}
