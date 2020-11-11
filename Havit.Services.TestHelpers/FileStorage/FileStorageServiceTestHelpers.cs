using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileInfo = Havit.Services.FileStorage.FileInfo;

namespace Havit.Services.TestHelpers.FileStorage
{
	public static class FileStorageServiceTestHelpers
	{
		public static void FileStorageService_Exists_ReturnsFalseWhenNotFound(IFileStorageService fileStorageService)
		{
			// Act
			bool exists = fileStorageService.Exists(Guid.NewGuid().ToString()); // spoléháme na náhodné číslo

			// Assert
			Assert.IsFalse(exists);
		}

		public static async Task FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(IFileStorageService fileStorageService)
		{
			// Act
			bool exists = await fileStorageService.ExistsAsync(Guid.NewGuid().ToString()); // spoléháme na náhodné číslo

			// Assert
			Assert.IsFalse(exists);
		}

		public static void FileStorageService_Exists_ReturnsTrueWhenExists(IFileStorageService fileStorageService)
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

		public static async Task FileStorageService_ExistsAsync_ReturnsTrueWhenExists(IFileStorageService fileStorageService)
		{
			// Arrange
			string blobName = Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				await fileStorageService.SaveAsync(blobName, ms, "text/plain");
			}

			bool exists = await fileStorageService.ExistsAsync(blobName);

			// Assert
			Assert.IsTrue(exists);

			// Clean Up
			await fileStorageService.DeleteAsync(blobName);
		}

		public static void FileStorageService_DoesNotExistsAfterDelete(IFileStorageService fileStorageService)
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

		public static async Task FileStorageService_DoesNotExistsAfterDeleteAsync(IFileStorageService fileStorageService)
		{
			// Arrange
			string blobName = Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				await fileStorageService.SaveAsync(blobName, ms, "text/plain");
			}
			Assert.IsTrue(await fileStorageService.ExistsAsync(blobName));
			await fileStorageService.DeleteAsync(blobName);

			bool exists = await fileStorageService.ExistsAsync(blobName);

