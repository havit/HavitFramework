namespace Havit.Services.Caching
{
	/// <summary>
	/// Konfigurace MemoryCacheService.
	/// </summary>
	public class MemoryCacheServiceOptions
	{
		/// <summary>
		/// Indikuje, zda má být použita podpora pro cache dependencies.
		/// </summary>
		public bool UseCacheDependenciesSupport { get; set; } = false;
	}
}