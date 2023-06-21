using Azure.Core;
using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Azure.FileStorage;

/// <summary>
/// Parametry pro AzureBlobStorageService.
/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
/// </summary>
public class AzureBlobStorageServiceOptions<TFileStorageServiceContext> : AzureBlobStorageServiceOptions
	where TFileStorageServiceContext : FileStorageContext
{
}
