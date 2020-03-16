using Havit.GoogleAnalytics.Measurements.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics.Measurements
{
    /// <summary>
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters
    /// </summary>
    public interface IGoogleAnalyticsMeasurementApiClient
    {
        /// <summary>
		/// Use custom serializer. Default is <see cref="PropertyNameAttributeSerializer"/>
		/// </summary>
        void UseCustomSerializer(IGoogleAnalyticsModelSerializer modelSerializer);

        /// <summary>
        /// Method that provides basic validation of parameters, serialization and calling of the GA API endpoint
        /// </summary>
        /// <param name="eventModel">Basic and extendable model of the event hit</param>
        /// <returns>Awaitable Task</returns>
        Task TrackEventAsync(MeasurementEvent eventModel);
    }
}
