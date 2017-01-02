using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataSeeds;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class FileStorageDataSeedRunDecisionStatePersisterTest
	{
		[TestMethod]
		public void FileStorageDataSeedRunDecisionStatePersister_ReadsWritten()
		{
			// Arrange
			string state = "ABCD";

			// Act
			FileStorageDataSeedRunDecisionStatePersister persister1 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			persister1.WriteCurrentState(state);

			// Assert
			FileStorageDataSeedRunDecisionStatePersister persister2 = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));
			Assert.AreEqual(state, persister2.ReadCurrentState());
		}

		[TestMethod]
		public void FileStorageDataSeedRunDecisionStatePersister_ShortStateOverwritesLongerState()
		{
			// Arrange
			FileStorageDataSeedRunDecisionStatePersister persister = new FileStorageDataSeedRunDecisionStatePersister(new FileSystemStorageService(System.IO.Path.GetTempPath()));

			// Act
			persister.WriteCurrentState("ABCD");
			persister.WriteCurrentState("A"); // zapíšeme kratší text po zápisu delšího

			// Assert
			Assert.AreEqual("A", persister.ReadCurrentState());
		}

	}
}
