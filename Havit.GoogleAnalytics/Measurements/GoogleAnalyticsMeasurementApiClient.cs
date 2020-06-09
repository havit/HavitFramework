using Havit.Diagnostics.Contracts;
using Havit.GoogleAnalytics.Measurements.Events;
using Havit.GoogleAnalytics.Measurements.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics.Measurements
{
	/// <inheritdoc/>
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
		/// Creates new instance of <see cref="GoogleAnalyticsMeasurementApiClient"/>
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

		/// <inheritdoc/>
		public void UseCustomSerializer(IGoogleAnalyticsModelSerializer modelSerializer)
		{
			this.modelSerializer = modelSerializer;
		}

		/// <inheritdoc/>
		public Task TrackEventAsync<TModel>(TModel eventModel)
			where TModel : MeasurementEvent
		{
			Contract.Requires<ArgumentNullException>(eventModel != null);
			// Set internal properties
			eventModel.Version = GoogleApiVersion;
			eventModel.TrackingId = configuration.GoogleAnalyticsTrackingId;
			// Validate
			new MeasurementEventValidator().Validate(eventModel);
			// Serialize
			var itemPostData = modelSerializer.SerializeModel(eventModel);
			// Post
			return requestSender.PostAsync(configuration.MeasurementEndpointUrl, new FormUrlEncodedContent(itemPostData));
		}

		/// <inheritdoc/>
		public async Task TrackTransactionAsync<TModel>(TModel transactionModel, IEnumerable<MeasurementTransactionItemVM> transactionItems)
			where TModel : MeasurementTransaction
		{
			Contract.Requires<ArgumentNullException>(transactionModel != null);
			Contract.Requires<ArgumentNullException>(transactionItems != null);
			// Set internal properties
			transactionModel.Version = GoogleApiVersion;
			transactionModel.TrackingId = configuration.GoogleAnalyticsTrackingId;
			//Validate
			new MeasurementTransactionValidator().Validate(transactionModel);
			//Serialize
			var transactionData = modelSerializer.SerializeModel(transactionModel);
			// Post
			await requestSender.PostAsync(configuration.MeasurementEndpointUrl, new FormUrlEncodedContent(transactionData)).ConfigureAwait(false);

			List<Task> taskList = new List<Task>();
			foreach (var itemVm in transactionItems)
			{
				// Map to ItemModel and copy properties from TransactionModel
				var itemModel = new MeasurementTransactionItem(transactionModel.TransactionId, itemVm);
				transactionModel.CopyTo(itemModel);
				// Validate
				new MeasurementTransactionItemValidator().Validate(itemModel);
				// Serialize
				var itemData = modelSerializer.SerializeModel(itemModel);
				// Post
				taskList.Add(requestSender.PostAsync(configuration.MeasurementEndpointUrl, new FormUrlEncodedContent(itemData)));
			}

			await Task.WhenAll(taskList).ConfigureAwait(false);
		}
	}
}
