using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Havit.Services.Sftp.Tests.Infrastructure;

internal class SftpContainerService
{
	public static string Hostname => s_SftpContainer.Hostname;
	public static int Port => s_SftpContainer.GetMappedPublicPort(22);

	private static IContainer s_SftpContainer;

	internal static async Task StartContainerAsync(CancellationToken cancellationToken)
	{
		// Není použit atmoz/sftp:alpine z Testcontainers.Sftp, protože neumí nastavit oprávnění pro zápis do rootové složky.
		s_SftpContainer = new ContainerBuilder("emberstack/sftp:latest")
			.WithResourceMapping(new FileInfo("sftp.json"), new FileInfo("/app/config/sftp.json")) 
			.WithPortBinding(22, true) // SFTP
			.WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(22))
			.Build();

		await s_SftpContainer.StartAsync(cancellationToken);
	}

	internal static async Task StopContainerAsync(CancellationToken cancellationToken)
	{
		await s_SftpContainer.StopAsync(cancellationToken);
		await s_SftpContainer.DisposeAsync();
	}
}
