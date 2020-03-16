using Havit.GoogleAnalytics.Measurements;
using Havit.GoogleAnalytics.Measurements.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Tests.Measurements
{
    [TestClass]
    public class ServiceCollectionTests
    {
        [TestMethod]
        public void ServiceCollection_CanResolve_IGoogleAnalyticsMeasurementApiClient()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IGoogleAnalyticsMeasurementApiConfiguration>(new FakeGoogleAnalyticsMeasurementApiConfiguration());
            services.AddGoogleAnalyticMeasurementApiClient();
            var provider = services.BuildServiceProvider();
            var client = provider.GetRequiredService<IGoogleAnalyticsMeasurementApiClient>();

            Assert.IsNotNull(client);
        }
    }
}
