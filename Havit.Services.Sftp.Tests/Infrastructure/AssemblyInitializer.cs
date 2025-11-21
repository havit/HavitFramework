namespace Havit.Services.Sftp.Tests.Infrastructure;

[TestClass]
public static class AssemblyInitializer
{
	[AssemblyInitialize]
	public static async Task AssemblyInitAsync(TestContext testContext)
	{
		await SftpContainerService.StartContainerAsync(testContext.CancellationToken);
	}

	[AssemblyCleanup]
	public static async Task AssemblyCleanupAsync(TestContext testContext)
	{
		await SftpContainerService.StopContainerAsync(testContext.CancellationToken);
	}
}