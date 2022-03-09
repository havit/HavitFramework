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
			
			var services = new FileSystemStorageService(@"D:\Temp", useFullyQualifiedPathNames: false, encryptionOptions: null);

			var files = services.EnumerateFiles("nuget/*").ToList();
			files.ForEach(blob => Console.WriteLine(blob.Name));
		}
	}
}
