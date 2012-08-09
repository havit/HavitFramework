using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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
	public static class SqlDataAccess
	{
		#region private CreateCommand, SetCommandDefaults
		/// <summary>
		/// Vytvoøí prázdnı SqlCommand dle zadanıch parametrù. Nenastavuje spojení ani jiné vlastnosti.
		/// </summary>
		/// <param name="cmdText">SQL text pøíkazu</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns></returns>
		private static SqlCommand CreateCommand(string cmdText, CommandType cmdType)
		{
			if ((cmdText == null) || (cmdText.Length == 0))
			{
				throw new ArgumentNullException("cmdText");
			}

			// CommandType nemùe bıt null a není potøeba ho kontrolovat

			SqlCommand cmd = new SqlCommand(cmdText);
			cmd.CommandType = cmdType;

			return cmd;
		}

		/// <summary>
		/// Nastaví pøíkazu default parametry (zatím pouze Connection), nejsou-li nastaveny.
		/// </summary>
		/// <remarks>
		/// Pokud jsme v transakci, pak sjednotíme Connection (nechápu, proè to nedìlá sám .NET Framework).
		/// </remarks>
		/// <param name="cmd">SqlCommand k nastavení</param>
		private static void SetCommandDefaults(SqlCommand cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}

			if (cmd.Transaction != null)
			{
				cmd.Connection = cmd.Transaction.Connection;
			}
			
			if (cmd.Connection == null)
			{
				cmd.Connection = GetConnection();
			}
		}
		#endregion

		#region ConnectionString, ConfigConnectionString
		/// <summary>
		/// Implicitní ConnectionString.
		/// Pouije se vdy, není-li metodì jakkoliv explicitnì pøedán jinı.
		/// Není-li nastaven klientskou aplikací, naète se z .config souboru.
		/// </summary>
		/// <remarks>
		/// Pokus o naètení z .config souboru je proveden vdy, je-li právì nastaven
		/// na <c>null</c> nebo <c>String.Empty</c>. Jeho nastavením na <c>null</c> lze tedy
		/// poruèit, aby se ConnectionString naèetl z .config souboru.
		/// </remarks>
		public static string ConnectionString
		{
			get
			{
				if ((connectionString == null) || (connectionString.Length == 0))
				{
					connectionString = ConfigConnectionString;
				}
				return connectionString;
			}
			set
			{
				connectionString = value;
			}
		}
		private static string connectionString;


		/// <summary>
		/// ConnectionString z .config souboru (web.config nebo applikace.exe.config).
		/// </summary>
		/// <remarks>
		/// Pro .NET Framework 2.0 rozšíøeno o pouití hodnoty <b>DefaultConnectionString</b>
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
						throw new ConfigurationErrorsException("V konfiguraèním souboru není nastaven vıchozí ConnectionString");
					}
					return result;
				}
			}
		}
		#endregion

		#region DefaultCommandType

		/// <summary>
		/// CommandType, kterı se pouije pro internì vytváøené pøíkazy,
		/// je-li jako parametr metody zadán pouze textovı SQL pøíkaz.
		/// </summary>
		/// <remarks>
		/// Vıchozí hodnota nastavena na <c>CommandType.StoredProcedure</c>.
		/// </remarks>
		public static CommandType DefaultCommandType
		{
			get
			{
				return defaultCommandType;
			}
			set
			{
				defaultCommandType = value;
			}
		}
		private static CommandType defaultCommandType = CommandType.StoredProcedure;

		#endregion

		#region GetConnection
		/// <summary>
		/// Vytvoøí novou instanci SqlConnection podle poadovaného connectionStringu
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
			return GetConnection(ConnectionString, open);
		}


		/// <summary>
		/// Vytvoøí novou instanci SqlConnection podle implicitního <see cref="ConnectionString"/>.
		/// SqlConnection není otevøena.
		/// </summary>
		/// <returns>nová instance SqlConnection (není otevøena)</returns>
		public static SqlConnection GetConnection()
		{
			return GetConnection(ConnectionString, false);
		}
		#endregion

		#region ExecuteNonQuery

		/// <summary>
		/// Vykoná SqlCommand a vrátí poèet dotèenıch øádek.
		/// Nejobecnìjší metoda, kterou pouívají ostatní overloady.
		/// </summary>
		/// <remarks>
		/// Není-li SqlConnection pøíkazu nastaveno, pouije imlicitní.
		/// Není-li SqlConnection dosud otevøeno, otevøe ho, vykoná pøíkaz a zavøe.
		/// Nemá-li poèet dotèenıch øádek smysl, vrací -1.
		/// </remarks>
		/// <param name="cmd">SqlCommand, kterı má bıt vykonán</param>
		/// <returns>poèet dotèenıch øádek</returns>
		public static int ExecuteNonQuery(SqlCommand cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}

			SetCommandDefaults(cmd);

			bool mustCloseConnection = false;
			if (cmd.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				cmd.Connection.Open();
			}

			int result;
			try
			{
				result = cmd.ExecuteNonQuery();
			}
			catch
			{
				cmd.Connection.Close();
				throw;
			}

			if (mustCloseConnection)
			{
				cmd.Connection.Close();
			}

			return result;
		}


		/// <summary>
		/// Vykoná zadanı pøíkaz urèeného typu bez parametrù. Vrátí poèet dotèenıch øádek.
		/// </summary>
		/// <param name="cmdText">SQL pøíkaz</param>
		/// <param name="cmdType">CommandType pøíkazu</param>
		/// <returns>poèet dotèenıch øádek</returns>
		public static int ExecuteNonQuery(string cmdText, CommandType cmdType)
		{
			return ExecuteNonQuery(CreateCommand(cmdText, cmdType));
		}


		/// <summary>
		/// Vykoná zadanı pøíkaz bez parametrù. Vrátí poèet dotèenıch øádek.
		/// CommandType pøíkazu je nastaven z property DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">SQL pøíkaz</param>
		/// <returns>poèet dotèenıch øádek</returns>
		public static int ExecuteNonQuery(string cmdText)
		{
			return ExecuteNonQuery(cmdText, DefaultCommandType);
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
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}

			SetCommandDefaults(cmd);

			using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				adapter.Fill(ds);

				return ds;
			}
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
			return ExecuteDataSet(CreateCommand(cmdText, cmdType));
		}


		/// <summary>
		/// Vykoná SQL pøíkaz cmdText s implicitního typu (DefaultCommandType)
		/// proti implicitní connection (GetConnection).
		/// </summary>
		/// <param name="cmdText">textovı SQL pøíkaz</param>
		/// <returns>resultset ve formì DataSetu</returns>
		public static DataSet ExecuteDataSet(string cmdText)
		{
			return ExecuteDataSet(cmdText, DefaultCommandType);
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
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}

			SetCommandDefaults(cmd);

			using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
			{
				DataTable dt = new DataTable();

				adapter.Fill(dt);

				return dt;
			}
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
			return ExecuteDataTable(CreateCommand(cmdText, cmdType));
		}


		/// <summary>
		/// Vykoná SQL pøíkaz cmdText s implicitního typu (DefaultCommandType)
		/// proti implicitní connection (GetConnection) a vrátí první resultset
		/// ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="cmdText">textovı SQL pøíkaz</param>
		/// <returns>první resultset ve formì <see cref="System.Data.DataTable"/></returns>
		public static DataTable ExecuteDataTable(string cmdText)
		{
			return ExecuteDataTable(cmdText, DefaultCommandType);
		}
		#endregion

		#region ExecuteReader

		/// <summary>
		/// Donastaví a vykoná SqlCommand pomocí CommandBehavior a vrátí vıslednı resultset
		/// ve formì SqlDataReaderu.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">poadované "chování"</param>
		/// <returns>Resultset jako SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(SqlCommand cmd, CommandBehavior behavior)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}

			SetCommandDefaults(cmd);

			bool mustCloseConnection = false;
			if (cmd.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				cmd.Connection.Open();
			}

			SqlDataReader reader;

			try
			{
				if (mustCloseConnection)
				{
					// otevøeme-li si spojení sami, postaráme se i o jeho zavøení
					reader = cmd.ExecuteReader(behavior | CommandBehavior.CloseConnection);
				}
				else
				{
					// spojení bylo ji otevøeno, tak ho tak necháme, a se stará nadøazená aplikace
					reader = cmd.ExecuteReader(behavior);
				}
			}
			catch
			{
				if (mustCloseConnection)
				{
					cmd.Connection.Close();
				}
				throw;
			}

			return reader;
		}


		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí vıslednı resultset ve formì SqlDataReaderu.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(SqlCommand cmd)
		{
			return ExecuteReader(cmd, CommandBehavior.Default);
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná SqlCommand dle zadanıch parametrù a vrátí
		/// vıslednı resultset ve formì SqlDataReaderu.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns>resultset ve formì SqlDataReaderu</returns>
		public static SqlDataReader ExecuteReader(string cmdText, CommandType cmdType)
		{
			return ExecuteReader(CreateCommand(cmdText, cmdType));
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná SqlCommand dle zadaného SQL pøíkazu
		/// a vrátí vıslednı resultset ve formì SqlDataReaderu.
		/// Jako typ pøíkazu se pouije DefaultCommandType.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <returns>resultset ve formì SqlDataReaderu</returns>
		public static SqlDataReader ExecuteReader(string cmdText)
		{
			return ExecuteReader(cmdText, DefaultCommandType);
		}

		#endregion

		#region ExecuteDataRecord
		/// <summary>
		/// Donastaví a vykoná SqlCommand pomocí CommandBehavior a vrátí první øádek prvního resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">poadované "chování"</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(SqlCommand cmd, CommandBehavior behavior)
		{
			using (SqlDataReader reader = ExecuteReader(cmd, behavior))
			{
				if (reader.Read())
				{
					return new DataRecord(reader);
				}
				return null;
			}
		}

		/// <summary>
		/// Donastaví a vykoná SqlCommand a vrátí první øádek prvního resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(SqlCommand cmd)
		{
			return ExecuteDataRecord(cmd, CommandBehavior.Default);
		}


		/// <summary>
		/// Vytvoøí SqlCommand dle zadanıch parametrù, donasataví ho a vrátí první øádek prvního resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí null.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <param name="cmdType">typ SQL pøíkazu</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(string cmdText, CommandType cmdType)
		{
			return ExecuteDataRecord(CreateCommand(cmdText, cmdType));
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná SqlCommand dle zadaného SQL pøíkazu
		/// a vrátí vıslednı resultset ve formì <see cref="Havit.Data.DataRecord"/>.
		/// Jako typ pøíkazu se pouije <see cref="DefaultCommandType"/>.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <returns>první øádek prvního resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public static DataRecord ExecuteDataRecord(string cmdText)
		{
			return ExecuteDataRecord(cmdText, DefaultCommandType);
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
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}

			SetCommandDefaults(cmd);

			bool mustCloseConnection = false;
			if (cmd.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				cmd.Connection.Open();
			}

			object result;

			try
			{
				result = cmd.ExecuteScalar();
			}
			catch
			{
				cmd.Connection.Close();
				throw;
			}

			if (mustCloseConnection)
			{
				cmd.Connection.Close();
			}

			return result;
		}


		/// <summary>
		/// Vytvoøí ze zadanıch parametrù SqlCommand, vykoná ho a vrátí první sloupec
		/// prvního øádku jeho resultsetu.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <param name="cmdType">typ pøíkazu</param>
		/// <returns>první sloupec prvního øádku resultsetu</returns>
		public static object ExecuteScalar(string cmdText, CommandType cmdType)
		{
			return ExecuteScalar(CreateCommand(cmdText, cmdType));
		}


		/// <summary>
		/// Vytvoøí SqlCommand, nastaví mu DefaultCommandType, vykoná ho a vrátí
		/// první sloupec prvního øádku jeho resultsetu.
		/// </summary>
		/// <param name="cmdText">text SQL pøíkazu</param>
		/// <returns>první sloupec prvního øádku resultsetu</returns>
		public static object ExecuteScalar(string cmdText)
		{
			return ExecuteScalar(CreateCommand(cmdText, DefaultCommandType));
		}

		#endregion

		#region ExecuteTransaction
		/// <summary>
		/// Vykoná poadované kroky v rámci transakce.
		/// Pokud je zadaná transakce <c>null</c>, je spuštìna a commitována nová.
		/// Pokud zadaná transakce není <c>null</c>, jsou zadané kroky pouze vykonány.
		/// </summary>
		/// <param name="transaction">transakce</param>
		public static void ExecuteTransaction(SqlTransactionDelegate transactionWork, SqlTransaction transaction)
		{
			SqlTransaction currentTransaction = transaction;
			SqlConnection connection;
			if (transaction == null)
			{
				// otevøení spojení, pokud jsme iniciátory transakce
				connection = SqlDataAccess.GetConnection();
				connection.Open();
				currentTransaction = connection.BeginTransaction();
			}
			else
			{
				connection = currentTransaction.Connection;
			}

			try
			{
				transactionWork(currentTransaction);

				if (transaction == null)
				{
					// commit chceme jen v pøípadì, e nejsme uvnitø vnìjší transakce
					currentTransaction.Commit();
				}
			}
			catch
			{
				try
				{
					currentTransaction.Rollback();
				}
				catch
				{
					// chceme vyhodit vnìjší vıjimku, ne problém s rollbackem
				}
				throw;
			}
			finally
			{
				// uzavøení spojení, pokud jsme iniciátory transakce
				if (transaction == null)
				{
					connection.Close();
				}
			}
		}

		/// <summary>
		/// Vykoná poadované kroky v rámci transakce.
		/// Je spuštìna a commitována nová samostatná transakce.
		/// </summary>
		public static void ExecuteTransaction(SqlTransactionDelegate transactionWork)
		{
			ExecuteTransaction(transactionWork, null);
		}
		#endregion
	}
}
