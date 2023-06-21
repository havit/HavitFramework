using System.IO;
using System.Text;
using Havit.Services.FileStorage;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Spravuje stav pro implementace DataSeedRunDecision v IFileStorageService.
/// </summary>
public class FileStorageDataSeedRunDecisionStatePersister : IDataSeedRunDecisionStatePersister
{
	private readonly IFileStorageService fileStorageService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="fileStorageService">FileStorage použitá pro zápis stavu state persisteru.</param>
	public FileStorageDataSeedRunDecisionStatePersister(IFileStorageService fileStorageService)
	{
		this.fileStorageService = fileStorageService;
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
			using (Stream stream = fileStorageService.OpenRead(GetFileName(profileName)))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
		catch
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
			using (StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
			{
				writer.Write(currentState);
			}
			memoryStream.Seek(0, SeekOrigin.Begin);

			fileStorageService.Save(GetFileName(profileName), memoryStream, "text/plain");
		}
	}

	/// <summary>
	/// Smaže soubor se stavem. Určeno pro úklid v unit testech.
	/// </summary>
	internal void DeleteCurrentStateFile(string profileName)
        {
		fileStorageService.Delete(GetFileName(profileName));
	}

    private string GetFileName(string profileName)
    {
        return "DataSeedState." + profileName + ".txt";
    }

}