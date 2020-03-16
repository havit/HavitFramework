using System;
using System.Collections.Generic;
using System.Text;
using Havit.GoogleAnalytics.Measurements;

namespace Havit.GoogleAnalytics.Tests.Measurements
{
    internal class FakeGoogleAnalyticsMeasurementApiConfiguration : IGoogleAnalyticsMeasurementApiConfiguration
    {
        public string MeasurementEndpointUrl => "fakegaendpoint";

        public string GoogleAnalyticsTrackingId => "UA-FAKE";
    }
}
