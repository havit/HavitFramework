using Havit.Services.Caching;
using Havit.Services.SignalR.Caching;
using Havit.Services.SignalR.Caching.BackgroundServices;
using Havit.Services.SignalR.Caching.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Services.SignalR.Tests.Caching;

[TestClass]
public class ServiceCollectionExtensionsTests
{
	[TestMethod]
	public void ServiceCollectionExtensions_AddDistributedCacheInvalidation_Defaults()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act + Assert
		services.AddDistributedCacheInvalidation(o =>
		{
			Assert.IsTrue(o.SendCacheInvalidations, nameof(o.SendCacheInvalidations));
			Assert.IsTrue(o.ReceiveCacheInvalidations, nameof(o.ReceiveCacheInvalidations));
			Assert.IsFalse(o.IsSingleInstanceHubHost, nameof(o.IsSingleInstanceHubHost));
			Assert.IsNull(o.HubUrl, nameof(o.HubUrl));
			Assert.AreEqual(typeof(MemoryCacheService), o.LocalCacheServiceType, nameof(o.IsSingleInstanceHubHost));
			o.IsSingleInstanceHubHost = true;
		});
	}

	/// <summary>
	/// Scénáø: Typická webová aplikace bìžící v jedné instanci, která je i SignalR Hub hostitelem.
	/// </summary>
	[TestMethod]
	public void ServiceCollectionExtensions_AddDistributedCacheInvalidation_SenderReceiver_SingleInstanceHubHost()
	{
		// Arrange
		var sp = CreateServiceProvider(services =>
		{
			// Act
			services.AddDistributedCacheInvalidation(o =>
			{
				o.IsSingleInstanceHubHost = true;
			});
		});

		// Assert(s)

		// Assert: CacheService
		Assert.IsInstanceOfType<DistributedCacheInvalidationCacheService>(sp.GetRequiredService<ICacheService>(), nameof(ICacheService));
		Assert.IsInstanceOfType<MemoryCacheService>(sp.GetRequiredKeyedService<ICacheService>(DistributedCacheInvalidationCacheService.LocalCacheServiceKey), nameof(ICacheService) + " (" + DistributedCacheInvalidationCacheService.LocalCacheServiceKey + ")");
		Assert.IsInstanceOfType<DistributedCacheInvalidationStorageService>(sp.GetRequiredService<IDistributedCacheInvalidationStorageService>());

		// Assert: Sender
		Assert.IsNotNull(sp.GetHostedService<DistributedCacheInvalidationSenderBackgroundService>());
		Assert.IsInstanceOfType<DistributedCacheInvalidationInAppHostedHubContextSenderService>(sp.GetRequiredService<IDistributedCacheInvalidationSenderService>());

		// Assert: Receiver
		Assert.IsNull(sp.GetHostedService<DistributedCacheInvalidationReceiverBackgroundService>());

		// Assert: Sender+Receiver
		Assert.IsNull(sp.GetService<ISignalRConnectionProvider>());
	}

	/// <summary>
	/// Scénáø: Samostatná aplikace hostující pouze SignalR hub.
	/// </summary>
	[TestMethod]
	public void ServiceCollectionExtensions_AddDistributedCacheInvalidation_NoSenderNoReceiver_SingleInstanceHubHost()
	{
		// Arrange
		var sp = CreateServiceProvider(services =>
		{
			// Act
			services.AddDistributedCacheInvalidation(o =>
			{
				o.SendCacheInvalidations = false;
				o.ReceiveCacheInvalidations = false;
				o.IsSingleInstanceHubHost = true;
				o.LocalCacheServiceType = null;
			});
		});

		// Assert(s)

		// Assert: CacheService		
		Assert.IsNull(sp.GetService<ICacheService>(), nameof(ICacheService));
		Assert.IsNull(sp.GetKeyedService<ICacheService>(DistributedCacheInvalidationCacheService.LocalCacheServiceKey), nameof(ICacheService) + " (" + DistributedCacheInvalidationCacheService.LocalCacheServiceKey + ")");
		Assert.IsNull(sp.GetService<IDistributedCacheInvalidationStorageService>());

		// Assert: Sender
		Assert.IsNull(sp.GetHostedService<DistributedCacheInvalidationSenderBackgroundService>());
		Assert.IsNull(sp.GetService<IDistributedCacheInvalidationSenderService>());

		// Assert: Receiver
		Assert.IsNull(sp.GetHostedService<DistributedCacheInvalidationReceiverBackgroundService>());

		// Assert: Sender+Receiver
		Assert.IsNull(sp.GetService<ISignalRConnectionProvider>());
	}

	/// <summary>
	/// Scénáø: Aplikace pøipojující se k SignalR hubu.
	/// </summary>
	[TestMethod]
	public void ServiceCollectionExtensions_AddDistributedCacheInvalidation_SenderReceiver_AnotherApp()
	{
		// Arrange
		var sp = CreateServiceProvider(services =>
		{
			// Act
			services.AddDistributedCacheInvalidation(o =>
			{
				o.IsSingleInstanceHubHost = false; // = default
				o.HubUrl = "https://fake/address";
			});
		});

		// Assert: CacheService
		Assert.IsInstanceOfType<DistributedCacheInvalidationCacheService>(sp.GetRequiredService<ICacheService>(), nameof(ICacheService));
		Assert.IsInstanceOfType<MemoryCacheService>(sp.GetRequiredKeyedService<ICacheService>(DistributedCacheInvalidationCacheService.LocalCacheServiceKey), nameof(ICacheService) + " (" + DistributedCacheInvalidationCacheService.LocalCacheServiceKey + ")");
		Assert.IsInstanceOfType<DistributedCacheInvalidationStorageService>(sp.GetRequiredService<IDistributedCacheInvalidationStorageService>());

		// Assert: Sender
		Assert.IsNotNull(sp.GetHostedService<DistributedCacheInvalidationSenderBackgroundService>());
		Assert.IsInstanceOfType<DistributedCacheInvalidationHubSenderService>(sp.GetRequiredService<IDistributedCacheInvalidationSenderService>());

		// Assert: Receiver
		Assert.IsNotNull(sp.GetHostedService<DistributedCacheInvalidationReceiverBackgroundService>());

		// Assert: Sender+Receiver
		Assert.IsNotNull(sp.GetRequiredService<ISignalRConnectionProvider>());
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ServiceCollectionExtensions_AddDistributedCacheInvalidation_ThrowsExceptionWhenHubUrlIsMissingButRequired()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddDistributedCacheInvalidation(o =>
		{
			o.IsSingleInstanceHubHost = false;
			o.HubUrl = "";
		});

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ServiceCollectionExtensions_AddDistributedCacheInvalidation_ThrowsExceptionWhenHubUrlIsSetButNotRequired()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddDistributedCacheInvalidation(o =>
		{
			o.IsSingleInstanceHubHost = true;
			o.HubUrl = "https://fake/address";
		});

		// Assert by method attribute
	}

	private IServiceProvider CreateServiceProvider(Action<IServiceCollection> action)
	{
		ServiceCollection services = new ServiceCollection();
		services.AddSingleton(new MemoryCacheServiceOptions());
		services.AddMemoryCache();
		services.AddLogging();
		services.AddSingleton(new Mock<IHubContext<DistributedCacheInvalidationInAppHostedHub>>(MockBehavior.Strict).Object);

		action(services);

		return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
	}
}