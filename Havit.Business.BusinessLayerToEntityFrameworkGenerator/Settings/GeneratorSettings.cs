namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings
{
	internal static class GeneratorSettings
	{
		#region Namespace
		/// <summary>
		/// Namespace generovaných tříd.
		/// </summary>		
		public static string Namespace;
		#endregion

		#region SqlServerName
		/// <summary>
		/// SqlServer, ke kterému se generátor připojuje.
		/// </summary>
		public static string SqlServerName;
		#endregion

		#region Username
		/// <summary>
		/// Username k sql serveru. Není-li uvedeno, použije se integrované zabezpečení.
		/// </summary>
		public static string Username;
		#endregion

		#region Password
		/// <summary>
		/// Heslo k sql serveru.
		/// </summary>
		public static string Password;
		#endregion

		#region DatabaseName
		/// <summary>
		/// Název databáze, na jejímž základě generátor vytváří kód.
		/// </summary>
		public static string DatabaseName;
		#endregion

		#region SolutionPath
		/// <summary>
		/// Cesta ke složce s aplikací.
		/// </summary>
		public static string SolutionPath;
		#endregion
	}
}
