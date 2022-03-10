using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Havit.Services.Azure.FileStorage;
using Havit.Services.Azure.Tests.FileStorage.Infrastructure;
using Havit.Services.FileStorage;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace TestConsoleApp
{
	public class Program
	{
		public static void Main()
		{
			
			var service = new FileSystemStorageService(@"D:\Temp", useFullyQualifiedPathNames: false, encryptionOptions: null);
			var files = service.EnumerateFiles("nug*").ToList();
			files.ForEach(blob => Console.WriteLine(blob.Name));

			Console.WriteLine();

			var service2 = new FileSystemStorageService(null, useFullyQualifiedPathNames: true, encryptionOptions: null);
			var files2 = service2.EnumerateFiles(@"d:\temp\nug*").ToList();
			files2.ForEach(blob => Console.WriteLine(blob.Name));

		}
	}
}
