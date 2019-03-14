namespace Havit.Business.BusinessLayerGenerator.Settings
{
	public static class GeneratorSettings
	{
		/// <summary>
		/// Cesta kam se ukládají výsledné soubory.
		/// </summary>
		public static string OutputPath { get; set; }

		/// <summary>
		/// Namespace generovaných tříd.
		/// </summary>		
		public static string Namespace { get; set; }

		/// <summary>
		/// SqlServer, ke kterému se generátor připojuje.
		/// </summary>
		public static string SqlServerName { get; set; }

		/// <summary>
		/// Username k sql serveru. Není-li uvedeno, použije se integrované zabezpečení.
		/// </summary>
		public static string Username { get; set; }

		/// <summary>
		/// Heslo k sql serveru.
		/// </summary>
		public static string Password { get; set; }

		/// <summary>
		/// Název databáze, na jejímž základě generátor vytváří kód.
		/// </summary>
		public static string DatabaseName { get; set; }

		/// <summary>
		/// Název tabulky, na jejímž základě generátor vytváří kód. Není-li uveden, generuje se pro všechny tabulky.
		/// </summary>
		public static string TableName { get; set; }

		/// <summary>
		/// Režim ukládání nových objektů.
		/// </summary>
		public static GeneratorStrategy Strategy { get; set; } = GeneratorStrategy.Havit;

		/// <summary>
		/// Cílový platformat BL.
		/// </summary>
		public static TargetPlatform TargetPlatform { get; set; } = TargetPlatform.SqlServer2008;

        /// <summary>
        /// Indikuje, zda je možno použít System.Memory.Span&lt;T&gt; pro vygenerování optimalizovaného parsování dat do kolekcí.
        /// </summary>
        public static bool SystemMemorySpanSupported { get; set; } = true;

	}
}
