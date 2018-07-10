namespace Havit.Business.BusinessLayerGenerator.Settings
{
	/// <summary>
	/// Strategie generování business vrstvy.
	/// </summary>
	public enum GeneratorStrategy
	{
		/// <summary>
		/// Generuje na základě pravidel pro databáze Havit.
		/// </summary>
		Havit,

		/// <summary>
		/// Generuje na základě pravidel pro databáze Havit pro databáze udržované code migration.
		/// </summary>
		HavitCodeFirst,

		/// <summary>
		/// Generuje na základě pravidel pro databáze společnosti Exec.
		/// </summary>
		Exec,
	}
}
