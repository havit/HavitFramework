using Havit.GoogleAnalytics.Measurements;
using Havit.GoogleAnalytics.Measurements.Events;
using Havit.GoogleAnalytics.Measurements.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics.Tests.Measurements;

    [TestClass]
    public class GoogleAnalyticsMeasurementApiClientTests
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

            IGoogleAnalyticsMeasurementApiClient apiClient = new GoogleAnalyticsMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            try
            {
                await apiClient.TrackEventAsync(new MeasurementEvent
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

            IGoogleAnalyticsMeasurementApiClient apiClient = new GoogleAnalyticsMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackEventAsync(new MeasurementEvent
            {
                Action = "action",
                Category = "cat",
                ClientId = "0545",
                Value = 564
            });

            Assert.IsNotNull(content);
            string contentString = await content.ReadAsStringAsync();
            Assert.AreEqual("t=event&ec=cat&ea=action&ev=564&cid=0545&v=1&tid=UA-FAKE", contentString);
        }

        [TestMethod]
        public async Task TrackEventAsync_CustomMetrics_ValidateParametersQuery()
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

            IGoogleAnalyticsMeasurementApiClient apiClient = new GoogleAnalyticsMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackEventAsync(new MeasurementEvent
            {
                Action = "action",
                Category = "cat",
                ClientId = "0545",
                Value = 564,
                CustomMetrics = new Dictionary<int, int>
                {
                    { 4, 444 },
                    { 6, 666 }
                }
            });

            Assert.IsNotNull(content);
            string contentString = await content.ReadAsStringAsync();
            Assert.AreEqual("t=event&ec=cat&ea=action&ev=564&cid=0545&v=1&tid=UA-FAKE&cm4=444&cm6=666", contentString);
        }

        [TestMethod]
        public async Task TrackEventAsync_CustomDimensions_ValidateParametersQuery()
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

            IGoogleAnalyticsMeasurementApiClient apiClient = new GoogleAnalyticsMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackEventAsync(new MeasurementEvent
            {
                Action = "action",
                Category = "cat",
                ClientId = "0545",
                Value = 564,
                CustomDimensions = new Dictionary<int, string>
                {
                    { 4, "custom4" },
                    { 6, "custom6" }
                }
            });

            Assert.IsNotNull(content);
            string contentString = await content.ReadAsStringAsync();
            Assert.AreEqual("t=event&ec=cat&ea=action&ev=564&cid=0545&v=1&tid=UA-FAKE&cd4=custom4&cd6=custom6", contentString);
        }

        /// <summary>
        /// Transaction should be sent first, followed by Transaction items.
        /// Transaction items should have the same basic properties as transaction (such as <see cref="MeasurementModelBase.CustomDimensions"/>
        /// The transaction id (ti) has to be the same for transaction and each of it's items
        /// </summary>
        [TestMethod]
        public async Task TrackTransactionAsync_ValidateParametersQuery()
        {
            List<HttpContent> requests = new List<HttpContent>();

            Mock<IHttpRequestSender> senderMock = new Mock<IHttpRequestSender>(MockBehavior.Strict);
            senderMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback((string urlParam, HttpContent contentParam) => requests.Add(contentParam))
                .Returns(Task.CompletedTask);

            IGoogleAnalyticsMeasurementApiClient apiClient = new GoogleAnalyticsMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackTransactionAsync(new MeasurementTransaction
            {
                TransactionId = "traId",
                ClientId = "0545",
                CustomDimensions = new Dictionary<int, string>
                {
                    { 4, "custom4" },
                    { 6, "custom6" }
                }
            },
            new[]
            {
                new MeasurementTransactionItemVM
                {
                    Name = "traItem1"
                },
                new MeasurementTransactionItemVM
                {
                    Name = "traItem2"
                }
            });

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("t=transaction&ti=traId&cid=0545&v=1&tid=UA-FAKE&cd4=custom4&cd6=custom6", await requests[0].ReadAsStringAsync());
            Assert.AreEqual("t=item&ti=traId&in=traItem1&cid=0545&v=1&tid=UA-FAKE&cd4=custom4&cd6=custom6", await requests[1].ReadAsStringAsync());
            Assert.AreEqual("t=item&ti=traId&in=traItem2&cid=0545&v=1&tid=UA-FAKE&cd4=custom4&cd6=custom6", await requests[2].ReadAsStringAsync());
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

            IGoogleAnalyticsMeasurementApiClient apiClient = new GoogleAnalyticsMeasurementApiClient(
                senderMock.Object,
                GetConfiguration());

            await apiClient.TrackEventAsync(new MeasurementEvent
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
                .Setup(x => x.MeasurementEndpointUrl)
                .Returns(configUrl);
            configMock
                .Setup(x => x.GoogleAnalyticsTrackingId)
                .Returns(configTrackingId);

            return configMock.Object;
        }
    }
