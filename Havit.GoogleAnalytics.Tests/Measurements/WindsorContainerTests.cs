using Castle.Windsor;
using Havit.GoogleAnalytics.Measurements;
using Havit.GoogleAnalytics.Measurements.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Tests.Measurements
{
    [TestClass]
    public class WindsorContainerTests
    {
        [TestMethod]
        public void WindsorContainer_CanResolve()
        {
            WindsorContainer windsorContainer = new WindsorContainer();
            windsorContainer.RegisterGoogleAnalyticMeasurementApiClient<FakeGoogleAnalyticsMeasurementApiConfiguration>();

            var client = windsorContainer.Resolve<IGoogleAnalyticsMeasurementApiClient>();

            Assert.IsNotNull(client);
        }
    }
}
