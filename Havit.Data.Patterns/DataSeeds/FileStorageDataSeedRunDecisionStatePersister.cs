using System.Text;
using Havit.Services.FileStorage;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Spravuje stav pro implementace DataSeedRunDecision v IFileStorageService.
/// </summary>
public class FileStorageDataSeedRunDecisionStatePersister : IDataSeedRunDecisionStatePersister
{
	private readonly IFileStorageService _fileStorageService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="fileStorageService">FileStorage použitá pro zápis stavu state persisteru.</param>
	public FileStorageDataSeedRunDecisionStatePersister(IFileStorageService fileStorageService)
	{
		this._fileStorageService = fileStorageService;
	}

	/// <summary>
	/// Přečte aktuální stav.
	/// V případě nemožnosti přečíst stav (neexistence souboru, atp.) vrací null.
	/// </summary>
	/// <returns>Aktuální stav.</returns>
	public string ReadCurrentState(string profileName)
	{
		try
		{
			using (Stream stream = _fileStorageService.OpenRead(GetFileName(profileName)))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
		catch (FileNotFoundException)
		{
			return null;
		}
	}

	/// <summary>
	/// Přečte aktuální stav.
	/// V případě nemožnosti přečíst stav (neexistence souboru, atp.) vrací null.
	/// </summary>
	/// <returns>Aktuální stav.</returns>
	public async Task<string> ReadCurrentStateAsync(string profileName, CancellationToken cancellationToken = default)
	{
		try
		{
			using (Stream stream = await _fileStorageService.OpenReadAsync(GetFileName(profileName), cancellationToken).ConfigureAwait(false))
			using (StreamReader reader = new StreamReader(stream))
			{
#if NET8_0_OR_GREATER
				return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
#else
				return await reader.ReadToEndAsync().ConfigureAwait(false);
#endif
			}
		}
		catch (FileNotFoundException)
		{
			return null;
		}
	}

	/// <summary>
	/// Zapíše aktuální stav.
	/// </summary>
	public void WriteCurrentState(string profileName, string currentState)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8, 1024, leaveOpen: true))
			{
				writer.Write(currentState);
			}
			memoryStream.Seek(0, SeekOrigin.Begin);

			_fileStorageService.Save(GetFileName(profileName), memoryStream, "text/plain");
		}
	}

	/// <summary>
	/// Zapíše aktuální stav.
	/// </summary>
	public async Task WriteCurrentStateAsync(string profileName, string currentState, CancellationToken cancellationToken = default)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8, 1024, leaveOpen: true))
			{
				await writer.WriteAsync(currentState).ConfigureAwait(false);
			}
			memoryStream.Seek(0, SeekOrigin.Begin);

			await _fileStorageService.SaveAsync(GetFileName(profileName), memoryStream, "text/plain", cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Smaže soubor se stavem. Určeno pro úklid v unit testech.
	/// </summary>
	internal void DeleteCurrentStateFile(string profileName)
	{
		_fileStorageService.Delete(GetFileName(profileName));
	}

	private string GetFileName(string profileName)
	{
		return "DataSeedState." + profileName + ".txt";
	}

}