using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements;

    /// <summary>
    /// Configuration for the <see cref="IGoogleAnalyticsMeasurementApiClient"/>
    /// </summary>
    public interface IGoogleAnalyticsMeasurementApiConfiguration
    {
        /// <summary>
        /// Measurement protocol endpoint url obtainable from GA dashboard
        /// </summary>
        string MeasurementEndpointUrl { get; }

        /// <summary>
        /// The tracking ID / web property ID. The format is UA-XXXX-Y. All collected data is associated by this ID.
        /// </summary>
        /// <remarks>
        /// http://blog.analytics-toolkit.com/2016/google-analytics-tracking-code-id-check-setup/
        /// </remarks>
        string GoogleAnalyticsTrackingId { get; }
    }
