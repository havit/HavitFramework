namespace Havit.Services.Caching
{
	/// <summary>
	/// Konfigurace ObjectCacheService.
	/// </summary>
	public class ObjectCacheServiceOptions
	{
		/// <summary>
		/// Indikuje, zda má být použita podpora pro cache dependencies.
		/// </summary>
		public bool UseCacheDependenciesSupport { get; set; } = false;
	}
}