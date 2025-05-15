using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Patterns.Tests.DataSeeds;

[TestClass]
public class FileStorageDataSeedRunDecisionStatePersisterTests
{
	[TestMethod]
	public void FileStorageDataSeedRunDecisionStatePersister_ReadsWritten()
	{
		// Arrange
		string state = "ABCD";
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ReadsWritten";
		// Act
		FileStorageDataSeedRunDecisionStatePersister persister1 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
		persister1.WriteCurrentState(profileName, state);

		// Assert
		FileStorageDataSeedRunDecisionStatePersister persister2 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
		Assert.AreEqual(state, persister2.ReadCurrentState(profileName));

		// Clean up
		persister1.DeleteCurrentStateFile(profileName);
	}

	[TestMethod]
	public void FileStorageDataSeedRunDecisionStatePersister_ShortStateOverwritesLongerState()
	{
		// Arrange
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ShortStateOverwritesLongerState";
		FileStorageDataSeedRunDecisionStatePersister persister = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));

		// Act
		persister.WriteCurrentState(profileName, "ABCD");
		persister.WriteCurrentState(profileName, "A"); // zapíšeme kratší text po zápisu delšího

		// Assert
		Assert.AreEqual("A", persister.ReadCurrentState(profileName));

		// Clean up
		persister.DeleteCurrentStateFile(profileName);
	}

}
