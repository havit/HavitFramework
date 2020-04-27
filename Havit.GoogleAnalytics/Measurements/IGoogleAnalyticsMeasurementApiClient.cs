using Havit.GoogleAnalytics.Measurements.Events;
using Havit.GoogleAnalytics.Measurements.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// <exception cref="ValidationException">When a required property is missing or other requirements are not met</exception>
        Task TrackEventAsync<TModel>(TModel eventModel)
            where TModel : MeasurementEvent;

        /// <summary>
        /// Method that provides basic validation of parameters, serialization and calling of the GA API endpoint
        /// </summary>
        /// <param name="transactionModel">Basic and extendable model of the transaction hit</param>
        /// <param name="transactionItems">
        /// View models for items inside a transaction.
        /// All common properties from <see cref="MeasurementModelBase"/> will be copied from <paramref name="transactionModel"/>.
        /// </param>
        /// <returns>Awaitable Task</returns>
        /// <exception cref="ValidationException">When a required property is missing or other requirements are not met</exception>
        Task TrackTransactionAsync<TModel>(TModel transactionModel, IEnumerable<MeasurementTransactionItemVM> transactionItems)
            where TModel : MeasurementTransaction;
    }
}
