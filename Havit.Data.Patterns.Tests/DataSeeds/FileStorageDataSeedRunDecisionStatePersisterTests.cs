using Havit.Data.Patterns.DataSeeds;
using Havit.Services.FileStorage;

namespace Havit.Data.Patterns.Tests.DataSeeds;

[TestClass]
public class FileStorageDataSeedRunDecisionStatePersisterTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void FileStorageDataSeedRunDecisionStatePersister_ReadsWritten()
	{
		// Arrange
		string state = "ABCD";
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ReadsWritten";

		FileStorageDataSeedRunDecisionStatePersister persister1 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
		try
		{
			// Act
			persister1.WriteCurrentState(profileName, state);

			// Assert
			FileStorageDataSeedRunDecisionStatePersister persister2 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			Assert.AreEqual(state, persister2.ReadCurrentState(profileName));
		}
		finally
		{
			// Clean up (i při selhání assertu, ať nezůstává soubor v %TEMP%).
			persister1.DeleteCurrentStateFile(profileName);
		}
	}

	[TestMethod]
	public async Task FileStorageDataSeedRunDecisionStatePersister_ReadsWrittenAsync()
	{
		// Arrange
		string state = "ABCD";
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ReadsWrittenAsync";

		FileStorageDataSeedRunDecisionStatePersister persister1 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
		try
		{
			// Act
			await persister1.WriteCurrentStateAsync(profileName, state, TestContext.CancellationToken);

			// Assert
			FileStorageDataSeedRunDecisionStatePersister persister2 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			Assert.AreEqual(state, await persister2.ReadCurrentStateAsync(profileName, TestContext.CancellationToken));
		}
		finally
		{
			// Clean up (i při selhání assertu, ať nezůstává soubor v %TEMP%).
			persister1.DeleteCurrentStateFile(profileName);
		}
	}

	[TestMethod]
	public void FileStorageDataSeedRunDecisionStatePersister_ShortStateOverwritesLongerState()
	{
		// Arrange
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ShortStateOverwritesLongerState";
		FileStorageDataSeedRunDecisionStatePersister persister = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));

		try
		{
			// Act
			persister.WriteCurrentState(profileName, "ABCD");
			persister.WriteCurrentState(profileName, "A"); // zapíšeme kratší text po zápisu delšího

			// Assert
			Assert.AreEqual("A", persister.ReadCurrentState(profileName));
		}
		finally
		{
			// Clean up (i při selhání assertu, ať nezůstává soubor v %TEMP%).
			persister.DeleteCurrentStateFile(profileName);
		}
	}

	[TestMethod]
	public void FileStorageDataSeedRunDecisionStatePersister_ReadCurrentState_ReturnsNullWhenFileDoesNotExist()
	{
		// Arrange
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ReadCurrentState_ReturnsNullWhenFileDoesNotExist";
		FileStorageDataSeedRunDecisionStatePersister persister = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));

		// Act
		string state = persister.ReadCurrentState(profileName);

		// Assert (neexistuje-li soubor se stavem, vrací se null)
		Assert.IsNull(state);
	}

	[TestMethod]
	public void FileStorageDataSeedRunDecisionStatePersister_ReadsWrittenStateWithDiacritics()
	{
		// Arrange
		string state = "Příliš žluťoučký kůň úpěl ďábelské ódy";
		string profileName = "FileStorageDataSeedRunDecisionStatePersister_ReadsWrittenStateWithDiacritics";
		FileStorageDataSeedRunDecisionStatePersister persister = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));

		try
		{
			// Act
			persister.WriteCurrentState(profileName, state);

			// Assert (UTF-8 round-trip vč. BOM a diakritiky)
			Assert.AreEqual(state, persister.ReadCurrentState(profileName));
		}
		finally
		{
			// Clean up (i při selhání assertu, ať nezůstává soubor v %TEMP%).
			persister.DeleteCurrentStateFile(profileName);
		}
	}
}
