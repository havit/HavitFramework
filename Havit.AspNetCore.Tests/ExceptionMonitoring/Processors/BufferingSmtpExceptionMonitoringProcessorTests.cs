using Havit.AspNetCore.ExceptionMonitoring.Formatters;
using Havit.AspNetCore.ExceptionMonitoring.Processors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.AspNetCore.Tests.ExceptionMonitoring.Processors
{
    [TestClass]
    public class BufferingSmtpExceptionMonitoringProcessorTests
    {
        [TestMethod]
        public void BufferingSmtpExceptionMonitoringProcessor_ShouldProcessException_ExceptionsAreBuffered()
        {
            // Arrange
            DateTimeOffset currentTime = DateTimeOffset.MinValue;

            // mock setup to allow cache expiration without waiting
            Mock<ISystemClock> clockMock = new Mock<ISystemClock>(MockBehavior.Strict);
            clockMock.Setup(m => m.UtcNow).Returns(() => currentTime);

            var memoryCache = new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions { Clock = clockMock.Object }));
            
            var exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<BufferingSmtpExceptionMonitoringProcessor>>(MockBehavior.Loose);
            var options = Microsoft.Extensions.Options.Options.Create(new BufferingSmtpExceptionMonitoringOptions
            {
                Enabled = false, // do not send emails
                BufferingEnabled = true, // buffering enabled
                BufferingInterval = 60 // buffering interval (seconds)
            });
            var processor = new BufferingSmtpExceptionMonitoringProcessor(exceptionFormatterMock.Object, options, loggerMock.Object, memoryCache);

            Exception exception = new ApplicationException(); // does not have a stack trace

            // Act + Assert
            currentTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            Assert.IsTrue(processor.ShouldProcessException(exception));
            Assert.IsFalse(processor.ShouldProcessException(exception)); // ignored <-- in buffer
            Assert.IsFalse(processor.ShouldProcessException(exception)); // ignored <-- in buffer

            currentTime = currentTime.AddMinutes(2); // let memory cache expire items
            Assert.IsTrue(processor.ShouldProcessException(exception));
            Assert.IsFalse(processor.ShouldProcessException(exception));// ignored <-- in buffer
        }

        [TestMethod]
        public void BufferingSmtpExceptionMonitoringProcessor_ShouldProcessException_ExceptionsAreBufferedByType()
        {
            // Arrange
            var memoryCache = new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions()));

            var exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<BufferingSmtpExceptionMonitoringProcessor>>(MockBehavior.Loose);
            var options = Microsoft.Extensions.Options.Options.Create(new BufferingSmtpExceptionMonitoringOptions
            {
                Enabled = false, // do not send emails
                BufferingEnabled = true, // buffering enabled
                BufferingInterval = 1 // buffering interval is one second
            });
            var processor = new BufferingSmtpExceptionMonitoringProcessor(exceptionFormatterMock.Object, options, loggerMock.Object, memoryCache);

            // Act + Assert
            Assert.IsTrue(processor.ShouldProcessException(new ApplicationException()));
            Assert.IsTrue(processor.ShouldProcessException(new InvalidOperationException()));
        }

        [TestMethod]
        public void BufferingSmtpExceptionMonitoringProcessor_ShouldProcessException_ReturnsTrueWhenBufferingIsDisabled()
        {
            // Arrange
            var memoryCache = new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions()));

            var exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<BufferingSmtpExceptionMonitoringProcessor>>(MockBehavior.Loose);
            var options = Microsoft.Extensions.Options.Options.Create(new BufferingSmtpExceptionMonitoringOptions
            {
                Enabled = false, // do not send emails
                BufferingEnabled = false, // buffering disabled
                BufferingInterval = 1 // buffering interval is one second
            });
            var processor = new BufferingSmtpExceptionMonitoringProcessor(exceptionFormatterMock.Object, options, loggerMock.Object, memoryCache);

            Exception e = new ApplicationException();

            // Act + Assert
            Assert.IsTrue(processor.ShouldProcessException(e));
            Assert.IsTrue(processor.ShouldProcessException(e));
            Assert.IsTrue(processor.ShouldProcessException(e));
        }

        public void BufferingSmtpExceptionMonitoringProcessor_ExceptionInfo_InstancesWithSameValuesAreEqual()
        {
            // Arrange
            object exceptioninfo1 = new BufferingSmtpExceptionMonitoringProcessor.ExceptionInfo
            {
                ExceptionType = typeof(ApplicationException),
                StackTrace = nameof(BufferingSmtpExceptionMonitoringProcessor.ExceptionInfo.StackTrace) // just some string
            };

            object exceptioninfo2 = new BufferingSmtpExceptionMonitoringProcessor.ExceptionInfo
            {
                ExceptionType = typeof(ApplicationException),
                StackTrace = nameof(BufferingSmtpExceptionMonitoringProcessor.ExceptionInfo.StackTrace) // just some string
            };

            // Act
            // no action

            // Assert
            Assert.AreEqual(exceptioninfo1, exceptioninfo2);
        }

    }
}
