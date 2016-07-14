﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

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
		// TODO šifrování storage: protected
		internal EncryptionOptions EncryptionOptions { get; private set; }

		/// <summary>
		/// Indukuje, zda je podporováno šifrování souborů.
		/// </summary>
		protected bool SupportsBasicEncryption => EncryptionOptions != null;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected FileStorageServiceBase() : this(null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		// TODO šifrování storage: public, ev. sloužit s předchozím konstruktorem
		internal FileStorageServiceBase(EncryptionOptions encryptionOptions)
		{
			this.EncryptionOptions = encryptionOptions;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public abstract bool Exists(string fileName);

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// Pokud je zapnuto, provádí transparentní (de)šifrování.
		/// </summary>
		public Stream Read(string fileName)
		{
			if (!SupportsBasicEncryption)
			{
				return PerformRead(fileName);
			}
			else
			{
				return new CryptoStream(PerformRead(fileName), EncryptionOptions.CreateDecryptor(), CryptoStreamMode.Read);
			}
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// Pokud je zapnuto, provádí transparentní (de)šifrování, tj. do streamu jsou zapsána již dešifrovaná data.
		/// </summary>
		public void ReadToStream(string fileName, Stream stream)
		{
			if (!SupportsBasicEncryption)
			{
				PerformReadToStream(fileName, stream);
			}
			else
			{
				using (NotClosingCryptoStream decryptingStream = new NotClosingCryptoStream(stream, EncryptionOptions.CreateDecryptor(), CryptoStreamMode.Write))
				{
					PerformReadToStream(fileName, decryptingStream);
				}
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// Pokud je zapnuto, provádí transparentní šifrování, tj. streamu (fileContent) jsou zašifrována.
		/// </summary>
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			if (!SupportsBasicEncryption)
			{
				PerformSave(fileName, fileContent, contentType);
			}
			else
			{
				using (NotClosingCryptoStream encryptingStream = new NotClosingCryptoStream(fileContent, EncryptionOptions.CreateEncryptor(), CryptoStreamMode.Read))
				{			
					PerformSave(fileName, encryptingStream, contentType);
				}
			}
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public abstract void Delete(string fileName);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public abstract IEnumerable<FileInfo> EnumerateFiles(string pattern = null);

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		public abstract DateTime? GetLastModifiedTimeUtc(string fileName);

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected abstract Stream PerformRead(string fileName);

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected abstract void PerformReadToStream(string fileName, Stream stream);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected abstract void PerformSave(string fileName, Stream fileContent, string contentType);

	}
}