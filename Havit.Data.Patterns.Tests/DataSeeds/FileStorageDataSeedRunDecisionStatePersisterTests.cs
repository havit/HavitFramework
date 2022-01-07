using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class FileStorageDataSeedRunDecisionStatePersisterTests
	{
		[TestMethod]
		public void FileStorageDataSeedRunDecisionStatePersister_ReadsWritten()
		{
			// Arrange
			string state = "ABCD";
			IDataSeedProfile profile = new DefaultProfile();

			// Act
			FileStorageDataSeedRunDecisionStatePersister persister1 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			persister1.WriteCurrentState(profile.ProfileName, state);

			// Assert
			FileStorageDataSeedRunDecisionStatePersister persister2 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			Assert.AreEqual(state, persister2.ReadCurrentState(profile.ProfileName));

			// Clean up
			persister1.DeleteCurrentStateFile(profile.ProfileName);
		}

		[TestMethod]
		public void FileStorageDataSeedRunDecisionStatePersister_ShortStateOverwritesLongerState()
		{
			// Arrange
			FileStorageDataSeedRunDecisionStatePersister persister = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			IDataSeedProfile profile = new DefaultProfile();

			// Act
			persister.WriteCurrentState(profile.ProfileName, "ABCD");
			persister.WriteCurrentState(profile.ProfileName, "A"); // zapíšeme kratší text po zápisu delšího

			// Assert
			Assert.AreEqual("A", persister.ReadCurrentState(profile.ProfileName));

			// Clean up
			persister.DeleteCurrentStateFile(profile.ProfileName);
		}

	}
}
