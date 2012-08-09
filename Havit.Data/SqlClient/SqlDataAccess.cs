using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace Havit.Data.SqlClient
{
	/// <summary>
	/// Tøída SqlDataAccess obsahuje metody usnadòující práci se základní tøídou
	/// <b>System.Data.</b><see cref="System.Data.SqlClient"/>.
	/// </summary>
	/// <remarks>
	/// Inspirováno <a href="http://msdn.microsoft.com/vcsharp/downloads/components/default.aspx?pull=/library/en-us/dnbda/html/daab-rm.asp">
	/// Microsoft Data Access Application Block verze 2.0</a>.
	/// </remarks>
	[Obsolete("Místo tøídy SqlDataAccess použijte Havit.Data.DbConnector.")]
	public static class SqlDataAccess
	{
		#region ConnectionString, ConfigConnectionString
		/// <summary>
		/// Implicitní ConnectionString.
		/// Použije se vždy, není-li metodì jakkoliv explicitnì pøedán jiný.
		/// Není-li nastaven klientskou aplikací, naète se z .config souboru.
		/// </summary>
		/// <remarks>
		/// Pokus o naètení z .config souboru je proveden vždy, je-li právì nastaven
		/// na <c>null</c> nebo <c>String.Empty</c>. Jeho nastavením na <c>null</c> lze tedy
		/// poruèit, aby se ConnectionString naèetl z .config souboru.
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
		/// Pro .NET Framework 2.0 rozšíøeno o použití hodnoty <b>DefaultConnectionString</b>
		/// z konfiguraèní sekce &lt;connectionStrings&gt;.
		/// </remarks>
		/// <example>
		/// Buï <b>DefaultConnectionString</b> z konfiguraèní sekce &lt;connectionStrings&gt;:
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
		/// a nebo po staru <b>ConnectionString</b> z konfiguraèní sekce &lt;appStrings&gt;:
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
						throw new ConfigurationErrorsException("V konfiguraèním souboru není nastaven výchozí ConnectionString");
					}
					return result;
				}
			}
		}
		#endregion

		#region GetConnection
		/// <summary>
		/// Vytvoøí novou instanci SqlConnection podle požadovaného connectionStringu
		/// a pøípadnì ji rovnou otevøe.
		/// </summary>
		/// <param name="connectionString">ConnectionString</param>
		/// <param name="open"><c>true</c>, má-li se nová SqlConnection rovnou otevøít</param>
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
		/// Vytvoøí novou instanci SqlConnection podle z implicitního <see cref="ConnectionString"/>
		/// a pøípadnì ji rovnou otevøe.
		/// </summary>
		/// <param name="open"><c>true</c>, má-li se nová SqlConnection rovnou otevøít</param>
		/// <returns>nová instance SqlConnection</returns>
		public static SqlConnection GetConnection(bool open)
		{
			return (SqlConnection)DbConnector.Default.GetConnection(open);
		}


		/// <summary>
		/// Vytvoøí novou instanci SqlConnection podle implicitního <see cref="ConnectionString"/>.
		/// SqlConnection není otevøena.
		/// </summary>
		/// <returns>nová instance SqlConnection (není otevøena)</returns>
		public static SqlConnection GetConnection()
		{
			return (SqlConnection)DbConnector.Default.GetConnection();
		}
		#endregion

		#region ExecuteNonQuery

		/// <summary>
		/// Vykoná SqlCommand a vrátí poèet dotèených øádek.
		/// Nejobecnìjší metoda, kterou používají ostatní overloady.
		/// </summary>
		/// <remarks>
		/// Není-li SqlConnection pøíkazu nastaveno, použije imlicitní.
		/// Není-li SqlConnection dosud otevøeno, otevøe ho, vykoná pøíkaz a zavøe.
		/// Nemá-li poèet dotèených øádek smysl, vrací -1.
		/// </remarks>
		/// <param name="cmd">SqlCommand, který má být vykonán</param>
		/// <returns>poèet dotèených øádek</returns>
		public static int ExecuteNonQuery(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteNonQuery(cmd);
		}


		/// <summary>
		/// Vykoná zadaný pøíkaz urèeného typu bez parametrù. Vrátí poèet dotèených øádek.
		/// </summary>
		/// <param name="cmdText">SQL pøíkaz</param>
		/// <param name="cmdType">CommandType pøíkazu</param>
		/// <returns>poèet dotèených øádek</returns>
		public static int ExecuteNonQuery(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteNonQuery(cmdText, cmdType);
		}


		/// <summary>
		/// Vykoná zadaný pøíkaz bez parametrù. Vrátí poèet dotèených øádek.
		/// CommandType pøíkazu je nastaven z property DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">SQL pøíkaz</param>
		/// <returns>poèet dotèených øádek</returns>
		public static int ExecuteNonQuery(string cmdText)
		{
			return DbConnector.Default.ExecuteNonQuery(cmdText);
		}

		#endregion

		#region ExecuteDataSet

		/// <summary>
		/// Vykoná SqlCommand a vrátí resultset ve formì DataSetu.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevøeno, nechá ho otevøené. Není-li, otevøe si ho a zase zavøe.
		/// </remarks>
		/// <param name="cmd">SqlCommand k vykonání</param>
		/// <returns>resultset pøíkazu ve formì DataSetu</returns>
		public static DataSet ExecuteDataSet(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteDataSet(cmd);
		}


		/// <summary>
		/// Vykoná pøíkaz cmdText daného cmdType proti implicitní connection
		/// a vrátí resultset ve formì DataSetu.
		/// </summary>
		/// <param name="cmdText">SQL pøíkaz</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns>resultset pøíkazu ve formì DataSetu</returns>
		public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteDataSet(cmdText, cmdType);
		}


		/// <summary>
		/// Vykoná SQL pøíkaz cmdText s implicitního typu (DefaultCommandType)
		/// proti implicitní connection (GetConnection).
		/// </summary>
		/// <param name="cmdText">textový SQL pøíkaz</param>
		/// <returns>resultset ve formì DataSetu</returns>
		public static DataSet ExecuteDataSet(string cmdText)
		{
			return DbConnector.Default.ExecuteDataSet(cmdText);			
		}

		#endregion

		#region ExecuteDataTable
		/// <summary>
		/// Vykoná SqlCommand a vrátí první resultset ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevøeno, nechá ho otevøené. Není-li, otevøe si ho a zase zavøe.
		/// </remarks>
		/// <param name="cmd">SqlCommand k vykonání</param>
		/// <returns>první resultset pøíkazu ve formì <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteDataTable(cmd);
		}


		/// <summary>
		/// Vykoná pøíkaz cmdText daného cmdType proti implicitní connection
		/// a vrátí první resultset ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="cmdText">SQL pøíkaz</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns>první resultset pøíkazu ve formì <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteDataTable(cmdText, cmdType);
		}


		/// <summary>
		/// Vykoná SQL pøíkaz cmdText s implicitního typu (DefaultCommandType)
		/// proti implicitní connection (GetConnection) a vrátí první resultset
		/// ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="cmdText">textový SQL pøíkaz</param>
		/// <returns>první resultset ve formì <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(string cmdText)
		{
			return DbConnector.Default.ExecuteDataTable(cmdText);
		}
		#endregion

		#region ExecuteReader

		/// <summary>
		/// Donastaví a vykoná SqlCommand pomocí CommandBehavior a vrátí výsledný resultset
		/// ve formì SqlDataReaderu.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <returns>Resultset jako SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(SqlCommand cmd, CommandBehavior behavior)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmd, behavior);
		}


		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí výsledný resultset ve formì SqlDataReaderu.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(SqlCommand cmd)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmd);
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná SqlCommand dle zadaných parametrù a vrátí
		/// výsledný resultset ve formì SqlDataReaderu.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns>resultset ve formì SqlDataReaderu</returns>
		public static SqlDataReader ExecuteReader(string cmdText, CommandType cmdType)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmdText, cmdType);
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná SqlCommand dle zadaného SQL pøíkazu
		/// a vrátí výsledný resultset ve formì SqlDataReaderu.
		/// Jako typ pøíkazu se použije DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <returns>resultset ve formì SqlDataReaderu</returns>
		public static SqlDataReader ExecuteReader(string cmdText)
		{
			return (SqlDataReader)DbConnector.Default.ExecuteReader(cmdText);
		}

		#endregion

		#region ExecuteDataRecord
		/// <summary>
		/// Donastaví a vykoná SqlCommand pomocí CommandBehavior a vrátí první øádek prvního resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(SqlCommand cmd, CommandBehavior behavior)
		{
			return DbConnector.Default.ExecuteDataRecord(cmd, behavior);
		}

		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí první øádek prvního resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteDataRecord(cmd);
		}


		/// <summary>
		/// Vytvoøí SqlCommand dle zadaných parametrù, donasataví ho a vrátí první øádek prvního resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <param name="cmdType">typ SQL pøíkazu</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteDataRecord(cmdText, cmdType);
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná SqlCommand dle zadaného SQL pøíkazu
		/// a vrátí výsledný resultset ve formì <see cref="Havit.Data.DataRecord"/>.
		/// Jako typ pøíkazu se použije <see cref="DefaultCommandType"/>.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(string cmdText)
		{
			return DbConnector.Default.ExecuteDataRecord(cmdText);
		}
		#endregion

		#region ExecuteScalar

		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí první sloupec prvního øádku jeho resultsetu.
		/// </summary>
		/// <example>
		/// int result = (int)ExecuteScalar(cmd);
		/// </example>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první sloupec prvního øádku resultsetu</returns>
		public static object ExecuteScalar(SqlCommand cmd)
		{
			return DbConnector.Default.ExecuteScalar(cmd);
		}


		/// <summary>
		/// Vytvoøí ze zadaných parametrù SqlCommand, vykoná ho a vrátí první sloupec
		/// prvního øádku jeho resultsetu.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns>první sloupec prvního øádku resultsetu</returns>
		public static object ExecuteScalar(string cmdText, CommandType cmdType)
		{
			return DbConnector.Default.ExecuteScalar(cmdText, cmdType);
		}


		/// <summary>
		/// Vytvoøí SqlCommand, nastaví mu DefaultCommandType, vykoná ho a vrátí
		/// první sloupec prvního øádku jeho resultsetu.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <returns>první sloupec prvního øádku resultsetu</returns>
		public static object ExecuteScalar(string cmdText)
		{
			return DbConnector.Default.ExecuteScalar(cmdText);
		}

		#endregion

		#region ExecuteTransaction
		/// <summary>
		/// Vykoná požadované kroky v rámci transakce.
		/// Pokud je zadaná transakce <c>null</c>, je spuštìna a commitována nová.
		/// Pokud zadaná transakce není <c>null</c>, jsou zadané kroky pouze vykonány.
		/// </summary>
		/// <param name="transactionWork"><see cref="SqlTransactionDelegate"/> reprezentující s úkony, které mají být souèástí transakce</param>
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
		/// Je spuštìna a commitována nová samostatná transakce.
		/// </summary>
		/// <param name="transactionWork"><see cref="SqlTransactionDelegate"/> reprezentující s úkony, které mají být souèástí transakce</param>
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
