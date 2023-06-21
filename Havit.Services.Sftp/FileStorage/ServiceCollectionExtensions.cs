using System;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;

namespace Havit.Services.Sftp.FileStorage;

/// <summary>
/// Extension metody k IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Zaregistruje úložiště souborů jakožto klienta SFTP serveru.
	/// </summary>
	public static void AddSftpStorageService<TFileStorageContext>(this IServiceCollection services, Func<ConnectionInfo> connectionInfoFunc)
		where TFileStorageContext : FileStorageContext
	{
		Contract.Requires(connectionInfoFunc != null);

		var options = new SftpStorageServiceOptions<TFileStorageContext> { ConnectionInfoFunc = connectionInfoFunc };
		AddSftpStorageService(services, options);
	}

	/// <summary>
	/// Zaregistruje úložiště souborů jakožto klienta SFTP serveru.
	/// </summary>
	public static void AddSftpStorageService<TFileStorageContext>(this IServiceCollection services, SftpStorageServiceOptions<TFileStorageContext> options)
		where TFileStorageContext : FileStorageContext
	{
		services.AddTransient<IFileStorageService<TFileStorageContext>, SftpStorageService<TFileStorageContext>>();
		services.AddSingleton<SftpStorageServiceOptions<TFileStorageContext>>(options);
	}

}
