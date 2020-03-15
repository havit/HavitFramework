using Havit.GoogleAnalytics.Measurements;
using Havit.GoogleAnalytics.Measurements.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics.Tests.Measurements
{
    [TestClass]
    public class GAMeasurementApiClientTests
    {
        private const string configUrl = "https://fakega.com/";
        private const string configTrackingId = "UA-FAKE";

        [TestMethod]
        public async Task TrackEventAsync_MissingClientIdOrUserId_ThrowException()
        {
            string url;
            HttpContent content;

            Mock<IHttpRequestSender> senderMock = new Mock<IHttpRequestSender>(MockBehavior.Strict);
            senderMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback((string urlParam, HttpContent contentParam) => 
                { 
                    url = urlParam; 
                    content = contentParam; 
                })
                .Returns(Task.CompletedTask);

            IGoogleAnalyticsMeasurementApiClient apiClient = new GAMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            try
            {
                await apiClient.TrackEventAsync(new GAMeasurementEvent
                {
                    Action = "action",
                    Category = "cat"
                });
                Assert.Fail("Expected exception");
            }
            catch (Exception ex)
            {
                ValidationException validationException = ex as ValidationException;
                Assert.IsTrue(validationException != null);
                Assert.IsTrue(validationException.Message.EndsWith("ClientId, UserId: At least one value required."));
            }
        }

        [TestMethod]
        public async Task TrackEventAsync_ValidateParametersQuery()
        {
            string url;
            HttpContent content = null;

            Mock<IHttpRequestSender> senderMock = new Mock<IHttpRequestSender>(MockBehavior.Strict);
            senderMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback((string urlParam, HttpContent contentParam) =>
                {
                    url = urlParam;
                    content = contentParam;
                })
                .Returns(Task.CompletedTask);

            IGoogleAnalyticsMeasurementApiClient apiClient = new GAMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackEventAsync(new GAMeasurementEvent
            {
                Action = "action",
                Category = "cat",
                ClientId = "0545"
            });

            Assert.IsNotNull(content);
            string contentString = await content.ReadAsStringAsync();
            Assert.AreEqual(contentString, "t=event&ec=cat&ea=action&cid=0545&v=1&tid=UA-FAKE");
        }

        [TestMethod]
        public async Task TrackEventAsync_ValidateRequestUrlBase()
        {
            string urlBase = null;
            HttpContent content = null;

            Mock<IHttpRequestSender> senderMock = new Mock<IHttpRequestSender>(MockBehavior.Strict);
            senderMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback((string urlParam, HttpContent contentParam) =>
                {
                    urlBase = urlParam;
                    content = contentParam;
                })
                .Returns(Task.CompletedTask);

            IGoogleAnalyticsMeasurementApiClient apiClient = new GAMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackEventAsync(new GAMeasurementEvent
            {
                Action = "action",
                Category = "cat",
                ClientId = "0545"
            });

            Assert.IsNotNull(urlBase);
            Assert.AreEqual(urlBase, configUrl);
        }

        private IGoogleAnalyticsMeasurementApiConfiguration GetConfiguration()
        {
            Mock<IGoogleAnalyticsMeasurementApiConfiguration> configMock = new Mock<IGoogleAnalyticsMeasurementApiConfiguration>(MockBehavior.Strict);
            configMock
                .Setup(x => x.GoogleAnalyticsMeasurementProtocolEndpointUrl)
                .Returns(configUrl);
            configMock
                .Setup(x => x.GoogleAnalyticsTrackingId)
                .Returns(configTrackingId);

            return configMock.Object;
        }
    }
}
