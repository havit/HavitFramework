﻿using Havit.Services.FileStorage;

namespace Havit.Services.Azure.FileStorage;

/// <summary>
/// Úložiště souborů jako Azure File Storage. Pro velmi specifické použití! V Azure je obvykle používán <see cref="AzureBlobStorageService" />.
/// Umožňuje jako svůj root použít specifickou složku ve FileShare.
/// 
/// Nepodporuje transparentní šifrování z předka.
/// (Použití šifrování beztak postrádá smysl.)
/// 
/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
/// </summary>
public class AzureFileStorageService<TFileStorageContext> : AzureFileStorageService, IFileStorageService<TFileStorageContext>
	where TFileStorageContext : FileStorageContext
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AzureFileStorageService(AzureFileStorageServiceOptions<TFileStorageContext> options) : base(options)
	{
		// NOOP
	}
}