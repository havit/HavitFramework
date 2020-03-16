using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.Events
{
    /// <summary>
    /// Event Tracking.
    /// </summary>
    /// <remarks>
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters#events
    /// </remarks>
    public class MeasurementEvent : MeasurementModelBase
    {
        /// <summary>
        /// Defines event hit type.
        /// </summary>
        public override MeasurementHitType HitType => MeasurementHitType.Event;

        /// <summary>
        /// [Required]
        /// Specifies the event category. Must not be empty.
        /// </summary>
        [Required]
        [ParameterName("ec")]
        public virtual string Category { get; set; }

        /// <summary>
        /// [Required]
        /// Specifies the event action. Must not be empty.
        /// </summary>
        [Required]
        [ParameterName("ea")]
        public virtual string Action { get; set; }

        /// <summary>
        /// [Optional]
        /// Specifies the event label.
        /// </summary>
        [ParameterName("el")]
        public virtual string Label { get; set; }

        /// <summary>
        /// [Optional]
        /// Specifies the event value. Values must be non-negative.
        /// </summary>
        [ParameterName("ev")]
        public virtual int? Value { get; set; }
    }
}
