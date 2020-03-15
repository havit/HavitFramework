using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements
{
    /// <summary>
    /// Configuration for the <see cref="IGoogleAnalyticsMeasurementApiClient"/>
    /// </summary>
    public interface IGoogleAnalyticsMeasurementApiConfiguration
    {
        /// <summary>
        /// Measurement protocol endpoint url obtainable from GA dashboard
        /// </summary>
        string GoogleAnalyticsMeasurementProtocolEndpointUrl { get; }

        /// <summary>
        /// Tracking ID obtainable from GA dashboard
        /// </summary>
        /// <remarks>
        /// https://support.google.com/analytics/thread/13109681
        /// </remarks>
        string GoogleAnalyticsTrackingId { get; }
    }
}
