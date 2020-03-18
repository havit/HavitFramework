using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements
{
    /// <summary>
    /// This class contains required parameters for each measurement hit
    /// </summary>
    /// <remarks>
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters
    /// </remarks>
    public abstract class MeasurementModelBase
    {
        /// <summary>
        /// [Required]
        /// The type of hit. 
        /// Must be one of 'pageview', 'screenview', 'event', 'transaction', 'item', 'social', 'exception', 'timing'.
        /// </summary>
        [ParameterName("t")]
        public abstract MeasurementHitType HitType { get; }

        /// <summary>
        /// [Required]
        /// This field is required if User ID (uid) is not specified in the request. 
        /// This anonymously identifies a particular user, device, or browser instance. 
        /// For the web, this is generally stored as a first-party cookie with a two-year expiration.
        /// </summary>
        [ParameterName("cid")]
        public string ClientId { get; set; }

        /// <summary>
        /// [Required]
        /// This field is required if Client ID (cid) is not specified in the request. 
        /// This is intended to be a known identifier for a user provided by the site owner/tracking library user.
        /// It must not itself be PII (personally identifiable information). 
        /// The value should never be persisted in GA cookies or other Analytics provided storage.
        /// </summary>
        [ParameterName("uid")]
        public string UserId { get; set; }

        /// <summary>
        /// [Required]
        /// The Protocol version. The current value is '1'. 
        /// This will only change when there are changes made that are not backwards compatible.
        /// </summary>
        [Required]
        [ParameterName("v")]
        public string Version { get; internal set; } = "1";

        /// <summary>
        /// [Required]
        /// The tracking ID / web property ID. The format is UA-XXXX-Y. All collected data is associated by this ID.
        /// This value is defined in <see cref="IGoogleAnalyticsMeasurementApiConfiguration"/>
        /// </summary>
        [Required]
        [ParameterName("tid")]
        public string TrackingId { get; internal set; }

        /// <summary>
        /// [Optional]
        /// Indicates the data source of the hit.
        /// Hits sent from analytics.js will have data source set to 'web'; hits sent from one of the mobile SDKs will have data source set to 'app'.
        /// </summary>
        [ParameterName("ds")]
        public string DataSource { get; set; }

        /// <summary>
        /// [Optional]
        /// Specifies that a hit be considered non-interactive.
        /// </summary>
        public bool NonInteractive { get; set; }

        /// <summary>
        /// Each custom dimension has an associated index.
        /// There is a maximum of 20 custom dimensions (200 for Analytics 360 accounts).
        /// The dimension index must be a positive integer between 1 and 200, inclusive.
        /// </summary>
        [ParameterName("cd")]
        public Dictionary<int, string> CustomDimensions { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Each custom metric has an associated index.
        /// There is a maximum of 20 custom metrics (200 for Analytics 360 accounts).
        /// The metric index must be a positive integer between 1 and 200, inclusive.
        /// </summary>
        [ParameterName("cm")]
        public Dictionary<int, int> CustomMetrics { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// This method copies properties from the instance object into the <paramref name="target"/>
        /// </summary>
        /// <param name="target">Target instance to copy parameters to</param>
        public virtual void CopyTo(MeasurementModelBase target)
        {
            target.ClientId = ClientId;
            target.UserId = UserId;
            target.Version = Version;
            target.TrackingId = TrackingId;
            target.DataSource = DataSource;
            target.NonInteractive = NonInteractive;
            target.CustomDimensions = CustomDimensions;
            target.CustomMetrics = CustomMetrics;
        }
    }
}
