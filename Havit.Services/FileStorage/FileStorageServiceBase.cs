using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage.Infrastructure;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Abstraktní předek pro implementaci úložiště souborů. 
	/// Podporuje šifrování. Šifrování souborů je transparentní operací při čtení a zápisu.
	/// </summary>
	public abstract class FileStorageServiceBase : IFileStorageService
	{
		/// <summary>
		/// Nastavení šifrování.
		/// </summary>
		protected EncryptionOptions EncryptionOptions { get; private set; }

		/// <summary>
		/// Indukuje, zda je podporováno šifrování souborů.
		/// </summary>
		public bool SupportsBasicEncryption => EncryptionOptions != null;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected FileStorageServiceBase() : this(null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected FileStorageServiceBase(EncryptionOptions encryptionOptions)
		{
			this.EncryptionOptions = encryptionOptions;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public abstract bool Exists(string fileName);

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public abstract Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// Pokud je zapnuto, provádí transparentní (de)šifrování.
		/// </summary>
		public Stream Read(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			Stream s = PerformRead(fileName);
			return Read_EnsureDecryption(s);
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// Pokud je zapnuto, provádí transparentní (de)šifrování.
		/// </summary>
		public async Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			Stream s = await PerformReadAsync(fileName, cancellationToken).ConfigureAwait(false);
			return Read_EnsureDecryption(s);
		}

		private Stream Read_EnsureDecryption(Stream stream)
		{
			if (!SupportsBasicEncryption)
			{
				return stream;
			}
			else
			{
				return new InternalCryptoStream(stream, new InternalCryptoTransform(EncryptionOptions.CreateDecryptor()), CryptoStreamMode.Read);
			}
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// Pokud je zapnuto, provádí transparentní (de)šifrování, tj. do streamu jsou zapsána již dešifrovaná data.
		/// </summary>
		public void ReadToStream(string fileName, Stream stream)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			if (!SupportsBasicEncryption)
			{
				PerformReadToStream(fileName, stream);
			}
			else
			{
				// použití NonClosingWrappingStreamu testuje Havit.Services.Tests.FileStorage.FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform
				using (Stream notClosingWrappingStream = new NonClosingWrappingStream(stream))
				using (CryptoStream decryptingStream = new CryptoStream(notClosingWrappingStream, EncryptionOptions.CreateDecryptor(), CryptoStreamMode.Write))
				{
					PerformReadToStream(fileName, decryptingStream);
				}
			}
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// Pokud je zapnuto, provádí transparentní (de)šifrování, tj. do streamu jsou zapsána již dešifrovaná data.
		/// </summary>
		public async Task ReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			if (!SupportsBasicEncryption)
			{
				await PerformReadToStreamAsync(fileName, stream, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// použití NonClosingWrappingStreamu testuje Havit.Services.Tests.FileStorage.FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform
				using (Stream notClosingWrappingStream = new NonClosingWrappingStream(stream))
				using (CryptoStream decryptingStream = new InternalCryptoStream(notClosingWrappingStream, new InternalCryptoTransform(EncryptionOptions.CreateDecryptor()), CryptoStreamMode.Write))
				{
					await PerformReadToStreamAsync(fileName, decryptingStream, cancellationToken).ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// Pokud je zapnuto, provádí transparentní šifrování, tj. streamu (fileContent) jsou zašifrována.
		/// </summary>
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));
			Contract.Requires<InvalidOperationException>(!fileContent.CanSeek || fileContent.Position == 0, "Actual position in the stream is not at the beginning.");

			if (!SupportsBasicEncryption)
			{
				PerformSave(fileName, fileContent, contentType);
			}
			else
			{
				using (CryptoStream encryptingStream = new CryptoStream(fileContent, EncryptionOptions.CreateEncryptor(), CryptoStreamMode.Read))
				{			
					PerformSave(fileName, encryptingStream, contentType);
				}
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// Pokud je zapnuto, provádí transparentní šifrování, tj. streamu (fileContent) jsou zašifrována.
		/// </summary>
		public async Task SaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));
			Contract.Requires<InvalidOperationException>(!fileContent.CanSeek || fileContent.Position == 0, "Actual position in the stream is not at the beginning.");

			if (!SupportsBasicEncryption)
			{
				await PerformSaveAsync(fileName, fileContent, contentType, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				using (CryptoStream encryptingStream = new CryptoStream(fileContent, EncryptionOptions.CreateEncryptor(), CryptoStreamMode.Read))
				{
					await PerformSaveAsync(fileName, encryptingStream, contentType, cancellationToken).ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// Vrátí prefix pro vyhledání.
		/// Prefix je úvodní část cesty po poslední '/', která neobsahuje '*' a '?'.
		/// </summary>
		static protected internal string EnumerableFilesGetPrefix(string searchPattern)
		{
			if ((searchPattern != null) && searchPattern.Contains('/'))
			{
				// prvni vyskyt '*' nebo '?'
				int firstIndexOfSearchToken = searchPattern.IndexOfAny(new char[] { '?', '*' });
				if (firstIndexOfSearchToken == -1)
				{
					// neni zadny vyskyt '*' nebo '?', vrat vse do posledniho '/'
					return searchPattern.Remove(searchPattern.LastIndexOf("/"));
				}

				// vrat posledni '/' pred vyskytem '*' nebo '?'
				int lastIndexOfDelimiter = searchPattern.Remove(firstIndexOfSearchToken).LastIndexOf("/");
				if (lastIndexOfDelimiter == -1)
				{
					// pred vyskytem '*' nebo '?' neexistuje zadny '/', vrat null, žádný prefix není
					return null;
				}

				// vrat cast retezce do posledniho vyskytu '/' pred vyskytem '*' nebo '?'
				return searchPattern.Substring(0, lastIndexOfDelimiter);
			}
			else
			{
				// pattern neobsahuje zadny znak '/', tudiz neni zadny prefix pro vyhledavani
				return null;
			}
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public abstract void Delete(string fileName);

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public abstract Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public abstract IEnumerable<FileInfo> EnumerateFiles(string pattern = null);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public abstract IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string pattern = null, CancellationToken cancellationToken = default);

			/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public abstract DateTime? GetLastModifiedTimeUtc(string fileName);

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public abstract Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected abstract Stream PerformRead(string fileName);

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected abstract Task<Stream> PerformReadAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected abstract void PerformReadToStream(string fileName, Stream stream);

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected abstract Task PerformReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected abstract void PerformSave(string fileName, Stream fileContent, string contentType);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected abstract Task PerformSaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vrátí content type k souboru.
		/// </summary>
		protected abstract string GetContentType(string fileName);

		/// <summary>
		/// Vrátí content type k souboru.
		/// </summary>
		protected abstract ValueTask<string> GetContentTypeAsync(string fileName, CancellationToken cancellationToken);

		/// <summary>
		/// Zkopíruje soubor do dalšího úložiště.
		/// </summary>
		public virtual void Copy(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			if (targetFileStorageService is IFileStorageWrappingService fileStorageWrappingService)
			{
				Copy(sourceFileName, fileStorageWrappingService.GetWrappedFileStorageService(), targetFileName);
				return;
			}
			
			using (var stream = Read(sourceFileName))
			{
				targetFileStorageService.Save(targetFileName, stream, GetContentType(sourceFileName));
			}
		}

		/// <summary>
		/// Zkopíruje soubor do dalšího úložiště.
		/// </summary>
		public virtual async Task CopyAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			if (targetFileStorageService is IFileStorageWrappingService fileStorageWrappingService)
			{
				await CopyAsync(sourceFileName, fileStorageWrappingService.GetWrappedFileStorageService(), targetFileName, cancellationToken).ConfigureAwait(false);
				return;
			}

			using (var stream = await ReadAsync(sourceFileName, cancellationToken).ConfigureAwait(false))
			{
				string contentType = await GetContentTypeAsync(sourceFileName, cancellationToken).ConfigureAwait(false);
				await targetFileStorageService.SaveAsync(sourceFileName, stream, contentType, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Přesune soubor do jiného úložiště.
		/// </summary>
		public virtual void Move(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			if (targetFileStorageService is IFileStorageWrappingService fileStorageWrappingService)
			{
				Move(sourceFileName, fileStorageWrappingService.GetWrappedFileStorageService(), targetFileName);
				return;
			}

			Copy(sourceFileName, targetFileStorageService, targetFileName);
			Delete(sourceFileName);
		}

		/// <summary>
		/// Přesune soubor do jiného úložiště.
		/// </summary>
		public virtual async Task MoveAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			if (targetFileStorageService is IFileStorageWrappingService fileStorageWrappingService)
			{
				await MoveAsync(sourceFileName, fileStorageWrappingService.GetWrappedFileStorageService(), targetFileName, cancellationToken).ConfigureAwait(false);
				return;
			}

			await CopyAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
			await DeleteAsync(sourceFileName, cancellationToken).ConfigureAwait(false);
		}
	}
}
