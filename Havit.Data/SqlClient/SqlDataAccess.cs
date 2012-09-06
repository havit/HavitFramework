using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace Havit.Data.SqlClient
{
	/// <summary>
	/// Třída SqlDataAccess obsahuje metody usnadňující práci se základní třídou
	/// <b>System.Data.</b><see cref="System.Data.SqlClient"/>.
	/// </summary>
	/// <remarks>
	/// Inspirováno <a href="http://msdn.microsoft.com/vcsharp/downloads/components/default.aspx?pull=/library/en-us/dnbda/html/daab-rm.asp">
	/// Microsoft Data Access Application Block verze 2.0</a>.
	/// </remarks>
	[Obsolete("Místo třídy SqlDataAccess použijte Havit.Data.DbConnector.")]
	public static class SqlDataAccess
	{
		#region ConnectionString, ConfigConnectionString
		/// <summary>
		/// Implicitní ConnectionString.
		/// Použije se vždy, není-li metodě jakkoliv explicitně předán jiný.
		/// Není-li nastaven klientskou aplikací, načte se z .config souboru.
		/// </summary>
		/// <remarks>
		/// Pokus o načtení z .config souboru je proveden vždy, je-li právě nastaven
		/// na <c>null</c> nebo <c>String.Empty</c>. Jeho nastavením na <c>null</c> lze tedy
		/// poručit, aby se ConnectionString načetl z .config souboru.
		/// </remarks>
		public static string ConnectionString
		{
			get
			{
				return DbConnector.Default.ConnectionString;
			}
		}


		/// <summary>
		/// ConnectionString z .config souboru (web.config nebo applikace.exe.config).
		/// </summary>
		/// <remarks>
		/// Pro .NET Framework 2.0 rozšířeno o použití hodnoty <b>DefaultConnectionString</b>
		/// z konfigurační sekce &lt;connectionStrings&gt;.
		/// </remarks>
		/// <example>
		/// Buď <b>DefaultConnectionString</b> z konfigurační sekce &lt;connectionStrings&gt;:
		/// <code>
		/// &lt;configuration&gt;
		///		&lt;connectionStrings&gt;
		///			&lt;add name="DefaultConnectionString"
		///				connectionString="Server=localhost;Database=pubs;UID=user;PWD=heslo;"
		///				providerName="System.Data.SqlClient"
		///			/&gt;
		///		&lt;/connectionStrings&gt;
		///	&lt;/configuration&gt;
		/// </code>
		/// a nebo po staru <b>ConnectionString</b> z konfigurační sekce &lt;appStrings&gt;:
		/// <code>
		/// &lt;configuration&gt;
		///		&lt;appSettings&gt;
		///			&lt;add key="ConnectionString" value="Server=localhost;Database=pubs;UID=user;PWD=heslo;" /&gt;
		///		&lt;/appSettings&gt;
		///	&lt;/configuration&gt;
		/// </code>
		/// </example>
		public static string ConfigConnectionString
		{
			get
			{
				ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["DefaultConnectionString"]; 
				if (css != null)
				{
					return css.ConnectionString;
				}
				else
				{
					string result = ConfigurationManager.AppSettings["ConnectionString"];
					if (result == null)
					{
						throw new ConfigurationErrorsException("V konfiguračním souboru není nastaven výchozí ConnectionString");
					}
					return result;
				}
			}
		}
		#endregion

		#region GetConnection
		/// <summary>
		/// Vytvoří novou instanci SqlConnection podle požadovaného connectionStringu
		/// a případně ji rovnou otevře.
		/// </summary>
		/// <param name="connectionString">ConnectionString</param>
		/// <param name="open"><c>true</c>, má-li se nová SqlConnection rovnou otevřít</param>
		/// <returns>nová instance SqlConnection</returns>
		public static SqlConnection GetConnection(string connectionString, bool open)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			if (open)
			{
				conn.Open();
			}
			return conn;
		}


		/// <summary>
		/// Vytvoří novou instanci SqlConnection podle z implicitního <see cref="ConnectionString"/>
		/// a případně ji rovnou otevře.
		/// </summary>
		/// <param name="open"><c>true</c>, má-li se nová SqlConnection rovnou otevřít</param>
		/// <returns>nová instance SqlConnection</returns>
		public static SqlConnection GetConnection(bool open)
		{
			return (SqlConnection)DbConnector.Default.GetConnection(open);
		}


		/// <summary>
		/// Vytvoří novou instanci SqlConnection podle implicitního <see cref="ConnectionString"/>.
		/// SqlConnection není otevřena.
		/// </summary>
		/// <returns>nová instance SqlConnection (není otevřena)</returns>
		public static SqlConnection GetConnection()
		{
			return (SqlConnection)DbConnector.Default.GetConnection();
		}
		#endregion

		#region ExecuteNonQuery

		/// <summary>
		/// Vykoná SqlCommand a vrátí počet dotčených řádek.
		/// Nejobecnější metoda, kterou používají ostatní overloady.
		/// </summary>
		/// <remarks>
		/// Není-li SqlConnection příkazu nastaveno, použije imlicitní.
		/// Není-li SqlConnection dosud otevřeno, otevře ho, vykoná příkaz a zavře.
		/// Nemá-li počet dotčených řádek smysl, vrací -1.
		/// </remarks>
		/// <param name="cmd">SqlCommand, který má být vykonán</param>
		/// <returns>počet dotčených řádek</returns>
		public static int ExecuteNonQuery(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteNonQuery(cmd);
		}


		/// <summary>
		/// Vykoná zadaný příkaz určeného typu bez parametrů. Vrátí počet dotčených řádek.
		/// </summary>
		/// <param name="cmdText">SQL příkaz</param>
		/// <param name="cmdType">CommandType příkazu</param>
		/// <returns>počet dotčených řádek</returns>
		public static int ExecuteNonQuery(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteNonQuery(cmdText, cmdType);
		}


		/// <summary>
		/// Vykoná zadaný příkaz bez parametrů. Vrátí počet dotčených řádek.
		/// CommandType příkazu je nastaven z property DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">SQL příkaz</param>
		/// <returns>počet dotčených řádek</returns>
		public static int ExecuteNonQuery(string cmdText)
		{
			return DbConnector.Default.ExecuteNonQuery(cmdText);
		}

		#endregion

		#region ExecuteDataSet

		/// <summary>
		/// Vykoná SqlCommand a vrátí resultset ve formě DataSetu.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevřeno, nechá ho otevřené. Není-li, otevře si ho a zase zavře.
		/// </remarks>
		/// <param name="cmd">SqlCommand k vykonání</param>
		/// <returns>resultset příkazu ve formě DataSetu</returns>
		public static DataSet ExecuteDataSet(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteDataSet(cmd);
		}


		/// <summary>
		/// Vykoná příkaz cmdText daného cmdType proti implicitní connection
		/// a vrátí resultset ve formě DataSetu.
		/// </summary>
		/// <param name="cmdText">SQL příkaz</param>
		/// <param name="cmdType">typ příkazu</param>
		/// <returns>resultset příkazu ve formě DataSetu</returns>
		public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteDataSet(cmdText, cmdType);
		}


		/// <summary>
		/// Vykoná SQL příkaz cmdText s implicitního typu (DefaultCommandType)
		/// proti implicitní connection (GetConnection).
		/// </summary>
		/// <param name="cmdText">textový SQL příkaz</param>
		/// <returns>resultset ve formě DataSetu</returns>
		public static DataSet ExecuteDataSet(string cmdText)
		{
			return DbConnector.Default.ExecuteDataSet(cmdText);			
		}

		#endregion

		#region ExecuteDataTable
		/// <summary>
		/// Vykoná SqlCommand a vrátí první resultset ve formě <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevřeno, nechá ho otevřené. Není-li, otevře si ho a zase zavře.
		/// </remarks>
		/// <param name="cmd">SqlCommand k vykonání</param>
		/// <returns>první resultset příkazu ve formě <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteDataTable(cmd);
		}


		/// <summary>
		/// Vykoná příkaz cmdText daného cmdType proti implicitní connection
		/// a vrátí první resultset ve formě <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="cmdText">SQL příkaz</param>
		/// <param name="cmdType">typ příkazu</param>
		/// <returns>první resultset příkazu ve formě <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteDataTable(cmdText, cmdType);
		}


		/// <summary>
		/// Vykoná SQL příkaz cmdText s implicitního typu (DefaultCommandType)
		/// proti implicitní connection (GetConnection) a vrátí první resultset
		/// ve formě <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="cmdText">textový SQL příkaz</param>
		/// <returns>první resultset ve formě <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(string cmdText)
		{
			return DbConnector.Default.ExecuteDataTable(cmdText);
		}
		#endregion

		#region ExecuteReader

		/// <summary>
		/// Donastaví a vykoná SqlCommand pomocí CommandBehavior a vrátí výsledný resultset
		/// ve formě SqlDataReaderu.
		/// </summary>
		/// <param name="cmd">příkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <returns>Resultset jako SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(SqlCommand cmd, CommandBehavior behavior)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmd, behavior);
		}


		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí výsledný resultset ve formě SqlDataReaderu.
		/// </summary>
		/// <param name="cmd">příkaz (nemusí mít nastaveno Connection)</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(SqlCommand cmd)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmd);
		}


		/// <summary>
		/// Vytvoří, nastaví a vykoná SqlCommand dle zadaných parametrů a vrátí
		/// výsledný resultset ve formě SqlDataReaderu.
		/// </summary>
		/// <param name="cmdText">text SQL příkazu</param>
		/// <param name="cmdType">typ příkazu</param>
		/// <returns>resultset ve formě SqlDataReaderu</returns>
		public static SqlDataReader ExecuteReader(string cmdText, CommandType cmdType)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmdText, cmdType);
		}


		/// <summary>
		/// Vytvoří, nastaví a vykoná SqlCommand dle zadaného SQL příkazu
		/// a vrátí výsledný resultset ve formě SqlDataReaderu.
		/// Jako typ příkazu se použije DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">text SQL příkazu</param>
		/// <returns>resultset ve formě SqlDataReaderu</returns>
		public static SqlDataReader ExecuteReader(string cmdText)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmdText);
		}

		#endregion

		#region ExecuteDataRecord
		/// <summary>
		/// Donastaví a vykoná SqlCommand pomocí CommandBehavior a vrátí první řádek prvního resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmd">příkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <returns>první řádek prvního resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(SqlCommand cmd, CommandBehavior behavior)
		{
			return DbConnector.Default.ExecuteDataRecord(cmd, behavior);
		}

		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí první řádek prvního resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmd">příkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první řádek prvního resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteDataRecord(cmd);
		}


		/// <summary>
		/// Vytvoří SqlCommand dle zadaných parametrů, donasataví ho a vrátí první řádek prvního resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmdText">text SQL příkazu</param>
		/// <param name="cmdType">typ SQL příkazu</param>
		/// <returns>první řádek prvního resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteDataRecord(cmdText, cmdType);
		}


		/// <summary>
		/// Vytvoří, nastaví a vykoná SqlCommand dle zadaného SQL příkazu
		/// a vrátí výsledný resultset ve formě <see cref="Havit.Data.DataRecord"/>.
		/// Jako typ příkazu se použije DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">text SQL příkazu</param>
		/// <returns>první řádek prvního resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(string cmdText)
		{
			return DbConnector.Default.ExecuteDataRecord(cmdText);
		}
		#endregion

		#region ExecuteScalar

		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí první sloupec prvního řádku jeho resultsetu.
		/// </summary>
		/// <example>
		/// int result = (int)ExecuteScalar(cmd);
		/// </example>
		/// <param name="cmd">příkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první sloupec prvního řádku resultsetu</returns>
		public static object ExecuteScalar(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteScalar(cmd);
		}


		/// <summary>
		/// Vytvoří ze zadaných parametrů SqlCommand, vykoná ho a vrátí první sloupec
		/// prvního řádku jeho resultsetu.
		/// </summary>
		/// <param name="cmdText">text SQL příkazu</param>
		/// <param name="cmdType">typ příkazu</param>
		/// <returns>první sloupec prvního řádku resultsetu</returns>
		public static object ExecuteScalar(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteScalar(cmdText, cmdType);
		}


		/// <summary>
		/// Vytvoří SqlCommand, nastaví mu DefaultCommandType, vykoná ho a vrátí
		/// první sloupec prvního řádku jeho resultsetu.
		/// </summary>
		/// <param name="cmdText">text SQL příkazu</param>
		/// <returns>první sloupec prvního řádku resultsetu</returns>
		public static object ExecuteScalar(string cmdText)
		{
			return DbConnector.Default.ExecuteScalar(cmdText);
		}

		#endregion

		#region ExecuteTransaction
		/// <summary>
		/// Vykoná požadované kroky v rámci transakce.
		/// Pokud je zadaná transakce <c>null</c>, je spuštěna a commitována nová.
		/// Pokud zadaná transakce není <c>null</c>, jsou zadané kroky pouze vykonány.
		/// </summary>
		/// <param name="transactionWork"><see cref="SqlTransactionDelegate"/> reprezentující s úkony, které mají být součástí transakce</param>
		/// <param name="outerTransaction">transakce</param>
		public static void ExecuteTransaction(SqlTransactionDelegate transactionWork, SqlTransaction outerTransaction)
		{
			DbConnector.Default.ExecuteTransaction(delegate(DbTransaction innerTransaction)
			{
				transactionWork((SqlTransaction)innerTransaction);
			}, outerTransaction);
		}

		/// <summary>
		/// Vykoná požadované kroky v rámci transakce.
		/// Je spuštěna a commitována nová samostatná transakce.
		/// </summary>
		/// <param name="transactionWork"><see cref="SqlTransactionDelegate"/> reprezentující s úkony, které mají být součástí transakce</param>
		public static void ExecuteTransaction(SqlTransactionDelegate transactionWork)
		{
			DbConnector.Default.ExecuteTransaction(delegate(DbTransaction innerTransaction)
			{
				transactionWork((SqlTransaction)innerTransaction);
			}, null);
		}
		#endregion
	}
}
