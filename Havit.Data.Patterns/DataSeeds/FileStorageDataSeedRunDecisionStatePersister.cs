using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Havit.Services.FileStorage;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Spravuje stav pro implementace DataSeedRunDecision v IFileStorageService.
	/// </summary>
	public class FileStorageDataSeedRunDecisionStatePersister : IDataSeedRunDecisionStatePersister
	{
		private readonly IFileStorageService fileStorageService;

		private readonly string filename;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="fileStorageService">FileStorage použitá pro zápis stavu state persisteru.</param>
		/// <param name="filename">Název souboru.</param>
		public FileStorageDataSeedRunDecisionStatePersister(IFileStorageService fileStorageService, string filename = "DataSeedState.txt")
		{
			this.fileStorageService = fileStorageService;
			this.filename = filename;
		}

		/// <summary>
		/// Přečte aktuální stav.
		/// V případě nemožnosti přečíst stav (neexistence souboru, atp.) vrací null.
		/// </summary>
		/// <returns>Aktuální stav.</returns>
		public string ReadCurrentState()
		{
			try
			{
				using (Stream stream = fileStorageService.Read(filename))
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
		/// <param name="currentState">Aktuální stav k zapsání.</param>
		public void WriteCurrentState(string currentState)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
				{
					writer.Write(currentState);
				}
				memoryStream.Seek(0, SeekOrigin.Begin);

				fileStorageService.Save(filename, memoryStream, "text/plain");
			}
		}
	
	}
}