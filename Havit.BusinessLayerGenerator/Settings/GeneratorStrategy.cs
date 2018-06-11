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
		/// Generuje na základě pravidel pro databáze společnosti Exec.
		/// </summary>
		Exec,

		/// <summary>
		/// Generuje na základě pravidel pro databáze společnosti WikiReality.
		/// </summary>
		WikiReality
	}
}
