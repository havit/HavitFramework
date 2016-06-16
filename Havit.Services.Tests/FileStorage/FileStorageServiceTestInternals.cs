using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.FileStorage
{
	internal static class FileStorageServiceTestInternals
	{
		internal static void FileStorageService_Exists_ReturnsFalseWhenNotFound(IFileStorageService fileStorageService)
		{
			// Act
			bool exists = fileStorageService.Exists(Guid.NewGuid().ToString()); // spoléháme na náhodné číslo

			// Assert
			Assert.IsFalse(exists);
		}

		internal static void FileStorageService_Exists_ReturnsTrueForExistingBlob(IFileStorageService fileStorageService)
		{
			// Arrange
			string blobName = Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				fileStorageService.Save(blobName, ms, "text/plain");
			}

			bool exists = fileStorageService.Exists(blobName);

			// Assert
			Assert.IsTrue(exists);

			// Clean Up
			fileStorageService.Delete(blobName);
		}

		internal static void FileStorageService_DoesNotExistsAfterDelete(IFileStorageService fileStorageService)
		{
			// Arrange
			string blobName = Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				fileStorageService.Save(blobName, ms, "text/plain");
			}
			Assert.IsTrue(fileStorageService.Exists(blobName));
			fileStorageService.Delete(blobName);

			bool exists = fileStorageService.Exists(blobName);

			// Assert
			Assert.IsFalse(exists);
		}

		internal static void FileStorageService_SavedAndReadContentsAreSame_Perform(IFileStorageService fileStorageService)
		{
			// Arrange
			string content = "abcdefghijklmnopqrśtuvwxyz\r\n12346790\t+ěščřžýáíé";
			string filename = Guid.NewGuid().ToString();

			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					sw.Write(content);
				}
				ms.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(filename, ms, "text/plain");
			}

			using (Stream stream = fileStorageService.Read(filename))
			{
				using (StreamReader sr = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
				{
					// Act
					string readContent = sr.ReadToEnd();
					
					// Assert
					Assert.AreEqual(content, readContent); 
				}
			}

			using (MemoryStream ms = new MemoryStream())
			{
				fileStorageService.ReadToStream(filename, ms);
				ms.Seek(0, SeekOrigin.Begin);
				using (StreamReader sr = new StreamReader(ms, Encoding.UTF8, false, 1024, true))
				{
					// Act
					string readContent = sr.ReadToEnd();

					// Assert
					Assert.AreEqual(content, readContent);
				}
			}

			// Clean-up
			fileStorageService.Delete(filename);
		}

		internal static void FileStorageService_EnumerateFiles_SupportsSearchPattern(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = "test123[#].txt";
			using (MemoryStream ms = new MemoryStream())
			{
				fileStorageService.Save(testFilename, ms, "text/plain");
			}

			// Act + Assert
			Assert.IsTrue(fileStorageService.EnumerateFiles().Contains(testFilename), "no mask");
			Assert.IsTrue(fileStorageService.EnumerateFiles("*").Contains(testFilename), "*");
			Assert.IsTrue(fileStorageService.EnumerateFiles("*.*").Contains(testFilename), "*.*");
			Assert.IsTrue(fileStorageService.EnumerateFiles("test*").Contains(testFilename), "test*");
			Assert.IsTrue(fileStorageService.EnumerateFiles("*123*").Contains(testFilename), "*123*");
			Assert.IsTrue(fileStorageService.EnumerateFiles("t??t??????.t?t").Contains(testFilename), "t??t??????.t?t");
			Assert.IsTrue(fileStorageService.EnumerateFiles("t*.txt").Contains(testFilename), "t*.txt");
			Assert.IsTrue(fileStorageService.EnumerateFiles("t*[??.txt").Contains(testFilename), "t*[??.txt");
			Assert.IsTrue(fileStorageService.EnumerateFiles("t*].txt").Contains(testFilename), "t*].txt");
			Assert.IsTrue(fileStorageService.EnumerateFiles("t*#?.txt").Contains(testFilename), "t*š?.txt");

			Assert.IsFalse(fileStorageService.EnumerateFiles("test###*.txt").Contains(testFilename), "test###*.txt");
			Assert.IsFalse(fileStorageService.EnumerateFiles("est*").Contains(testFilename), "est*"); // začátek
			Assert.IsFalse(fileStorageService.EnumerateFiles("*.tx").Contains(testFilename), "*.tx"); // konec

			// Clean-up
			fileStorageService.Delete(testFilename);
		}

	}
}
