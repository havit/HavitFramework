namespace Havit.Business.BusinessLayerGenerator.Settings;

/// <summary>
/// Strategie generování business vrstvy.
/// </summary>
public enum GeneratorStrategy
{
	/// <summary>
	/// Generuje na základě pravidel pro databáze Havit.
	/// </summary>
	Havit = 1,

	/// <summary>
	/// Generuje na základě pravidel pro databáze Havit pro databáze udržované code migration.
	/// </summary>
	HavitCodeFirst = 2,

	/// <summary>
	/// Generuje na základě pravidel pro databáze s konvencemi Entity Frameworku.
	/// </summary>
	HavitEFCoreCodeFirst = 3,

	/// <summary>
	/// Generuje na základě pravidel pro databáze společnosti Exec.
	/// </summary>
	Exec = 999,
}
