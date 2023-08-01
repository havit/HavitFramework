using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace Havit.Services.Azure.FileStorage.Internal;

internal class AzureFileStorageGrowingFileSizeStream : Stream
{
	private readonly Stream underlyingStream;
	private readonly ShareFileClient shareFileClient;

	private long currentFileSize;
	private long bytesWritten;
	private bool exceptionOccured = false;

	#region Stream properties
	public override bool CanRead => false;
	public override bool CanSeek => false;
	public override bool CanWrite => underlyingStream.CanWrite;
	public override long Length => throw new NotSupportedException();
	public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
	public override bool CanTimeout => underlyingStream.CanTimeout;
	public override int ReadTimeout { get => underlyingStream.ReadTimeout; set => underlyingStream.ReadTimeout = value; }
	public override int WriteTimeout { get => underlyingStream.WriteTimeout; set => underlyingStream.WriteTimeout = value; }
	#endregion

	public AzureFileStorageGrowingFileSizeStream(long initialFileSize, ShareFileClient shareFileClient)
	{
		this.shareFileClient = shareFileClient;

		underlyingStream = shareFileClient.OpenWrite(true, 0, new ShareFileOpenWriteOptions
		{
			MaxSize = initialFileSize
		});
		currentFileSize = initialFileSize;
	}

	#region Not supported methods
	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => throw new NotSupportedException();
	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => throw new NotSupportedException();
	public override int EndRead(IAsyncResult asyncResult) => throw new NotSupportedException();
	public override void EndWrite(IAsyncResult asyncResult) => throw new NotSupportedException();
	public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException();
	public override int ReadByte() => throw new NotSupportedException();
	public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
	public override void SetLength(long value) => throw new NotSupportedException();
	#endregion

	public override void Flush()
	{
		underlyingStream.Flush();
	}

	public override async Task FlushAsync(CancellationToken cancellationToken)
	{
		await underlyingStream.FlushAsync(cancellationToken).ConfigureAwait(false);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		// pokud při zápisu došlo k výjimce, nedovolíme pokračovat v zápisu
		VerifyExceptionNotOccured();

		try
		{
			if (NeedsFileToBeResized(bytesWritten + count, currentFileSize, out var newFileSize))
			{
				// nastavíme novou velikost souboru (jsme v try/catch, pokud se nepodaří, neumožníme další zápis)
				shareFileClient.SetHttpHeaders(newSize: newFileSize);
				currentFileSize = newFileSize;
			}

			underlyingStream.Write(buffer, offset, count);

			// Navyšujeme počet zapsaných bytes až po provedení Write,
			// protože chceme v Close nastavit velikost souboru na skutečně zapsané bytes (i v případě výjimky ve underlyingStream.Write).
			bytesWritten += count;
		}
		catch
		{
			exceptionOccured = true;
			throw;
		}
	}

	public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		// pokud při zápisu došlo k výjimce, nedovolíme pokračovat v zápisu
		VerifyExceptionNotOccured();

		try
		{
			if (NeedsFileToBeResized(bytesWritten + count, currentFileSize, out var newFileSize))
			{
				// nastavíme novou velikost souboru (jsme v try/catch, pokud se nepodaří, neumožníme další zápis)
				await shareFileClient.SetHttpHeadersAsync(newSize: newFileSize, cancellationToken: cancellationToken).ConfigureAwait(false);
				currentFileSize = newFileSize;
			}

			await underlyingStream.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);

			// Navyšujeme počet zapsaných bytes až po provedení Write,
			// protože chceme v Close nastavit velikost souboru na skutečně zapsané bytes (i v případě výjimky ve underlyingStream.Write).
			bytesWritten += count;
		}
		catch
		{
			exceptionOccured = true;
			throw;
		}
	}

	//public override void WriteByte(byte value) - netřeba overridovat, pod pokličkou volá Write(...)

	public override void Close()
	{
		underlyingStream.Close();

		// Pokud aktuální velikost souboru nesouhlasí s počtem zapsaných bytes (což, pokud neznáme cílovou velikost souboru obvykle nikdy nesouhlasí),
		// nastavíme souboru cílovou velikost. Cílovou velikost nastavujeme bez ohledu na to, zda došlo při zápisu k výjimce v underlying streamu
		// (nenecháme tak ve File Storagee větší soubor, než kolik bylo úspěšně zapsáno.
		if (currentFileSize != bytesWritten)
		{
			// nastavíme správnou velikost souboru podle počtu zapsaných bytes
			shareFileClient.SetHttpHeaders(newSize: bytesWritten);
		}

		base.Close();
	}

	/// <summary>
	/// Vrací true, pokud je vyžadována změna velikosti souboru.
	/// </summary>
	/// <param name="fileSizeAfterWritingBuffer">Aktuální potřebná velikost souboru (počet již zapsaných bytes + počet bytes k zápisu).</param>
	/// <param name="currentFileSize">Aktuální velikost souboru.</param>
	/// <param name="newFileSize">Nová velikost souboru (pokud nemá být soubor resizován, vrací aktuální velikost souboru).</param>
	/// <returns>True, pokud je třeba provést resize souboru.</returns>
	private bool NeedsFileToBeResized(long fileSizeAfterWritingBuffer, long currentFileSize, out long newFileSize)
	{
		newFileSize = currentFileSize;
		while (fileSizeAfterWritingBuffer > newFileSize) // je požadován zápis za současný konec souboru (while: pokud bychom zapisovali extrémní počet bytes, nemusí stačit jedno +=.
		{
			newFileSize += (newFileSize == 0)
				? 1 * 1024 * 1024 /* 1MB */
				: Math.Min(64 * 1024 * 1024 /* 64 MB */, newFileSize * 3); // soubor zvětšujem na čtyřnásobek (přidáním trojnásobku), avšak maximálně o 64 MB
		}

		return currentFileSize != newFileSize;
	}

	private void VerifyExceptionNotOccured()
	{
		if (exceptionOccured)
		{
			throw new InvalidOperationException("Cannot continue writing on a stream after an exception occured.");
		}
	}

	protected override void Dispose(bool disposing)
	{
		// try/finally - see https://github.com/dotnet/corert/blob/c6af4cfc8b625851b91823d9be746c4f7abdc667/src/System.Private.CoreLib/shared/System/IO/Stream.cs#L1257
		try
		{
			if (disposing)
			{
				underlyingStream.Dispose();
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}
}
