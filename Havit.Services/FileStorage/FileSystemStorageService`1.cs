using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Havit.Text.RegularExpressions;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// IFileStorageService a IFileStorageServiceAsync s file systémem pro datové úložiště.
	/// Některé asynchronní metody pod pokličkou nejsou asynchronní, viz dokumentace jednotlivých metod (jejichž název končí Async).
	/// </summary>
	public class FileSystemStorageService<TFileStorageContext> : FileSystemStorageService, IFileStorageService<TFileStorageContext>
		where TFileStorageContext : FileStorageContext
	{
		public FileSystemStorageService(FileSystemStorageServiceOptions<TFileStorageContext> options) : base(options)
		{
			// NOOP
		}
	}
}
