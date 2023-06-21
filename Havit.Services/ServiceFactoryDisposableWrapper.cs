using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services;

/// <summary>
/// Třída pro použití vyzvednutí a uvolnění služby z factory v patternu using.
/// Implementovaný operátor implicitní konverze navíc umožňuje snadné použití resolvované služby.
/// <pre>
/// IServiceFactory&lt;IService&gt; serviceFactory = ...;
/// using (var service = serviceFactory.CreateDisposableService())
/// {
///    // Proměnná service je typu ServiceFactoryDisposableWrapper&lt;IService&gt;, díky implicitní konverzi můžeme použít metody IService přímo.
///    service.DoSomething();
/// }
/// </pre>
/// </summary>
public class ServiceFactoryDisposableWrapper<TService> : IDisposable
	where TService : class
{
	private readonly IServiceFactory<TService> serviceFactory;

	/// <summary>
	/// Konstruktor. Internal - předpokládá se použití z extension metody ServiceFactoryExtensions.CreateDisposableService&lt;TService&gt;(this IServiceFactory&lt;TService&gt; serviceFactory).
	/// Vyzvedává požadovanou službu z factory.
	/// </summary>
	internal ServiceFactoryDisposableWrapper(IServiceFactory<TService> serviceFactory)
	{
		this.serviceFactory = serviceFactory;
		Service = serviceFactory.CreateService();
	}

	/// <summary>
	/// Služba získaná z factory.
	/// </summary>
	public TService Service { get; }

	/// <summary>
	/// Implicitní konverze wrapperu na požadovanou službu.
	/// </summary>
	public static implicit operator TService(ServiceFactoryDisposableWrapper<TService> wrapper) => wrapper.Service;

	/// <summary>
	/// Provede vrácení (uvolnění) získané služby.
	/// </summary>
	public void Dispose()
	{
		serviceFactory.ReleaseService(Service);
	}
}
