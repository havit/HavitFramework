using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Tests.FileStorage.Infrastructure;

public class ApplicationFileStorageService : FileStorageWrappingService<TestUnderlyingFileStorage>, IApplicationFileStorageService
{
	public ApplicationFileStorageService(IFileStorageService<TestUnderlyingFileStorage> fileStorageService) : base(fileStorageService)
	{
	}
}
