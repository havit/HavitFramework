using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

		internal static void FileStorageService_SaveDoNotAcceptSeekedStream(IFileStorageService fileStorageService)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(65 /* A */);
				fileStorageService.Save("test.txt", ms, "text/plain");
			}
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
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, null, testFilename), "no mask");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "*", testFilename), "*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "*.*", testFilename), "*.*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "test*", testFilename), "test*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "*123*", testFilename), "*123*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "t??t??????.t?t", testFilename), "t??t??????.t?t");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "t*.txt", testFilename), "t*.txt");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "t*[??.txt", testFilename), "t*[??.txt");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "t*].txt", testFilename), "t*].txt");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "t*#?.txt", testFilename), "t*š?.txt");

			Assert.IsFalse(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "test###*.txt", testFilename), "test###*.txt");
			Assert.IsFalse(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "est*", testFilename), "est*"); // začátek
			Assert.IsFalse(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "*.tx", testFilename), "*.tx"); // konec

			// Clean-up
			fileStorageService.Delete(testFilename);
		}

		internal static void FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(FileStorageServiceBase fileStorageService)
		{
			Contract.Requires(fileStorageService.SupportsBasicEncryption);

			// Arrange
			string testFilename = "encryption.txt";
			string contentLine = "abcdefghijklmnopqrśtuvwxyz";

			// zapíšeme 3 řádky
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					for (int i = 0; i < 1000; i++)
					{
						sw.WriteLine(contentLine);
					}
				}
				ms.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(testFilename, ms, "text/plain");
			}

			// Act

			// přečteme jen jednu řádku, poté provedeme dispose
			using (Stream stream = fileStorageService.Read(testFilename))
			using (StreamReader reader = new StreamReader(stream))
			{
				reader.ReadLine();
			}

			// Assert
			// no exception was thrown (vs. System.Security.Cryptography.CryptographicException: Výplň je neplatná a nelze ji odebrat.)

			// Clean-up
			fileStorageService.Delete(testFilename);

		}
		private static bool FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(IFileStorageService fileStorageService, string searchPattern, string testFilename)
		{
			return fileStorageService.EnumerateFiles(searchPattern).Any(fileInfo => String.Equals(fileInfo.Name, testFilename, StringComparison.InvariantCultureIgnoreCase));

		}

	}
}