			// Assert
			Assert.IsFalse(exists);
		}

		public static void FileStorageService_Save_AcceptsPathWithNewSubfolders(IFileStorageService fileStorageService)
		{
			// Arrange
			string folderName = Guid.NewGuid().ToString();
			string fileName = folderName + "/subfolder/" + Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				fileStorageService.Save(fileName, ms, "text/plain");
			}

			// Assert
			Assert.IsTrue(fileStorageService.Exists(fileName));

			// Clean-up
			fileStorageService.Delete(fileName);
		}

		public static async Task FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(IFileStorageService fileStorageService)
		{
			// Arrange
			string folderName = Guid.NewGuid().ToString();
			string fileName = folderName + "/" + Guid.NewGuid().ToString();

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				await fileStorageService.SaveAsync(fileName, ms, "text/plain");
			}

			// Assert
			Assert.IsTrue(await fileStorageService.ExistsAsync(fileName));

			// Clean-up
			await fileStorageService.DeleteAsync(fileName);
		}

		public static void FileStorageService_SaveDoNotAcceptSeekedStream(IFileStorageService fileStorageService)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(65 /* A */);
				fileStorageService.Save("test.txt", ms, "text/plain");
			}
		}

		public static async Task FileStorageService_SaveAsyncDoNotAcceptSeekedStream(IFileStorageService fileStorageService)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(65 /* A */);
				await fileStorageService.SaveAsync("test.txt", ms, "text/plain");
			}
		}

		public static void FileStorageService_SavedAndReadContentsAreSame_Perform(IFileStorageService fileStorageService)
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

		public static async Task FileStorageService_SavedAndReadContentsAreSame_PerformAsync(IFileStorageService fileStorageService)
		{
			// Arrange
			string content = "abcdefghijklmnopqrśtuvwxyz\r\n12346790\t+ěščřžýáíé";
			string filename = Guid.NewGuid().ToString();

			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					await sw.WriteAsync(content);
				}
				ms.Seek(0, SeekOrigin.Begin);

				await fileStorageService.SaveAsync(filename, ms, "text/plain");
			}

			using (Stream stream = await fileStorageService.ReadAsync(filename))
			{
				using (StreamReader sr = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
				{
					// Act
					string readContent = await sr.ReadToEndAsync();

					// Assert
					Assert.AreEqual(content, readContent);
				}
			}

			using (MemoryStream ms = new MemoryStream())
			{
				await fileStorageService.ReadToStreamAsync(filename, ms);
				ms.Seek(0, SeekOrigin.Begin);
				using (StreamReader sr = new StreamReader(ms, Encoding.UTF8, false, 1024, true))
				{
					// Act
					string readContent = await sr.ReadToEndAsync();

					// Assert
					Assert.AreEqual(content, readContent);
				}
			}

			// Clean-up
			await fileStorageService.DeleteAsync(filename);
		}

		public static void FileStorageService_EnumerateFiles_SupportsSearchPattern(IFileStorageService fileStorageService)
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

		public static async Task FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = "test123[#].txt";
			using (MemoryStream ms = new MemoryStream())
			{
				await fileStorageService.SaveAsync(testFilename, ms, "text/plain");
			}

			// Act + Assert
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, null, testFilename), "no mask");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "*", testFilename), "*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "*.*", testFilename), "*.*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "test*", testFilename), "test*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "*123*", testFilename), "*123*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "t??t??????.t?t", testFilename), "t??t??????.t?t");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "t*.txt", testFilename), "t*.txt");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "t*[??.txt", testFilename), "t*[??.txt");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "t*].txt", testFilename), "t*].txt");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "t*#?.txt", testFilename), "t*š?.txt");

			Assert.IsFalse(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "test###*.txt", testFilename), "test###*.txt");
			Assert.IsFalse(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "est*", testFilename), "est*"); // začátek
			Assert.IsFalse(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "*.tx", testFilename), "*.tx"); // konec

			// Clean-up
			await fileStorageService.DeleteAsync(testFilename);
		}

		public static void FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = @"subfolder1/subfolder2/test123.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				fileStorageService.Save(testFilename, ms, "text/plain");
			}

			// Act + Assert
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, null, testFilename), "no mask");

			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"*", testFilename), @"*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"*.*", testFilename), @"*.*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\*", testFilename), @"subfolder1\*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\*.*", testFilename), @"subfolder1\*.*");

			Assert.IsFalse(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"test*", testFilename), @"test*");
			Assert.IsFalse(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2", testFilename), @"subfolder1\subfolder2");
			Assert.IsFalse(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\", testFilename), @"subfolder1\subfolder2\");

			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\*", testFilename), @"subfolder1\subfolder2\*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "subfolder1/subfolder2/*", testFilename), "subfolder1/subfolder2/*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\*.*", testFilename), @"subfolder1\subfolder2\*.*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\test*", testFilename), @"subfolder1\subfolder2\test*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\t*.txt", testFilename), @"subfolder1\subfolder2\t*.txt");

			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"*\*", testFilename), @"*\*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"sub*\subfolder2\*", testFilename), @"sub*\subfolder2*\*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, "sub*/subfolder2/*", testFilename), "sub*/subfolder2*/*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\*", testFilename), @"subfolder1\sub*\*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\*.*", testFilename), @"subfolder1\sub*\*.*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\test*", testFilename), @"subfolder1\sub*\test*");
			Assert.IsTrue(FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\test123.txt", testFilename), @"subfolder1\sub*\test*");

			// složka samotná není nalezena
			Assert.IsFalse(fileStorageService.EnumerateFiles(@"subfolder1").Any(), @"Folder subfolder1");
			Assert.IsFalse(fileStorageService.EnumerateFiles(@"subfolder1\").Any(), @"Folder subfolder1\");
			Assert.IsFalse(fileStorageService.EnumerateFiles(@"subfolder1\subfolder2").Any(), @"Folder subfolder1\subfolder2");
			Assert.IsFalse(fileStorageService.EnumerateFiles("subfolder1/subfolder2").Any(), "Folder subfolder1/subfolder2");
			Assert.IsFalse(fileStorageService.EnumerateFiles(@"subfolder1\subfolder2\").Any(), @"Folder subfolder1\subfolder2\");

			// Clean-up
			fileStorageService.Delete(testFilename);
		}

		public static async Task FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = @"subfolder1/subfolder2/test123.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				await fileStorageService.SaveAsync(testFilename, ms, "text/plain");
			}

			// Act + Assert

			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, null, testFilename), "no mask");

			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"*", testFilename), @"*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"*.*", testFilename), @"*.*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\*", testFilename), @"subfolder1\*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\*.*", testFilename), @"subfolder1\*.*");

			Assert.IsFalse(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"test*", testFilename), @"test*");
			Assert.IsFalse(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2", testFilename), @"subfolder1\subfolder2");
			Assert.IsFalse(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\", testFilename), @"subfolder1\subfolder2\");

			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\*", testFilename), @"subfolder1\subfolder2\*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "subfolder1/subfolder2/*", testFilename), "subfolder1/subfolder2/*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\*.*", testFilename), @"subfolder1\subfolder2\*.*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\test*", testFilename), @"subfolder1\subfolder2\test*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\subfolder2\t*.txt", testFilename), @"subfolder1\subfolder2\t*.txt");

			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"*\*", testFilename), @"*\*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"sub*\subfolder2\*", testFilename), @"sub*\subfolder2*\*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, "sub*/subfolder2/*", testFilename), "sub*/subfolder2*/*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\*", testFilename), @"subfolder1\sub*\*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\*.*", testFilename), @"subfolder1\sub*\*.*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\test*", testFilename), @"subfolder1\sub*\test*");
			Assert.IsTrue(await FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(fileStorageService, @"subfolder1\sub*\test123.txt", testFilename), @"subfolder1\sub*\test*");

			// složka samotná není nalezena
			Assert.IsFalse((await fileStorageService.EnumerateFilesAsync(@"subfolder1").ToListAsync()).Any(), @"Folder subfolder1");
			Assert.IsFalse((await fileStorageService.EnumerateFilesAsync(@"subfolder1\").ToListAsync()).Any(), @"Folder subfolder1\");
			Assert.IsFalse((await fileStorageService.EnumerateFilesAsync(@"subfolder1\subfolder2").ToListAsync()).Any(), @"Folder subfolder1\subfolder2");
			Assert.IsFalse((await fileStorageService.EnumerateFilesAsync("subfolder1/subfolder2").ToListAsync()).Any(), "Folder subfolder1/subfolder2");
			Assert.IsFalse((await fileStorageService.EnumerateFilesAsync(@"subfolder1\subfolder2\").ToListAsync()).Any(), @"Folder subfolder1\subfolder2\");

			// Clean-up
			await fileStorageService.DeleteAsync(testFilename);
		}

		public static void FileStorageService_EnumerateFiles_HasLastModifiedUtc(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = "file.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(0); // zapíšeme jeden byte
				ms.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(testFilename, ms, "text/plain");
			}

			// Act
			List<FileInfo> files = fileStorageService.EnumerateFiles(testFilename).ToList();

			// Assert
			Assert.AreNotEqual(default(DateTime), files.Single().LastModifiedUtc);

			// Clean-up
			fileStorageService.Delete(testFilename);
		}

		public static void FileStorageService_EnumerateFiles_HasSize(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = "file.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(0); // zapíšeme jeden byte
				ms.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(testFilename, ms, "text/plain");
			}

			// Act
			List<FileInfo> files = fileStorageService.EnumerateFiles(testFilename).ToList();

			// Assert
			Assert.AreEqual(1, files.Single().Size); // zapsali jsme jeden byte

			// Clean-up
			fileStorageService.Delete(testFilename);
		}

		public static async Task FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = "file.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(0); // zapíšeme jeden byte
				ms.Seek(0, SeekOrigin.Begin);

				await fileStorageService.SaveAsync(testFilename, ms, "text/plain");
			}

			// Act
			List<FileInfo> files = await fileStorageService.EnumerateFilesAsync(testFilename).ToListAsync();

			// Assert
			Assert.AreNotEqual(default(DateTime), files.Single().LastModifiedUtc);

			// Clean-up
			await fileStorageService.DeleteAsync(testFilename);
		}
		public static async Task FileStorageService_EnumerateFilesAsync_HasSize(IFileStorageService fileStorageService)
		{
			// Arrange
			string testFilename = "file.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(0); // zapíšeme jeden byte
				ms.Seek(0, SeekOrigin.Begin);

				await fileStorageService.SaveAsync(testFilename, ms, "text/plain");
			}

			// Act
			List<FileInfo> files = await fileStorageService.EnumerateFilesAsync(testFilename).ToListAsync();

			// Assert
			Assert.AreEqual(1, files.Single().Size); // zapsali jsme jeden byte

			// Clean-up
			await fileStorageService.DeleteAsync(testFilename);
		}

		public static void FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(FileStorageServiceBase fileStorageService)
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

		public static async Task FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(FileStorageServiceBase fileStorageService)
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

				await fileStorageService.SaveAsync(testFilename, ms, "text/plain");
			}

			// Act

			// přečteme jen jednu řádku, poté provedeme dispose
			using (Stream stream = await fileStorageService.ReadAsync(testFilename))
			using (StreamReader reader = new StreamReader(stream))
			{
				await reader.ReadLineAsync();
			}

			// Assert
			// no exception was thrown (vs. System.Security.Cryptography.CryptographicException: Výplň je neplatná a nelze ji odebrat.)

			// Clean-up
			await fileStorageService.DeleteAsync(testFilename);
		}

		private static bool FileStorageService_EnumerateFiles_SupportsSearchPattern_ContainsFile(IFileStorageService fileStorageService, string searchPattern, string testFilename)
		{
			IEnumerable<FileInfo> fileInfos = fileStorageService.EnumerateFiles(searchPattern);
			return fileInfos.Any(fileInfo => String.Equals(fileInfo.Name, testFilename, StringComparison.InvariantCultureIgnoreCase));
		}

		private static async Task<bool> FileStorageService_EnumerateFilesAsync_SupportsSearchPattern_ContainsFile(IFileStorageService fileStorageService, string searchPattern, string testFilename)
		{
			return (await fileStorageService.EnumerateFilesAsync(searchPattern).ToListAsync()).Any(fileInfo => String.Equals(fileInfo.Name, testFilename, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
