namespace Havit.Business.BusinessLayerGenerator.Settings
{
	internal static class GeneratorSettings
	{
		#region OutputPath
		/// <summary>
		/// Cesta kam se ukládají výsledné soubory.
		/// </summary>
		public static string OutputPath;
		#endregion

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

		#region TableName
		/// <summary>
		/// Název tabulky, na jejímž základě generátor vytváří kód. Není-li uveden, generuje se pro všechny tabulky.
		/// </summary>
		public static string TableName;
		#endregion

		#region Strategy
		/// <summary>
		/// Režim ukládání nových objektů.
		/// </summary>
		public static GeneratorStrategy Strategy = GeneratorStrategy.Havit;
		#endregion

		#region TargetPlatform
		/// <summary>
		/// Cílový platformat BL.
		/// </summary>
		public static TargetPlatform TargetPlatform = TargetPlatform.SqlServer2008;
		#endregion

	}
}
