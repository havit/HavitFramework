using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.Azure.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Azure.Test
{
	[TestClass]
	[Ignore]
	public class AzureBlobStorageServiceTest		
	{
		[TestMethod]
		public void AzureBlobStorageService_Exists_ReturnsFalseWhenNotFound()
		{
			// Arrange
			AzureBlobStorageService service = GetAzureBlobStorageService();

			// Act
			bool exists = service.Exists(Guid.NewGuid().ToString()); // spoléháme na náhodné číslo

			// Assert
			Assert.IsFalse(exists);
		}

		[TestMethod]
		public void AzureBlobStorageService_Exists_ReturnsTrueForExistingBlob()
		{
			// Arrange
			AzureBlobStorageService service = GetAzureBlobStorageService();

			string blobName = Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				service.Save(blobName, ms, "text/plain");
			}

			bool exists = service.Exists(blobName);

			// Clean Up
			service.Delete(blobName);

			// Assert
			Assert.IsTrue(exists);
		}

		[TestMethod]
		public void AzureBlobStorageService_ReadsSavedContent()
		{
			// Arrange
			AzureBlobStorageService service = GetAzureBlobStorageService();
			var ms = new MemoryStream();
			ms.WriteByte((byte)'A');
			ms.WriteByte((byte)'B');
			ms.WriteByte((byte)'C');
			ms.Seek(0, SeekOrigin.Begin);

			string blobName = Guid.NewGuid().ToString();

			// Act + Assert
			service.Save(blobName, ms, "plain/text");
			Stream stream = service.Read(blobName);
			Assert.AreEqual((byte)'A', stream.ReadByte());
			Assert.AreEqual((byte)'B', stream.ReadByte());
			Assert.AreEqual((byte)'C', stream.ReadByte());
			Assert.AreEqual(-1, stream.ReadByte());

			// Clean Up
			service.Delete(blobName);
		}

		[TestMethod]
		public void AzureBlobStorageService_DoesNotExistsAfterDelete()
		{
			// Arrange
			AzureBlobStorageService service = GetAzureBlobStorageService();

			string blobName = Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				service.Save(blobName, ms, "text/plain");
			}
			Assert.IsTrue(service.Exists(blobName));
			service.Delete(blobName);

			bool exists = service.Exists(blobName);

			// Assert
			Assert.IsFalse(exists);
		}

		private AzureBlobStorageService GetAzureBlobStorageService()
		{
			return new AzureBlobStorageService("DefaultEndpointsProtocol=https;AccountName=hfwtest;AccountKey=2WjQXR02GG2uu3xiEa9T17nExF/gJ16UYzbIZg5gMV7evaufMX/K1uv+mKwiXwkCbYnOvJjDmHW2LQM/zmrqjA==", "tests");
		}
	}
}
