#if NET8_0_OR_GREATER
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Havit.Hangfire.Extensions.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Havit.Hangfire.Extensions.Tests.HealthChecks;

[TestClass]
public class HangfireServerHealthCheckTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public async Task HangfireServerHealthCheck_heckHealthAsync_ReturnsHealthy_WhenServerRunning()
	{
		// Arrange
		var currentTime = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
		var recentHeartbeat = currentTime.AddSeconds(-1);

		var server = new ServerDto
		{
			Name = "server1",
			Heartbeat = recentHeartbeat,
			Queues = ["myqueue"]
		};

		JobStorage jobStorage = CreateJobStorageMock([server]);

		var options = new HangfireServerHealthCheckOptions
		{
			Queue = "myqueue",
			RequiredInstances = 1,
			MaxHeartbeatAgeSeconds = 300
		};

		Mock<HangfireServerHealthCheck> healthCheckMock = new Mock<HangfireServerHealthCheck>(jobStorage, options);
		healthCheckMock.CallBase = true;
		healthCheckMock.SetupGet(m => m.UtcNow).Returns(currentTime);

		// Act
		var result = await healthCheckMock.Object.CheckHealthAsync(TestContext.CancellationToken);

		// Assert
		Assert.AreEqual(HealthStatus.Healthy, result.Status);
	}

	[TestMethod]
	public async Task HangfireServerHealthCheck_CheckHealthAsync_ReturnsUnhealthy_WhenHeartbeatExpired()
	{
		// Arrange
		var currentTime = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
		var expiredHeartbeat = currentTime.AddSeconds(-301);

		var server = new ServerDto
		{
			Name = "server1",
			Heartbeat = expiredHeartbeat,
			Queues = new[] { "myqueue" }
		};

		JobStorage jobStorage = CreateJobStorageMock([server]);

		var options = new HangfireServerHealthCheckOptions
		{
			Queue = "myqueue",
			RequiredInstances = 1,
			MaxHeartbeatAgeSeconds = 300
		};

		Mock<HangfireServerHealthCheck> healthCheckMock = new Mock<HangfireServerHealthCheck>(jobStorage, options);
		healthCheckMock.CallBase = true;
		healthCheckMock.SetupGet(m => m.UtcNow).Returns(currentTime);

		// Act
		var result = await healthCheckMock.Object.CheckHealthAsync(TestContext.CancellationToken);

		// Assert
		Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
	}

	[TestMethod]
	public async Task HangfireServerHealthCheck_CheckHealthAsync_ReturnsUnhealthy_WhenInsufficientInstancesCount()
	{
		// Arrange
		var currentTime = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
		var recentHeartbeat = currentTime.AddSeconds(-60);

		var server = new ServerDto
		{
			Name = "server1",
			Heartbeat = recentHeartbeat,
			Queues = new[] { "myqueue" }
		};

		JobStorage jobStorage = CreateJobStorageMock([server]);

		var options = new HangfireServerHealthCheckOptions
		{
			Queue = "myqueue",
			RequiredInstances = 2,
			MaxHeartbeatAgeSeconds = 300
		};

		Mock<HangfireServerHealthCheck> healthCheckMock = new Mock<HangfireServerHealthCheck>(jobStorage, options);
		healthCheckMock.CallBase = true;
		healthCheckMock.SetupGet(m => m.UtcNow).Returns(currentTime);

		// Act
		var result = await healthCheckMock.Object.CheckHealthAsync(TestContext.CancellationToken);

		// Assert
		Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
	}

	private JobStorage CreateJobStorageMock(List<ServerDto> servers)
	{
		var monitoringApiMock = new Mock<IMonitoringApi>();
		monitoringApiMock.Setup(m => m.Servers()).Returns(servers);

		Mock<JobStorage> jobStorageMock = new Mock<JobStorage>();
		jobStorageMock.Setup(js => js.GetMonitoringApi()).Returns(monitoringApiMock.Object);
		return jobStorageMock.Object;
	}
}
#endif
