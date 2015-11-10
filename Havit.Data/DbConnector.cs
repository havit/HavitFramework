using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading;

using Havit.Data.Extensions;
using Havit.Data.Trace;
using Havit.Diagnostics.Contracts;

namespace Havit.Data
{
	/// <summary>
	/// Třída usnadňující práci s databázemi.
	/// </summary>	
	public class DbConnector
	{
		#region commandExecutionTrace (private field)
		private readonly TraceSource commandExecutionTrace = new TraceSource("DbConnector Command Execution Trace", SourceLevels.All);
		#endregion

		#region ConnectionString
		/// <summary>
		/// Vrátí connection-string, který spolu s <see cref="DbConnector.ProviderFactory"/> určuje parametry DbConnectoru.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				//Contract.Ensures(Contract.Result<string>() != null);
				//Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));
				return _connectionString;
			}
		}
		private readonly string _connectionString;
		#endregion

		#region ProviderFactory
		/// <summary>
		/// Vrátí <see cref="DbProviderFactory"/>, která spolu s <see cref="DbConnector.ConnectionString"/>em určuje parametry DbConnectoru.
		/// </summary>
		public DbProviderFactory ProviderFactory
		{
			get
			{
				//Contract.Ensures(Contract.Result<DbProviderFactory>() != null);
				return _providerFactory;
			}
		}
		private readonly DbProviderFactory _providerFactory;
		#endregion

		#region Constructors
		/// <summary>
		/// Inicializuje instanci třídy <see cref="DbConnector"/>.
		/// </summary>
		/// <param name="connectionString">connection-string</param>
		/// <param name="providerFactory">DbProviderFactory</param>
		public DbConnector(string connectionString, DbProviderFactory providerFactory)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(connectionString), "Parametr connectionString nesmí být null ani String.Empty.");
			Contract.Requires<ArgumentNullException>(providerFactory != null, "providerFactory");

			this._connectionString = connectionString;
			this._providerFactory = providerFactory;
		}

		/// <summary>
		/// Inicializuje instanci třídy <see cref="DbConnector"/>.
		/// </summary>
		/// <param name="connectionString">Connection-string</param>
		/// <param name="providerInvariantName">Invariant name of a provider.</param>
		public DbConnector(string connectionString, string providerInvariantName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(connectionString), "Parametr connectionString nesmí být null ani String.Empty.");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(providerInvariantName), "Parametr providerInvariantName nesmí být null ani String.Empty.");

			this._connectionString = connectionString;
			this._providerFactory = DbProviderFactories.GetFactory(providerInvariantName);
		}

		/// <summary>
		/// Inicializuje instanci třídy <see cref="DbConnector"/>.
		/// </summary>
		/// <param name="connectionStringSettings">Nastavení <see cref="ConnectionStringSettings"/> načtené z .config souboru. Např. získané přes ConfigurationManager.ConnectionStrings["ConnectionStringName"].</param>
		public DbConnector(ConnectionStringSettings connectionStringSettings)
		{
			Contract.Requires<ArgumentNullException>(connectionStringSettings != null, "connectionStringSettings");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(connectionStringSettings.ConnectionString), "Parametr connectionStringSettings nemá nastavenu vlastnost ConnectionString.");

			this._connectionString = connectionStringSettings.ConnectionString;
			if (String.IsNullOrEmpty(connectionStringSettings.ProviderName))
			{
				this._providerFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
			}
			else
			{
				this._providerFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
			}
		}
		#endregion

		#region CreateCommand, SetCommandDefaults (private)
		/// <summary>
		/// Vytvoří DbCommand dle zadaných parametrů. Nenastavuje spojení ani jiné vlastnosti.
		/// </summary>
		/// <param name="commandText">SQL text příkazu</param>
		/// <param name="commandType">typ příkazu <see cref="CommandType"/></param>
		private DbCommand CreateCommand(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Argument commandText nesmí být null ani String.Empty."); 
			// CommandType nemůže být null a není potřeba ho kontrolovat

			DbCommand cmd = this.ProviderFactory.CreateCommand();
			cmd.CommandText = commandText;
			cmd.CommandType = commandType;

			return cmd;
		}

		/// <summary>
		/// Nastaví příkazu default parametry (zatím pouze Connection), nejsou-li nastaveny.
		/// </summary>
		/// <remarks>
		/// Pokud jsme v transakci, pak zde sjednotíme Connection (nechápu, proč to nedělá sám .NET Framework).
		/// </remarks>
		/// <param name="command"><see cref="DbCommand"/> k nastavení</param>
		private void SetCommandDefaults(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");

			if (command.Transaction != null)
			{
				command.Connection = command.Transaction.Connection;
			}

			if (command.Connection == null)
			{
				command.Connection = GetConnection();
			}
		}
		#endregion

		#region GetConnection
		/// <summary>
		/// Vrátí novou instanci provider-specific potomka <see cref="DbConnection"/> a pokud to požadujeme, tak ji rovnou otevře.
		/// </summary>
		/// <param name="openConnection"><c>true</c>, má-li se nová SqlConnection rovnou otevřít</param>
		public DbConnection GetConnection(bool openConnection)
		{
			//Contract.Ensures(Contract.Result<DbConnection>() != null);

			DbConnection conn = this.ProviderFactory.CreateConnection();
			conn.ConnectionString = this.ConnectionString;
			if (openConnection)
			{
				conn.OpenWithRetry();
			}
			return conn;
		}

		/// <summary>
		/// Vrátí novou instanci provider-specific potomka <see cref="DbConnection"/>.
		/// Connection není otevřena.
		/// </summary>
		public DbConnection GetConnection()
		{
			return GetConnection(false);
		}
		#endregion

		#region ExecuteNonQuery
		/// <summary>
		/// Vykoná <see cref="DbCommand"/> a vrátí počet dotčených řádek.
		/// Nejobecnější metoda, kterou používají ostatní overloady.
		/// </summary>
		/// <remarks>
		/// Není-li Connection příkazu nastaveno, použije imlicitní.
		/// Není-li Connection dosud otevřeno, otevře ho, vykoná příkaz a zavře.
		/// Nemá-li počet dotčených řádek smysl, vrací -1.
		/// </remarks>
		/// <param name="command"><see cref="DbCommand"/>, který má být vykonán</param>
		/// <returns>počet dotčených řádek</returns>
		public int ExecuteNonQuery(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");

			SetCommandDefaults(command);

			bool mustCloseConnection = false;
			if (command.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				command.Connection.OpenWithRetry();
			}

			DbConnectorTrace dbConnectorTrace = new DbConnectorTrace(command, "ExecuteNonQuery");

			int result;
			try
			{
				result = command.ExecuteNonQueryWithRetry();
			}
			catch
			{
				command.Connection.Close();
				throw;
			}

			if (mustCloseConnection)
			{
				command.Connection.Close();
			}

			dbConnectorTrace.Trace(this.commandExecutionTrace);	

			return result;
		}

		/// <summary>
		/// Vykoná zadaný příkaz určeného typu bez parametrů. Vrátí počet dotčených řádek.
		/// </summary>
		/// <param name="commandText">SQL příkaz</param>
		/// <param name="commandType"><see cref="CommandType"/> příkazu</param>
		/// <returns>počet dotčených řádek</returns>
		public int ExecuteNonQuery(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteNonQuery(CreateCommand(commandText, commandType));
		}

		/// <summary>
		/// Vykoná zadaný příkaz bez parametrů. Vrátí počet dotčených řádek.
		/// </summary>
		/// <remarks>
		/// Jako <see cref="CommandType"/> používá <see cref="CommandType.Text"/>.
		/// </remarks>
		/// <param name="commandText">SQL příkaz</param>
		/// <returns>počet dotčených řádek</returns>
		public int ExecuteNonQuery(string commandText)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteNonQuery(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteDataSet
		/// <summary>
		/// Vykoná <see cref="DbCommand"/> a vrátí resultset ve formě <see cref="DataSet"/>u.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevřeno, nechá ho otevřené. Není-li, otevře si ho a zase zavře.
		/// </remarks>
		/// <param name="command">DbCommand k vykonání</param>
		/// <returns>resultset příkazu ve formě <see cref="DataSet"/>u</returns>
		public DataSet ExecuteDataSet(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");

			SetCommandDefaults(command);

			using (DbDataAdapter adapter = this.ProviderFactory.CreateDataAdapter())
			{
				adapter.SelectCommand = command;

				DataSet ds = new DataSet();

				DbConnectorTrace dbConnectorTrace = new DbConnectorTrace(command, "ExecuteDataSet");
				adapter.Fill(ds);

				int tablesCount = ds.Tables.Count;
				if (tablesCount == 0)
				{
					dbConnectorTrace.SetResult("0 tables");
				}
				else
				{
					int recordsCount = ds.Tables.Cast<DataTable>().Sum(item => item.Rows.Count);
					dbConnectorTrace.SetResult(String.Format("{0} table{1} with {2} record{3}",
						tablesCount,
						(tablesCount == 1) ? "" : "s",
						recordsCount,
						(recordsCount == 1) ? "" : "s"));
				}
				
				dbConnectorTrace.Trace(this.commandExecutionTrace);
				
				return ds;
			}
		}

		/// <summary>
		/// Vykoná příkaz commandText daného commandType a vrátí resultset ve formě <see cref="DataSet"/>u.
		/// </summary>
		/// <param name="commandText">SQL příkaz</param>
		/// <param name="commandType">typ příkazu</param>
		/// <returns>resultset příkazu ve formě <see cref="DataSet"/>u</returns>
		public DataSet ExecuteDataSet(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteDataSet(CreateCommand(commandText, commandType));
		}

		/// <summary>
		/// Vykoná SQL příkaz cmdText typu <see cref="CommandType.Text"/> a vrátí resultset ve formě <see cref="DataSet"/>u.
		/// </summary>
		/// <param name="commandText">textový SQL příkaz</param>
		/// <returns>resultset příkazu ve formě <see cref="DataSet"/>u</returns>
		public DataSet ExecuteDataSet(string commandText)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteDataSet(commandText, CommandType.Text);
		}

		#endregion

		#region ExecuteDataTable
		/// <summary>
		/// Vykoná <see cref="DbCommand"/> a vrátí první resultset ve formě <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevřeno, nechá ho otevřené. Není-li, otevře si ho a zase zavře.
		/// </remarks>
		/// <param name="command"><see cref="DbCommand"/> k vykonání</param>
		/// <returns>první resultset příkazu ve formě <see cref="System.Data.DataTable"/></returns>
		public DataTable ExecuteDataTable(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");

			SetCommandDefaults(command);

			using (DbDataAdapter adapter = this.ProviderFactory.CreateDataAdapter())
			{
				adapter.SelectCommand = command;

				DataTable dt = new DataTable();

				DbConnectorTrace dbConnectorTrace = new DbConnectorTrace(command, "ExecuteDataTable");
				adapter.Fill(dt);

				int recordsCount = dt.Rows.Count;
				dbConnectorTrace.SetResult(String.Format("{0} record{1}",
					recordsCount,
					(recordsCount == 1) ? "" : "s"));
				dbConnectorTrace.Trace(this.commandExecutionTrace);

				return dt;
			}
		}

		/// <summary>
		/// Vykoná příkaz commandText typu commandType a vrátí první tabulku resultsetu ve formě <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="commandText">SQL příkaz</param>
		/// <param name="commandType">typ příkazu</param>
		/// <returns>první tabulka resultsetu vykonaného příkazu ve formě <see cref="System.Data.DataTable"/></returns>
		public DataTable ExecuteDataTable(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteDataTable(CreateCommand(commandText, commandType));
		}

		/// <summary>
		/// Vykoná příkaz commandText typu <see cref="CommandType.Text"/> a vrátí první tabulku resultsetu ve formě <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="commandText">textový SQL příkaz</param>
		/// <returns>první tabulka resultsetu vykonaného příkazu ve formě <see cref="System.Data.DataTable"/></returns>
		public DataTable ExecuteDataTable(string commandText)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteDataTable(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteReader
		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> pomocí <see cref="CommandBehavior"/> a vrátí výsledný resultset ve formě <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="command">příkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <returns>resultset vykonaného příkazu jako <see cref="DbDataReader"/></returns>
		public DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");

			SetCommandDefaults(command);

			bool mustCloseConnection = false;
			if (command.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				command.Connection.OpenWithRetry();
			}

			DbConnectorTrace dbConnectorTrace = new DbConnectorTrace(command, "ExecuteReader");
			DbDataReader reader;

			try
			{
				if (mustCloseConnection)
				{
					// otevřeme-li si spojení sami, postaráme se i o jeho zavření
					reader = command.ExecuteReaderWithRetry(behavior | CommandBehavior.CloseConnection);
				}
				else
				{
					// spojení bylo již otevřeno, tak ho tak necháme, ať se stará nadřazená aplikace
					reader = command.ExecuteReaderWithRetry(behavior);
				}
			}
			catch
			{
				if (mustCloseConnection)
				{
					command.Connection.Close();
				}
				throw;
			}

			DbDataReader result = new TraceDbDataReader(reader, dbConnectorTrace);

			dbConnectorTrace.Trace(this.commandExecutionTrace);

			return result;
		}

		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> a vrátí výsledný resultset ve formě <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="command">příkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>resultset vykonaného příkazu jako <see cref="DbDataReader"/></returns>
		public DbDataReader ExecuteReader(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");
			return ExecuteReader(command, CommandBehavior.Default);
		}

		/// <summary>
		/// Vytvoří, nastaví a vykoná <see cref="DbCommand"/> dle zadaných parametrů a vrátí výsledný resultset ve formě <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="commandText">text SQL příkazu</param>
		/// <param name="commandType">typ příkazu <see cref="CommandType"/></param>
		/// <returns>resultset ve formě <see cref="DbDataReader"/>u</returns>
		public DbDataReader ExecuteReader(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteReader(CreateCommand(commandText, commandType));
		}

		/// <summary>
		/// Vytvoří, nastaví a vykoná <see cref="DbCommand"/> dle zadaného SQL příkazu typu <see cref="CommandType.Text"/>
		/// a vrátí výsledný resultset ve formě <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="commandText">text SQL příkazu</param>
		/// <returns>resultset ve formě <see cref="DbDataReader"/>u</returns>
		public DbDataReader ExecuteReader(string commandText)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteReader(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteDataRecord
		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> pomocí <see cref="CommandBehavior"/> a vrátí první řádek první tabulky resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <param name="command">příkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <param name="dataLoadPower"><see cref="DataLoadPower"/>, která se má použít pro <see cref="DataRecord"/></param>
		/// <returns>první řádek první tabulky resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(DbCommand command, CommandBehavior behavior, DataLoadPower dataLoadPower)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");			
			using (DbDataReader reader = ExecuteReader(command, behavior))
			{
				if (reader.Read())
				{
					return new DataRecord(reader, dataLoadPower);
				}
				return null;
			}
		}

		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> pomocí <see cref="CommandBehavior"/> a vrátí první řádek první tabulky resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> výsledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="command">příkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">požadované "chování"</param>
		/// <returns>první řádek první tabulky resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(DbCommand command, CommandBehavior behavior)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");
			using (DbDataReader reader = ExecuteReader(command, behavior))
			{
				if (reader.Read())
				{
					return new DataRecord(reader, DataLoadPower.FullLoad);
				}
				return null;
			}
		}

		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> a vrátí první řádek první tabulky resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> výsledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="command">příkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první řádek první tabulky resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");
			return ExecuteDataRecord(command, CommandBehavior.Default);
		}

		/// <summary>
		/// Vytvoří <see cref="DbCommand"/> dle zadaných parametrů, donasataví ho a vrátí první řádek první tabulky resultsetu
		/// ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> výsledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="commandText">text SQL příkazu</param>
		/// <param name="commandType"><see cref="CommandType"/> SQL příkazu</param>
		/// <returns>první řádek první tabulky resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteDataRecord(CreateCommand(commandText, commandType));
		}

		/// <summary>
		/// Vytvoří, nastaví a vykoná <see cref="DbCommand"/> dle zadaného SQL příkazu
		/// a vrátí první řádek první tabulky resultsetu ve formě <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> výsledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="commandText">text SQL příkazu</param>
		/// <returns>první řádek první tabulky resultsetu ve formě <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(string commandText)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteDataRecord(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteScalar

		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> a vrátí první sloupec prvního řádku první tabulky jeho resultsetu.
		/// </summary>
		/// <example>
		/// int result = (int)ExecuteScalar(cmd);
		/// </example>
		/// <param name="command">příkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první sloupec prvního řádku první tabulky resultsetu</returns>
		public object ExecuteScalar(DbCommand command)
		{
			Contract.Requires<ArgumentNullException>(command != null, "command");

			SetCommandDefaults(command);

			bool mustCloseConnection = false;
			if (command.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				command.Connection.OpenWithRetry();
			}

			object result;

			DbConnectorTrace dbConnectorTrace = new DbConnectorTrace(command, "ExecuteScalar");
			try
			{
				result = command.ExecuteScalarWithRetry();
			}
			catch
			{
				command.Connection.Close();
				throw;
			}

			if (mustCloseConnection)
			{
				command.Connection.Close();
			}

			dbConnectorTrace.SetResult(result);
			dbConnectorTrace.Trace(this.commandExecutionTrace);

			return result;
		}

		/// <summary>
		/// Vytvoří ze zadaných parametrů <see cref="DbCommand"/>, nastaví, vykoná ho a vrátí první sloupec
		/// prvního řádku první tabulky jeho resultsetu.
		/// </summary>
		/// <param name="commandText">text SQL příkazu</param>
		/// <param name="commandType">typ příkazu</param>
		/// <returns>první sloupec prvního řádku první tabulky resultsetu</returns>
		public object ExecuteScalar(string commandText, CommandType commandType)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteScalar(CreateCommand(commandText, commandType));
		}

		/// <summary>
		/// Vytvoří <see cref="DbCommand"/> typu <see cref="CommandType.Text"/>, vykoná ho a vrátí
		/// první sloupec prvního řádku první tabulky jeho resultsetu.
		/// </summary>
		/// <param name="commandText">text SQL příkazu typu <see cref="CommandType.Text"/></param>
		/// <returns>první sloupec prvního řádku resultsetu</returns>
		public object ExecuteScalar(string commandText)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(commandText), "Parametr commandText nesmí být null ani String.Empty");
			return ExecuteScalar(CreateCommand(commandText, CommandType.Text));
		}

		#endregion

		#region ExecuteTransaction
		/// <summary>
		/// Vykoná požadované kroky v rámci transakce.
		/// Pokud je outerTransaction <c>null</c>, je spuštěna a commitována nová.
		/// Pokud je outerTransaction zadáno, jsou požadované kroky v rámci ní pouze vykonány, pokud se shoduje IsolationLevel.
		/// Pokud se IsolationLevel neshoduje, založí se nová nested-transakce s požadovaným IsolationLevelem.
		/// </summary>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají být součástí transakce</param>
		/// <param name="outerTransaction">vnější transakce, pokud existuje; jinak <c>null</c></param>
		/// <param name="isolationLevel">požadovaný <see cref="IsolationLevel"/> transakce; pokud je <see cref="IsolationLevel.Unspecified"/>, použije se outerTransaction, pokud je definována, nebo založí nová transakce s defaultním isolation-levelem</param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork, DbTransaction outerTransaction, IsolationLevel isolationLevel)
		{
			Contract.Requires<ArgumentNullException>(transactionWork != null, "transactionWork");
			
			DbTransaction currentTransaction = outerTransaction;
			DbConnection connection;

			bool mustCloseConnection = false;
			bool mustCommitOrRollbackTransaction = false;

			if (outerTransaction == null)
			{
				// otevření spojení, pokud není již otevřeno
				connection = this.GetConnection();
				connection.OpenWithRetry();
				mustCloseConnection = true;
			}
			else
			{
				connection = outerTransaction.Connection;
			}

			if ((outerTransaction == null) || 
				((isolationLevel != IsolationLevel.Unspecified) && (outerTransaction.IsolationLevel != isolationLevel)))
			{
				if (isolationLevel == IsolationLevel.Unspecified)
				{
					currentTransaction = connection.BeginTransaction();
				}
				else
				{
					currentTransaction = connection.BeginTransaction(IsolationLevel.Unspecified);
				}
				mustCommitOrRollbackTransaction = true;
			}

			try
			{
				transactionWork(currentTransaction);

				if (mustCommitOrRollbackTransaction)
				{
					// commit chceme jen v případě, že jsme sami transakci založili (ať už úplně novou, nebo nested)
					currentTransaction.Commit();
				}
			}
			catch
			{
				try
				{
					if (mustCommitOrRollbackTransaction)
					{
						// rollback chceme taky jen v případě, že jsme sami transakci založili (ať už úplně novou, nebo nested)
						// případné vnější transakce shodí naše výjimka
						currentTransaction.Rollback();
					}
				}
				catch
				{
					// NOOP: chceme vyhodit věcnou výjimku, ne problém s rollbackem
				}
				throw;
			}
			finally
			{
				if (mustCloseConnection)
				{
					// uzavření spojení, pokud jsme iniciátory transakce
					connection.Close();
				}
			}
		}

		/// <summary>
		/// Vykoná požadované kroky v rámci transakce.
		/// Pokud je outerTransaction <c>null</c>, je spuštěna a commitována nová.
		/// Pokud je outerTransaction zadáno, jsou požadované kroky v rámci ní pouze vykonány.
		/// </summary>
		/// <remarks>
		/// Pokud je outerTransaction <c>null</c>, pak založí novou transakci s daným IsolationLevelem.
		/// Pokud je outerTransakce zadána, pak se pro zadaný transactionWork použije zadaný IsolationLevel a pak ho vrátí na původní hodnotu.
		/// </remarks>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají být součástí transakce</param>
		/// <param name="outerTransaction">vnější transakce, pokud existuje; jinak <c>null</c></param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork, DbTransaction outerTransaction)
		{
			Contract.Requires<ArgumentNullException>(transactionWork != null, "transactionWork");			
			ExecuteTransaction(transactionWork, outerTransaction, IsolationLevel.Unspecified);
		}

		/// <summary>
		/// Vykoná požadované kroky v rámci transakce s daným isolation-levelem.
		/// </summary>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají být součástí transakce</param>
		/// <param name="isolationLevel">požadovaný <see cref="IsolationLevel"/> transakce</param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork, IsolationLevel isolationLevel)
		{
			Contract.Requires<ArgumentNullException>(transactionWork != null, "transactionWork");
			ExecuteTransaction(transactionWork, null, isolationLevel);
		}

		/// <summary>
		/// Vykoná požadované kroky v rámci transakce.
		/// Je spuštěna a commitována nová samostatná transakce.
		/// </summary>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají být součástí transakce</param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork)
		{
			Contract.Requires<ArgumentNullException>(transactionWork != null, "transactionWork");
			ExecuteTransaction(transactionWork, null, IsolationLevel.Unspecified);
		}
		#endregion

		/**********************************************************************************************************/

		#region Default (static)
		/// <summary>
		/// Defaultní <see cref="DbConnector"/>. Pokud není nastaven ručně, pak se vytvoří při prvním přístupu z defaultního connection-stringu načteného z .config souboru.
		/// Nastavením na null mohu pro příští přístup vynutit opětovnou inicializaci z .config souboru.
		/// </summary>
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
		[SuppressMessage("Havit.StyleCop.Rules.HavitRules", "HA0002:MembersOrder", Justification = "Kód ohledně Default connectoru je pohromadě.")]
		public static DbConnector Default
		{
			get
			{
				//Contract.Ensures(Contract.Result<DbConnector>() != null);

				if (_default == null)
				{
					_default = GetDbConnectorFromDefaultConfig();
				}
				return _default;
			}
			set
			{
				_default = value;
			}
		}
		private static DbConnector _default;

		/// <summary>
		/// Vrátí <see cref="DbConnection"/> inicializovaný defaulty z .config souboru.
		/// </summary>
		/// <remarks>Viz vlastnost <see cref="DbConnector.Default"/>.</remarks>
		private static DbConnector GetDbConnectorFromDefaultConfig()
		{
			//Contract.Ensures(Contract.Result<DbConnector>() != null);

			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnectionString"];
			if (connectionStringSettings != null)
			{
				return new DbConnector(connectionStringSettings);
			}
			else
			{
				string appSettingsConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
				if (String.IsNullOrEmpty(appSettingsConnectionString))
				{
					throw new InvalidOperationException("Z konfiguračního souboru se nepodařilo načíst defaultní parametry připojení k databázi.");
				}
				return new DbConnector(appSettingsConnectionString, "System.Data.SqlClient");
			}
		}
		#endregion

	}
}
