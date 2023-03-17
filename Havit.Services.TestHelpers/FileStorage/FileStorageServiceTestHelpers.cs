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

		public static void FileStorageService_Save_DoNotAcceptSeekedStream(IFileStorageService fileStorageService)
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

			using (Stream stream = fileStorageService.OpenRead(filename))
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

			using (Stream stream = await fileStorageService.OpenReadAsync(filename))
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

		public static void FileStorageService_Save_OverwritesTargetFile(IFileStorageService fileStorageService)
		{
			// Arrange
			string content1 = "abcdefghijklmnopqrstuvwxyz";
			string content2 = "abc"; // Musí být kratší než content!
			string filename = Guid.NewGuid().ToString();

			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					sw.Write(content1);
				}
				ms.Seek(0, SeekOrigin.Begin);

				// Act
				fileStorageService.Save(filename, ms, "text/plain");
				Assert.IsTrue(fileStorageService.Exists(filename));
				Assert.AreEqual(content1.Length + 3 /* BOM */, fileStorageService.EnumerateFiles(filename).Single().Size);
			}

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					sw.Write(content2);
				}
				ms.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(filename, ms, "text/plain");
			}

			// Assert - no exception is thrown
			Assert.AreEqual(content2.Length + 3 /* BOM */, fileStorageService.EnumerateFiles(filename).Single().Size);

			// Clean-up
			fileStorageService.Delete(filename);
		}

		public static async Task FileStorageService_SaveAsync_OverwritesTargetFile(IFileStorageService fileStorageService)
		{
			// Arrange
			string content1 = "abcdefghijklmnopqrstuvwxyz";
			string content2 = "abc"; // Musí být kratší než content!
			string filename = Guid.NewGuid().ToString();

			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					await sw.WriteAsync(content1);
				}
				ms.Seek(0, SeekOrigin.Begin);

				// Act
				await fileStorageService.SaveAsync(filename, ms, "text/plain");
				Assert.IsTrue(await fileStorageService.ExistsAsync(filename));
				Assert.AreEqual(content1.Length + 3 /* BOM */, (await fileStorageService.EnumerateFilesAsync(filename).ToListAsync()).Single().Size);
			}

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					await sw.WriteAsync(content2);
				}
				ms.Seek(0, SeekOrigin.Begin);

				await fileStorageService.SaveAsync(filename, ms, "text/plain");
			}

			// Assert - no exception is thrown
			Assert.AreEqual(content2.Length + 3 /* BOM */, (await fileStorageService.EnumerateFilesAsync(filename).ToListAsync()).Single().Size);

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

		public static void FileStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder(IFileStorageService fileStorageService)
		{
			// Arrange

			// Act
			IEnumerable<FileInfo> files = fileStorageService.EnumerateFiles("NONEXISTING_FOLDER\\*");

			// Assert
			Assert.IsNotNull(files);
			Assert.IsTrue(!files.Any());
		}

		public static async Task FileStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder(IFileStorageService fileStorageService)
		{
			// Arrange

			// Act
			List<FileInfo> files = await fileStorageService.EnumerateFilesAsync("NONEXISTING_FOLDER\\*").ToListAsync();

			// Assert
			Assert.IsTrue(!files.Any());
		}

		public static void FileStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException(FileStorageServiceBase fileStorageService)
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
			using (Stream stream = fileStorageService.OpenRead(testFilename))
			using (StreamReader reader = new StreamReader(stream))
			{
				reader.ReadLine();
			}

			// Assert
			// no exception was thrown (vs. System.Security.Cryptography.CryptographicException: Výplň je neplatná a nelze ji odebrat.)

			// Clean-up
			fileStorageService.Delete(testFilename);
		}

		public static async Task FileStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(FileStorageServiceBase fileStorageService)
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
			using (Stream stream = await fileStorageService.OpenReadAsync(testFilename))
			using (StreamReader reader = new StreamReader(stream))
			{
				await reader.ReadLineAsync();
			}

			// Assert
			// no exception was thrown (vs. System.Security.Cryptography.CryptographicException: Výplň je neplatná a nelze ji odebrat.)

			// Clean-up
			await fileStorageService.DeleteAsync(testFilename);
		}

		public static void FileStorageService_OpenWrite_OverwritesExistingFileAndContent(FileStorageServiceBase fileStorageService)
		{
			// Arrange
			string filename = "openwrite.txt";
			byte[] writeBuffer = new byte[4] { 65, 66, 67, 68 };

			using (MemoryStream ms = new MemoryStream())
			{
				byte[] buffer = new byte[100000];
				ms.Write(buffer, 0, buffer.Length);
				ms.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(filename, ms, ""); // upload file

				// preconfition
				Assert.AreEqual(buffer.Length, fileStorageService.EnumerateFiles(filename).Single().Size);
			}

			// Act
			using (var stream = fileStorageService.OpenWrite(filename, "")) // should overwrite file and the content
			{
				stream.Write(writeBuffer, 0, writeBuffer.Length);
			}

			// Assert
			Assert.AreEqual(writeBuffer.Length, fileStorageService.EnumerateFiles(filename).Single().Size);

			// Clean-up
			fileStorageService.Delete(filename);
		}

		public static async Task FileStorageService_OpenWriteAsync_OverwritesExistingFileAndContent(FileStorageServiceBase fileStorageService)
		{
			// Arrange
			string filename = "openwrite.txt";
			byte[] writeBuffer = new byte[4] { 65, 66, 67, 68 };

			using (MemoryStream ms = new MemoryStream())
			{
				byte[] buffer = new byte[100000];
				ms.Write(buffer, 0, buffer.Length);
				ms.Seek(0, SeekOrigin.Begin);

				await fileStorageService.SaveAsync(filename, ms, ""); // upload file

				// precondition
				Assert.AreEqual(buffer.Length, (await fileStorageService.EnumerateFilesAsync(filename).ToListAsync()).Single().Size);

			}

			// Act
			using (var stream = await fileStorageService.OpenWriteAsync(filename, "")) // should overwrite file and the content
			{
				await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
			}

			// Assert
			Assert.AreEqual(writeBuffer.Length, (await fileStorageService.EnumerateFilesAsync(filename).ToListAsync()).Single().Size);

			// Clean-up
			fileStorageService.Delete(filename);
		}

		public static void FileStorageService_OpenWriteAndOpenRead_ContentsAreSame(FileStorageServiceBase fileStorageService)
		{
			// Arrange
			string filename = "content.txt";
			byte[] writeBuffer = new byte[4] { 65, 66, 67, 68 };
			byte[] readBuffer = new byte[1000]; // přečteme alespoň o jeden znak více, pokud by byla chyba, než kolik očekáváme

			// Act
			using (var stream = fileStorageService.OpenWrite(filename, "")) // should overwrite file and the content
			{
				stream.Write(writeBuffer, 0, writeBuffer.Length);
			}

			// Assert
			using (var stream = fileStorageService.OpenRead(filename))
			{
				var readBytes = stream.Read(readBuffer, 0, readBuffer.Length);

				Assert.AreEqual(writeBuffer.Length, readBytes);
				CollectionAssert.AreEquivalent(writeBuffer, readBuffer.Take(readBytes).ToArray() /* readBuffer je záměrně větší než write buffer, ale porovnat chceme jen počet přečtených bytes */); // assert
			}

			// Clean-up
			fileStorageService.Delete(filename);
		}

		public static async Task FileStorageService_OpenWriteAsyncAndOpenReadAsync_ContentsAreSame(FileStorageServiceBase fileStorageService)
		{
			// Arrange
			string filename = "content.txt";
			byte[] writeBuffer = new byte[4] { 65, 66, 67, 68 };
			byte[] readBuffer = new byte[1000]; // přečteme alespoň o jeden znak více, pokud by byla chyba, než kolik očekáváme

			// Act
			using (var stream = await fileStorageService.OpenWriteAsync(filename, "")) // should overwrite file and the content
			{
				await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
			}

			// Assert
			using (var stream = await fileStorageService.OpenReadAsync(filename))
			{
				var readBytes = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);

				Assert.AreEqual(writeBuffer.Length, readBytes);
				CollectionAssert.AreEquivalent(writeBuffer, readBuffer.Take(readBytes).ToArray() /* readBuffer je záměrně větší než write buffer, ale porovnat chceme jen počet přečtených bytes */); // assert
			}

			// Clean-up
			await fileStorageService.DeleteAsync(filename);
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

		public static void FileStorageService_Copy(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = @"folder_copy\file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService1.Save(sourceFilename, stream, "text/plain");
			}

			Assert.IsTrue(fileStorageService1.Exists(sourceFilename));
			Assert.IsFalse(fileStorageService2.Exists(targetFilename));

			// Act
			fileStorageService1.Copy(sourceFilename, fileStorageService2, targetFilename);

			// Assert
			Assert.IsTrue(fileStorageService1.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService2.Exists(targetFilename));

			// Clean up
			fileStorageService1.Delete(sourceFilename);
			fileStorageService2.Delete(targetFilename);
		}

		public static async Task FileStorageService_CopyAsync(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = @"folder_copyasync\file2.txt";

			using (Stream stream = new MemoryStream())
			{
				await fileStorageService1.SaveAsync(sourceFilename, stream, "text/plain");
			}
			Assert.IsTrue(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsFalse(await fileStorageService2.ExistsAsync(targetFilename));

			// Act
			await fileStorageService1.CopyAsync(sourceFilename, fileStorageService2, targetFilename);

			// Assert
			Assert.IsTrue(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsTrue(await fileStorageService2.ExistsAsync(targetFilename));

			// Clean up
			await fileStorageService1.DeleteAsync(sourceFilename);
			await fileStorageService2.DeleteAsync(targetFilename);
		}

		public static void FileStorageService_Copy_OverwritesTargetFile(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = "file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService1.Save(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(fileStorageService1.Exists(sourceFilename));
			Assert.IsFalse(fileStorageService2.Exists(targetFilename));

			// Act
			fileStorageService1.Copy(sourceFilename, fileStorageService2, targetFilename);
			Assert.IsTrue(fileStorageService2.Exists(targetFilename));
			fileStorageService1.Copy(sourceFilename, fileStorageService2, targetFilename);
			Assert.IsTrue(fileStorageService2.Exists(targetFilename));

			// Assert - does not throw exception

			// Clean up
			fileStorageService1.Delete(sourceFilename);
			fileStorageService2.Delete(targetFilename);
		}

		public static async Task FileStorageService_CopyAsync_OverwritesTargetFile(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = "file2.txt";

			using (Stream stream = new MemoryStream())
			{
				await fileStorageService1.SaveAsync(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsFalse(await fileStorageService2.ExistsAsync(targetFilename));

			// Act
			await fileStorageService1.CopyAsync(sourceFilename, fileStorageService2, targetFilename);
			Assert.IsTrue(await fileStorageService2.ExistsAsync(targetFilename));
			await fileStorageService1.CopyAsync(sourceFilename, fileStorageService2, targetFilename);
			Assert.IsTrue(await fileStorageService2.ExistsAsync(targetFilename));

			// Assert - does not throw exception

			// Clean up
			await fileStorageService1.DeleteAsync(sourceFilename);
			await fileStorageService2.DeleteAsync(targetFilename);
		}

		public static void FileStorageService_Move(IFileStorageService fileStorageService)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = @"folder_move\file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
			}

			// Preconditions
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));
			Assert.IsFalse(fileStorageService.Exists(targetFilename));

			// Act
			fileStorageService.Move(sourceFilename, targetFilename);

			// Assert
			Assert.IsFalse(fileStorageService.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService.Exists(targetFilename));

			// Clean up
			fileStorageService.Delete(targetFilename);
		}

		public static async Task FileStorageService_MoveAsync(IFileStorageService fileStorageService)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = @"folder_move\file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
			}

			// Preconditions
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));
			Assert.IsFalse(fileStorageService.Exists(targetFilename));

			// Act
			await fileStorageService.MoveAsync(sourceFilename, targetFilename);

			// Assert
			Assert.IsFalse(fileStorageService.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService.Exists(targetFilename));

			// Clean up
			await fileStorageService.DeleteAsync(targetFilename);
		}

		public static void FileStorageService_Move_OverwritesTargetFile(IFileStorageService fileStorageService)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = "file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
				fileStorageService.Move(sourceFilename, fileStorageService, targetFilename);
			}

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService.Exists(targetFilename));

			// Act
			fileStorageService.Move(sourceFilename, targetFilename);

			// Assert
			Assert.IsFalse(fileStorageService.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService.Exists(targetFilename));

			// Clean up
			fileStorageService.Delete(targetFilename);
		}

		public static async Task FileStorageService_MoveAsync_OverwritesTargetFile(IFileStorageService fileStorageService)
		{
			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = "file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
				fileStorageService.Move(sourceFilename, fileStorageService, targetFilename);
			}

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService.Exists(targetFilename));

			// Act
			await fileStorageService.MoveAsync(sourceFilename, targetFilename);

			// Assert
			Assert.IsFalse(fileStorageService.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService.Exists(targetFilename));

			// Clean up
			fileStorageService.Delete(targetFilename);
		}

		public static void FileStorageService_Move_DoesNotDeleteFile(IFileStorageService fileStorageService)
		{
			// Arrange
			string sourceFilename = "file1.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));

			// Act
			fileStorageService.Move(sourceFilename, sourceFilename);

			// Assert
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));

			// Clean up
			fileStorageService.Delete(sourceFilename);
		}

		public static async Task FileStorageService_MoveAsync_DoesNotDeleteFile(IFileStorageService fileStorageService)
		{
			// Arrange
			string sourceFilename = "file1.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService.Save(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));

			// Act
			await fileStorageService.MoveAsync(sourceFilename, sourceFilename);

			// Assert
			Assert.IsTrue(fileStorageService.Exists(sourceFilename));

			// Clean up
			fileStorageService.Delete(sourceFilename);
		}

		public static void FileStorageService_Move_WithFileStorageService(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			Contract.Requires(fileStorageService1 != fileStorageService2);

			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = @"folder_move\file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService1.Save(sourceFilename, stream, "text/plain");
			}
			Assert.IsTrue(fileStorageService1.Exists(sourceFilename));
			Assert.IsFalse(fileStorageService2.Exists(targetFilename));

			// Act
			fileStorageService1.Move(sourceFilename, fileStorageService2, targetFilename);

			// Assert
			Assert.IsFalse(fileStorageService1.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService2.Exists(targetFilename));

			// Clean up
			fileStorageService2.Delete(targetFilename);
		}

		public static async Task FileStorageService_MoveAsync_WithFileStorageService(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			Contract.Requires(fileStorageService1 != fileStorageService2);

			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = @"folder_moveasync\file2.txt";

			using (Stream stream = new MemoryStream())
			{
				await fileStorageService1.SaveAsync(sourceFilename, stream, "text/plain");
			}
			Assert.IsTrue(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsFalse(await fileStorageService2.ExistsAsync(targetFilename));

			// Act
			await fileStorageService1.MoveAsync(sourceFilename, fileStorageService2, targetFilename);

			// Assert
			Assert.IsFalse(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsTrue(await fileStorageService2.ExistsAsync(targetFilename));

			// Clean up
			await fileStorageService2.DeleteAsync(targetFilename);
		}

		public static void FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			Contract.Requires(fileStorageService1 != fileStorageService2);

			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = "file2.txt";

			using (Stream stream = new MemoryStream())
			{
				fileStorageService1.Save(sourceFilename, stream, "text/plain");
				fileStorageService1.Move(sourceFilename, fileStorageService2, targetFilename);
			}

			using (Stream stream = new MemoryStream())
			{
				fileStorageService1.Save(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(fileStorageService1.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService2.Exists(targetFilename));

			// Act
			fileStorageService1.Move(sourceFilename, fileStorageService2, targetFilename);

			// Assert
			Assert.IsFalse(fileStorageService1.Exists(sourceFilename));
			Assert.IsTrue(fileStorageService2.Exists(targetFilename));

			// Clean up
			fileStorageService2.Delete(targetFilename);
		}

		public static async Task FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(IFileStorageService fileStorageService1, IFileStorageService fileStorageService2)
		{
			Contract.Requires(fileStorageService1 != fileStorageService2);

			// Arrange
			string sourceFilename = "file1.txt";
			string targetFilename = "file2.txt";

			using (Stream stream = new MemoryStream())
			{
				await fileStorageService1.SaveAsync(sourceFilename, stream, "text/plain");
				await fileStorageService1.MoveAsync(sourceFilename, fileStorageService2, targetFilename);
			}

			using (Stream stream = new MemoryStream())
			{
				await fileStorageService1.SaveAsync(sourceFilename, stream, "text/plain");
			}

			// Precondition
			Assert.IsTrue(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsTrue(await fileStorageService2.ExistsAsync(targetFilename));

			// Act
			await fileStorageService1.MoveAsync(sourceFilename, fileStorageService2, targetFilename);

			// Assert
			Assert.IsFalse(await fileStorageService1.ExistsAsync(sourceFilename));
			Assert.IsTrue(await fileStorageService2.ExistsAsync(targetFilename));

			// Clean up
			await fileStorageService2.DeleteAsync(targetFilename);
		}
	}
}
