namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;

internal static class GeneratorSettings
{
	/// <summary>
	/// Namespace generovaných tříd.
	/// </summary>		
	public static string Namespace;

	/// <summary>
	/// SqlServer, ke kterému se generátor připojuje.
	/// </summary>
	public static string SqlServerName;

	/// <summary>
	/// Username k sql serveru. Není-li uvedeno, použije se integrované zabezpečení.
	/// </summary>
	public static string Username;

	/// <summary>
	/// Heslo k sql serveru.
	/// </summary>
	public static string Password;

	/// <summary>
	/// Název databáze, na jejímž základě generátor vytváří kód.
	/// </summary>
	public static string DatabaseName;

	/// <summary>
	/// Cesta ke složce s aplikací.
	/// </summary>
	public static string SolutionPath;
}
