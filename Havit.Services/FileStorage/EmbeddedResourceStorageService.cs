using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Úložiště souborů pro práci s embedded resources. Podporuje pouze čtení z embedded resources a ověření existence embedded resource.
	/// </summary>
	public class EmbeddedResourceStorageService : FileStorageServiceBase
	{
		private readonly Assembly resourceAssembly;
		private readonly string rootNamespace;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EmbeddedResourceStorageService(Assembly resourceAssembly, string rootNamespace)
		{
			Contract.Requires<ArgumentNullException>(resourceAssembly != null, nameof(resourceAssembly));
			this.resourceAssembly = resourceAssembly;
			this.rootNamespace = rootNamespace;
		}

		/// <summary>
		/// Vrátí resource name, který bude dohledáván v assembly.
		/// </summary>
		private string GetResourceName(string fileName)
		{
			return String.IsNullOrEmpty(rootNamespace)
				? fileName
				: rootNamespace + "." + fileName;
		}

		/// <inheritdoc />
		public override bool Exists(string fileName)
		{
			return resourceAssembly.GetManifestResourceInfo(GetResourceName(fileName)) != null;
		}

		/// <inheritdoc />
		public override Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return Task.FromResult(Exists(fileName)); // no async version
		}

		/// <inheritdoc />
		protected override Stream PerformRead(string fileName)
		{
			string resourceName = GetResourceName(fileName);
			var result = resourceAssembly.GetManifestResourceStream(resourceName);
			if (result == null)
			{
				throw new FileNotFoundException($"Embedded resource {resourceName} not found in the assembly {resourceAssembly.GetName().Name}.", fileName);
			}
			return result;
		}

		/// <inheritdoc />
		protected override Task<Stream> PerformReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return Task.FromResult(Read(fileName)); // no async version
		}

		/// <inheritdoc />
		protected override void PerformReadToStream(string fileName, Stream stream)
		{
			using (Stream resourceStream = Read(fileName))
			{
				resourceStream.CopyTo(stream);
			}
		}

		/// <inheritdoc />
		protected override async Task PerformReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
		{
			using (Stream resourceStream = Read(fileName))
			{
				await resourceStream.CopyToAsync(stream, 81920 /* default */, cancellationToken).ConfigureAwait(false);
			}
		}

		#region Not supported methods
		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		public override void Delete(string fileName) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		public override Task DeleteAsync(string fileName, CancellationToken cancellationToken = default) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		public override IEnumerable<Havit.Services.FileStorage.FileInfo> EnumerateFiles(string pattern = null) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		public override IAsyncEnumerable<Havit.Services.FileStorage.FileInfo> EnumerateFilesAsync(string pattern = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		public override Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		protected override void PerformSave(string fileName, Stream fileContent, string contentType) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		protected override Task PerformSaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default) => throw new NotSupportedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		protected override string GetContentType(string fileName) => throw new NotImplementedException();

		/// <summary>
		/// Vyhazuje <see cref="NotSupportedException"/>.
		/// </summary>
		protected override ValueTask<string> GetContentTypeAsync(string fileName, CancellationToken cancellationToken) => throw new NotImplementedException();
		#endregion
	}
}
