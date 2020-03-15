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
    public abstract class GAMeasurementModelBase
    {
        /// <summary>
        /// [Required]
        /// The type of hit. 
        /// Must be one of 'pageview', 'screenview', 'event', 'transaction', 'item', 'social', 'exception', 'timing'.
        /// </summary>
        [ParameterName("t")]
        public abstract GAMeasurementHitType HitType { get; }

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
    }
}
