# Distributed Cache Invalidation

V aplikacích máme v paměti cachovaná data.
V případě potřeby invalidace těchto dat je cíleně vyhazujeme z cache (`ICacheService.Remove(...)`), ev. provádíme vyčištění veškerého obsahu cache (`ICacheService.Clear()`).
Avšak tato data jsou vyhozena pouze z cache dané aplikace.

Cílem distribuované invalidace cache je vyhodit data, jejiž invalidace byla cíleně vyžádána, též z cache ostatních aplikací (serverů, atp.), pokud se v jejich cache nachází.

K distribuované invalidaci dochází díky komunikaci mezi aplikacemi pomocí SignalR. Řešení pro distribuovanou invalidaci cache vyžaduje právě jeden SignalR hub (zjednodušeně: právě jeden webový server).

## Varianty

### Jeden "aplikační" webový server (bez škálování), bez dalších aplikací
Nepotřebuje distribuovanou invalidace cache.

### Jeden "aplikační" webový server (tj. bez škálování), libovolný počet dalších aplikací (WebJobs, atp.)
* SignalR hub je hostován v rámci webového serveru.
* Zprávy s daty k invalidaci z aplikace pro ostatní předává na SignalR hub pomocí `IHubContext<>`.
* Přijímá zprávy s daty k invalidaci od ostatních ze SignalR hubu (a provádí invalidaci prostřednictvím `IDistributedCacheInvalidationHubLocalMessageHandler`, implementace `DistributedCacheInvalidationHubLocalMessageHandler`).
* Nepřipojuje se sám ke svému hubu pomocí SignalR.

### Více "aplikačních" webový serverů (tj. škálovaný), libovolný počet dalších aplikací (WebJobs, atp.)
* SignalR hub je hostován na samostatném jednoúčelovém webovém serveru bez škálování, například na jiném App Service Planu než který používá aplikační server.
* SignalR hub pouze distribuuje zprávy mezi klienty SignalR hubu.

* Aplikace předávají SignalR hubu zprávy k invalidaci cache pro ostatní (sender).
* Aplikace přijímají ze SignalR hubu zprávy k invalidaci cache od ostatních (receiver).

Ev. lze řešit pomocí Azure SignalR (neověřeno).

## Uspořádání služeb

### Jeden "aplikační" webový server
* Hostování SignalR hubu
* Aplikace samotná používá `ICacheService`.

```
services.AddDistributedCacheInvalidation(o =>
{
	o.IsSingleInstanceHubHost = true;
});
...
services.AddSignalR();
...
endpoints.MapHub<DistributedCacheInvalidationInAppHostedHub>("/DistributedCacheInvalidation").AllowAnonymous();
```

### Vyhrazený server pro SignalR hub
* Pouze hostování SignalR hubu

```
services.AddSignalR();
...
endpoints.MapHub<DistributedCacheInvalidationStandaloneHub>("/DistributedCacheInvalidation").AllowAnonymous();
```

### Další aplikace
* Připojení k existujícímu SignalR hubu.

```
services.AddDistributedCacheInvalidation(o =>
{
	o.IsSingleInstanceHubHost = false; // = default
	o.HubUrl = "https://.../DistributedCacheInvalidation";
});
```
