[assembly: DoNotParallelize]

// Při paralelním běhu nad test containerem dostáváme výjimky typu "The connection was closed by the remote host."
// Implementace SftpFileStorageServiceTests.GetSftpFileStorageService předpokládá sekvenční běh testů.