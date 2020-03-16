using Havit.Diagnostics.Contracts;
using Havit.GoogleAnalytics.Measurements.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics.Measurements
{
	/// <summary>
	/// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters
	/// </summary>
	public class GoogleAnalyticsMeasurementApiClient : IGoogleAnalyticsMeasurementApiClient
	{
		private readonly IHttpRequestSender requestSender;
		private readonly IGoogleAnalyticsMeasurementApiConfiguration configuration;

		private IGoogleAnalyticsModelSerializer modelSerializer;

		/// <summary>
		/// Version of API's for the measurement protocol
		/// </summary>
		public string GoogleApiVersion { get; set; } = "1";

		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="requestSender">HttpClient abstraction</param>
		/// <param name="configuration">Configuration of this client</param>
		public GoogleAnalyticsMeasurementApiClient(
			IHttpRequestSender requestSender,
			IGoogleAnalyticsMeasurementApiConfiguration configuration)
		{
			this.requestSender = requestSender;
			this.configuration = configuration;

			this.modelSerializer = new PropertyNameAttributeSerializer();
		}

		/// <summary>
		/// Use custom serializer. Default is <see cref="PropertyNameAttributeSerializer"/>
		/// </summary>
		public void UseCustomSerializer(IGoogleAnalyticsModelSerializer modelSerializer)
		{
			this.modelSerializer = modelSerializer;
		}

		/// <summary>
		/// Method that provides basic validation of parameters, serialization and calling of the GA API endpoint
		/// </summary>
		/// <param name="eventModel">Basic and extendable model of the event hit</param>
		/// <returns>Awaitable Task</returns>
		public Task TrackEventAsync(MeasurementEvent eventModel)
		{
			Contract.Requires<ArgumentNullException>(eventModel != null);

			eventModel.Version = GoogleApiVersion;
			eventModel.TrackingId = configuration.GoogleAnalyticsTrackingId;
			new MeasurementEventValidator().Validate(eventModel);
			var itemPostData = modelSerializer.SerializeModel(eventModel);

			return requestSender.PostAsync(configuration.MeasurementEndpointUrl, new FormUrlEncodedContent(itemPostData));
		}
	}
}
