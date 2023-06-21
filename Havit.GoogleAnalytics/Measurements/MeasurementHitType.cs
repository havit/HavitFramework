using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements;

    /// <summary>
    /// The type of hit.
    /// </summary>
    /// <remarks>
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters#t
    /// </remarks>
    public enum MeasurementHitType
    {
        /// <summary>
        /// Event hit type
        /// </summary>
        [ParameterValue("event")]
        Event,
        /// <summary>
        /// Transaction hit type
        /// </summary>
        [ParameterValue("transaction")]
        Transaction,
        /// <summary>
        /// Transaction item hit type
        /// </summary>
        [ParameterValue("item")]
        Item
    }
