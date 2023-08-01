using Havit.Services.FileStorage;

namespace Havit.Services.Tests.FileStorage.Infrastructure;

public class ApplicationFileStorageService : FileStorageWrappingService<TestUnderlyingFileStorage>, IApplicationFileStorageService
{
	public ApplicationFileStorageService(IFileStorageService<TestUnderlyingFileStorage> fileStorageService) : base(fileStorageService)
	{
	}
}
