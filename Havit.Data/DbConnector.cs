using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Configuration;
using System.Data;

namespace Havit.Data
{
	/// <summary>
	/// Tøída usnadòující práci s databázemi. Nástupce <see cref="Havit.Data.SqlClient.SqlDataAccess"/>.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase")]
	public class DbConnector
	{
		#region ConnectionString
		/// <summary>
		/// Vrátí connection-string, kterı spolu s <see cref="DbConnector.ProviderFactory"/> urèuje parametry DbConnectoru.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}
		private string _connectionString;
		#endregion

		#region ProviderFactory
		/// <summary>
		/// Vrátí <see cref="DbProviderFactory"/>, která spolu s <see cref="DbConnector.ConnectionString"/>em urèuje parametry DbConnectoru.
		/// </summary>
		public DbProviderFactory ProviderFactory
		{
			get
			{
				return _providerFactory;
			}
		}
		private DbProviderFactory _providerFactory;
		#endregion

		#region Constructors
		/// <summary>
		/// Inicializuje instanci tøídy <see cref="DbConnector"/>.
		/// </summary>
		/// <param name="connectionString">connection-string</param>
		/// <param name="providerFactory">DbProviderFactory</param>
		public DbConnector(string connectionString, DbProviderFactory providerFactory)
		{
			if (String.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentException("Parametr nesmí bıt null ani String.Empty.", "connectionString");
			}
			if (providerFactory == null)
			{
				throw new ArgumentNullException("providerFactory");
			}

			this._connectionString = connectionString;
			this._providerFactory = providerFactory;
		}

		/// <summary>
		/// Inicializuje instanci tøídy <see cref="DbConnector"/>.
		/// </summary>
		/// <param name="connectionString">Connection-string</param>
		/// <param name="providerInvariantName">Invariant name of a provider.</param>
		public DbConnector(string connectionString, string providerInvariantName)
		{
			if (String.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentException("Parametr nesmí bıt null ani String.Empty.", "connectionString");
			}
			if (String.IsNullOrEmpty(providerInvariantName))
			{
				throw new ArgumentException("Parametr nesmí bıt null ani String.Empty.", "providerInvariantName");
			}

			this._connectionString = connectionString;
			this._providerFactory = DbProviderFactories.GetFactory(providerInvariantName);
		}

		/// <summary>
		/// Inicializuje instanci tøídy <see cref="DbConnector"/>.
		/// </summary>
		/// <param name="connectionStringSettings">Nastavení <see cref="ConnectionStringSettings"/> naètené z .config souboru. Napø. získané pøes ConfigurationManager.ConnectionStrings["ConnectionStringName"].</param>
		public DbConnector(ConnectionStringSettings connectionStringSettings)
		{
			if (connectionStringSettings == null)
			{
				throw new ArgumentNullException("connectionStringSettings");
			}
			if (String.IsNullOrEmpty(connectionStringSettings.ConnectionString))
			{
				throw new ArgumentException("Argument nemá nastavenu vlastnost ConnectionString.", "connectionStringSettings");
			}

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

		/**********************************************************************************************************/

		#region private CreateCommand, SetCommandDefaults
		/// <summary>
		/// Vytvoøí DbCommand dle zadanıch parametrù. Nenastavuje spojení ani jiné vlastnosti.
		/// </summary>
		/// <param name="commandText">SQL text pøíkazu</param>
		/// <param name="commandType">typ pøíkazu <see cref="CommandType"/></param>
		private DbCommand CreateCommand(string commandText, CommandType commandType)
		{
			if (String.IsNullOrEmpty(commandText))
			{
				throw new ArgumentException("commandText", "Argument nesmí bıt null ani String.Empty.");
			}

			// CommandType nemùe bıt null a není potøeba ho kontrolovat

			DbCommand cmd = this.ProviderFactory.CreateCommand();
			cmd.CommandText = commandText;
			cmd.CommandType = commandType;

			return cmd;
		}

		/// <summary>
		/// Nastaví pøíkazu default parametry (zatím pouze Connection), nejsou-li nastaveny.
		/// </summary>
		/// <remarks>
		/// Pokud jsme v transakci, pak zde sjednotíme Connection (nechápu, proè to nedìlá sám .NET Framework).
		/// </remarks>
		/// <param name="command"><see cref="DbCommand"/> k nastavení</param>
		private void SetCommandDefaults(DbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

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
		/// Vrátí novou instanci provider-specific potomka <see cref="DbConnection"/> a pokud to poadujeme, tak ji rovnou otevøe.
		/// </summary>
		/// <param name="openConnection"><c>true</c>, má-li se nová SqlConnection rovnou otevøít</param>
		public DbConnection GetConnection(bool openConnection)
		{
			DbConnection conn = this.ProviderFactory.CreateConnection();
			conn.ConnectionString = this.ConnectionString;
			if (openConnection)
			{
				conn.Open();
			}
			return conn;
		}


		/// <summary>
		/// Vrátí novou instanci provider-specific potomka <see cref="DbConnection"/>.
		/// Connection není otevøena.
		/// </summary>
		public DbConnection GetConnection()
		{
			return GetConnection(false);
		}
		#endregion

		#region ExecuteNonQuery
		/// <summary>
		/// Vykoná <see cref="DbCommand"/> a vrátí poèet dotèenıch øádek.
		/// Nejobecnìjší metoda, kterou pouívají ostatní overloady.
		/// </summary>
		/// <remarks>
		/// Není-li Connection pøíkazu nastaveno, pouije imlicitní.
		/// Není-li Connection dosud otevøeno, otevøe ho, vykoná pøíkaz a zavøe.
		/// Nemá-li poèet dotèenıch øádek smysl, vrací -1.
		/// </remarks>
		/// <param name="command"><see cref="DbCommand"/>, kterı má bıt vykonán</param>
		/// <returns>poèet dotèenıch øádek</returns>
		public int ExecuteNonQuery(DbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			SetCommandDefaults(command);

			bool mustCloseConnection = false;
			if (command.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				command.Connection.Open();
			}

			int result;
			try
			{
				result = command.ExecuteNonQuery();
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

			return result;
		}


		/// <summary>
		/// Vykoná zadanı pøíkaz urèeného typu bez parametrù. Vrátí poèet dotèenıch øádek.
		/// </summary>
		/// <param name="commandText">SQL pøíkaz</param>
		/// <param name="commandType"><see cref="CommandType"/> pøíkazu</param>
		/// <returns>poèet dotèenıch øádek</returns>
		public int ExecuteNonQuery(string commandText, CommandType commandType)
		{
			return ExecuteNonQuery(CreateCommand(commandText, commandType));
		}


		/// <summary>
		/// Vykoná zadanı pøíkaz bez parametrù. Vrátí poèet dotèenıch øádek.
		/// </summary>
		/// <remarks>
		/// Jako <see cref="CommandType"/> pouívá <see cref="CommandType.Text"/>.
		/// </remarks>
		/// <param name="commandText">SQL pøíkaz</param>
		/// <returns>poèet dotèenıch øádek</returns>
		public int ExecuteNonQuery(string commandText)
		{
			return ExecuteNonQuery(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteDataSet
		/// <summary>
		/// Vykoná <see cref="DbCommand"/> a vrátí resultset ve formì <see cref="DataSet"/>u.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevøeno, nechá ho otevøené. Není-li, otevøe si ho a zase zavøe.
		/// </remarks>
		/// <param name="command">DbCommand k vykonání</param>
		/// <returns>resultset pøíkazu ve formì <see cref="DataSet"/>u</returns>
		public DataSet ExecuteDataSet(DbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			SetCommandDefaults(command);

			using (DbDataAdapter adapter = this.ProviderFactory.CreateDataAdapter())
			{
				adapter.SelectCommand = command;

				DataSet ds = new DataSet();

				adapter.Fill(ds);

				return ds;
			}
		}


		/// <summary>
		/// Vykoná pøíkaz commandText daného commandType a vrátí resultset ve formì <see cref="DataSet"/>u.
		/// </summary>
		/// <param name="commandText">SQL pøíkaz</param>
		/// <param name="commandType">typ pøíkazu</param>
		/// <returns>resultset pøíkazu ve formì <see cref="DataSet"/>u</returns>
		public DataSet ExecuteDataSet(string commandText, CommandType commandType)
		{
			return ExecuteDataSet(CreateCommand(commandText, commandType));
		}


		/// <summary>
		/// Vykoná SQL pøíkaz cmdText typu <see cref="CommandType.Text"/> a vrátí resultset ve formì <see cref="DataSet"/>u.
		/// </summary>
		/// <param name="commandText">textovı SQL pøíkaz</param>
		/// <returns>resultset pøíkazu ve formì <see cref="DataSet"/>u</returns>
		public DataSet ExecuteDataSet(string commandText)
		{
			return ExecuteDataSet(commandText, CommandType.Text);
		}

		#endregion

		#region ExecuteDataTable
		/// <summary>
		/// Vykoná <see cref="DbCommand"/> a vrátí první resultset ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <remarks>
		/// Je-li cmd.Connection otevøeno, nechá ho otevøené. Není-li, otevøe si ho a zase zavøe.
		/// </remarks>
		/// <param name="command"><see cref="DbCommand"/> k vykonání</param>
		/// <returns>první resultset pøíkazu ve formì <see cref="System.Data.DataTable"/></returns>
		public DataTable ExecuteDataTable(DbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			SetCommandDefaults(command);

			using (DbDataAdapter adapter = this.ProviderFactory.CreateDataAdapter())
			{
				adapter.SelectCommand = command;

				DataTable dt = new DataTable();

				adapter.Fill(dt);

				return dt;
			}
		}


		/// <summary>
		/// Vykoná pøíkaz commandText typu commandType a vrátí první tabulku resultsetu ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="commandText">SQL pøíkaz</param>
		/// <param name="commandType">typ pøíkazu</param>
		/// <returns>první tabulka resultsetu vykonaného pøíkazu ve formì <see cref="System.Data.DataTable"/></returns>
		public DataTable ExecuteDataTable(string commandText, CommandType commandType)
		{
			return ExecuteDataTable(CreateCommand(commandText, commandType));
		}


		/// <summary>
		/// Vykoná pøíkaz commandText typu <see cref="CommandType.Text"/> a vrátí první tabulku resultsetu ve formì <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="commandText">textovı SQL pøíkaz</param>
		/// <returns>první tabulka resultsetu vykonaného pøíkazu ve formì <see cref="System.Data.DataTable"/></returns>
		public DataTable ExecuteDataTable(string commandText)
		{
			return ExecuteDataTable(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteReader
		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> pomocí <see cref="CommandBehavior"/> a vrátí vıslednı resultset ve formì <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="command">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">poadované "chování"</param>
		/// <returns>resultset vykonaného pøíkazu jako <see cref="DbDataReader"/></returns>
		public DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			SetCommandDefaults(command);

			bool mustCloseConnection = false;
			if (command.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				command.Connection.Open();
			}

			DbDataReader reader;

			try
			{
				if (mustCloseConnection)
				{
					// otevøeme-li si spojení sami, postaráme se i o jeho zavøení
					reader = command.ExecuteReader(behavior | CommandBehavior.CloseConnection);
				}
				else
				{
					// spojení bylo ji otevøeno, tak ho tak necháme, a se stará nadøazená aplikace
					reader = command.ExecuteReader(behavior);
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

			return reader;
		}


		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> a vrátí vıslednı resultset ve formì <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="command">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>resultset vykonaného pøíkazu jako <see cref="DbDataReader"/></returns>
		public DbDataReader ExecuteReader(DbCommand command)
		{
			return ExecuteReader(command, CommandBehavior.Default);
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná <see cref="DbCommand"/> dle zadanıch parametrù a vrátí vıslednı resultset ve formì <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="commandText">text SQL pøíkazu</param>
		/// <param name="commandType">typ pøíkazu <see cref="CommandType"/></param>
		/// <returns>resultset ve formì <see cref="DbDataReader"/>u</returns>
		public DbDataReader ExecuteReader(string commandText, CommandType commandType)
		{
			return ExecuteReader(CreateCommand(commandText, commandType));
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná <see cref="DbCommand"/> dle zadaného SQL pøíkazu typu <see cref="CommandType.Text"/>
		/// a vrátí vıslednı resultset ve formì <see cref="DbDataReader"/>u.
		/// </summary>
		/// <param name="commandText">text SQL pøíkazu</param>
		/// <returns>resultset ve formì <see cref="DbDataReader"/>u</returns>
		public DbDataReader ExecuteReader(string commandText)
		{
			return ExecuteReader(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteDataRecord
		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> pomocí <see cref="CommandBehavior"/> a vrátí první øádek první tabulky resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <param name="command">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">poadované "chování"</param>
		/// <param name="dataLoadPower"><see cref="DataLoadPower"/>, která se má pouít pro <see cref="DataRecord"/></param>
		/// <returns>první øádek první tabulky resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(DbCommand command, CommandBehavior behavior, DataLoadPower dataLoadPower)
		{
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
		/// Donastaví a vykoná <see cref="DbCommand"/> pomocí <see cref="CommandBehavior"/> a vrátí první øádek první tabulky resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> vısledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="command">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <param name="behavior">poadované "chování"</param>
		/// <returns>první øádek první tabulky resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(DbCommand command, CommandBehavior behavior)
		{
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
		/// Donastaví a vykoná <see cref="DbCommand"/> a vrátí první øádek první tabulky resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> vısledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param command="cmd">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první øádek první tabulky resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(DbCommand command)
		{
			return ExecuteDataRecord(command, CommandBehavior.Default);
		}


		/// <summary>
		/// Vytvoøí <see cref="DbCommand"/> dle zadanıch parametrù, donasataví ho a vrátí první øádek první tabulky resultsetu
		/// ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> vısledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="commandText">text SQL pøíkazu</param>
		/// <param name="commandType"><see cref="CommandType"/> SQL pøíkazu</param>
		/// <returns>první øádek první tabulky resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(string commandText, CommandType commandType)
		{
			return ExecuteDataRecord(CreateCommand(commandText, commandType));
		}


		/// <summary>
		/// Vytvoøí, nastaví a vykoná <see cref="DbCommand"/> dle zadaného SQL pøíkazu
		/// a vrátí první øádek první tabulky resultsetu ve formì <see cref="Havit.Data.DataRecord"/>. Pokud neexistuje, vrátí <c>null</c>.
		/// </summary>
		/// <remarks>
		/// <see cref="DataLoadPower"/> vısledného <see cref="DataRecord"/>u nastaví na <see cref="DataLoadPower.FullLoad"/>.
		/// </remarks>
		/// <param name="commandText">text SQL pøíkazu</param>
		/// <returns>první øádek první tabulky resultsetu ve formì <see cref="Havit.Data.DataRecord"/></returns>
		public DataRecord ExecuteDataRecord(string commandText)
		{
			return ExecuteDataRecord(commandText, CommandType.Text);
		}
		#endregion

		#region ExecuteScalar

		/// <summary>
		/// Donastaví a vykoná <see cref="DbCommand"/> a vrátí první sloupec prvního øádku první tabulky jeho resultsetu.
		/// </summary>
		/// <example>
		/// int result = (int)ExecuteScalar(cmd);
		/// </example>
		/// <param name="command">pøíkaz (nemusí mít nastaveno Connection)</param>
		/// <returns>první sloupec prvního øádku první tabulky resultsetu</returns>
		public object ExecuteScalar(DbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			SetCommandDefaults(command);

			bool mustCloseConnection = false;
			if (command.Connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				command.Connection.Open();
			}

			object result;

			try
			{
				result = command.ExecuteScalar();
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

			return result;
		}


		/// <summary>
		/// Vytvoøí ze zadanıch parametrù <see cref="DbCommand"/>, nastaví, vykoná ho a vrátí první sloupec
		/// prvního øádku první tabulky jeho resultsetu.
		/// </summary>
		/// <param name="commandText">text SQL pøíkazu</param>
		/// <param name="commandType">typ pøíkazu</param>
		/// <returns>první sloupec prvního øádku první tabulky resultsetu</returns>
		public object ExecuteScalar(string commandText, CommandType commandType)
		{
			return ExecuteScalar(CreateCommand(commandText, commandType));
		}


		/// <summary>
		/// Vytvoøí <see cref="DbCommand"/> typu <see cref="CommandType.Text"/>, vykoná ho a vrátí
		/// první sloupec prvního øádku první tabulky jeho resultsetu.
		/// </summary>
		/// <param name="commandText">text SQL pøíkazu typu <see cref="CommandType.Text"/></param>
		/// <returns>první sloupec prvního øádku resultsetu</returns>
		public object ExecuteScalar(string commandText)
		{
			return ExecuteScalar(CreateCommand(commandText, CommandType.Text));
		}

		#endregion

		#region ExecuteTransaction
		/// <summary>
		/// Vykoná poadované kroky v rámci transakce.
		/// Pokud je outerTransaction <c>null</c>, je spuštìna a commitována nová.
		/// Pokud je outerTransaction zadáno, jsou poadované kroky v rámci ní pouze vykonány, pokud se shoduje IsolationLevel.
		/// Pokud se IsolationLevel neshoduje, zaloí se nová nested-transakce s poadovanım IsolationLevelem.
		/// </summary>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají bıt souèástí transakce</param>
		/// <param name="outerTransaction">vnìjší transakce, pokud existuje; jinak <c>null</c></param>
		/// <param name="isolationLevel">poadovanı <see cref="IsolationLevel"/> transakce; pokud je <see cref="IsolationLevel.Unspecified"/>, pouije se outerTransaction, pokud je definována, nebo zaloí nová transakce s defaultním isolation-levelem</param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork, DbTransaction outerTransaction, IsolationLevel isolationLevel)
		{
			if (transactionWork == null)
			{
				throw new ArgumentNullException("transactionWork");
			}

			DbTransaction currentTransaction = outerTransaction;
			DbConnection connection;

			bool mustCloseConnection = false;
			bool mustCommitOrRollbackTransaction = false;

			if (outerTransaction == null)
			{
				// otevøení spojení, pokud není ji otevøeno
				connection = this.GetConnection();
				connection.Open();
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
					// commit chceme jen v pøípadì, e jsme sami transakci zaloili (a u úplnì novou, nebo nested)
					currentTransaction.Commit();
				}
			}
			catch
			{
				try
				{
					if (mustCommitOrRollbackTransaction)
					{
						// rollback chceme taky jen v pøípadì, e jsme sami transakci zaloili (a u úplnì novou, nebo nested)
						// pøípadné vnìjší transakce shodí naše vıjimka
						currentTransaction.Rollback();
					}
				}
				catch
				{
					// NOOP: chceme vyhodit vìcnou vıjimku, ne problém s rollbackem
				}
				throw;
			}
			finally
			{
				if (mustCloseConnection)
				{
					// uzavøení spojení, pokud jsme iniciátory transakce
					connection.Close();
				}
			}
		}

		/// <summary>
		/// Vykoná poadované kroky v rámci transakce.
		/// Pokud je outerTransaction <c>null</c>, je spuštìna a commitována nová.
		/// Pokud je outerTransaction zadáno, jsou poadované kroky v rámci ní pouze vykonány.
		/// </summary>
		/// <remarks>
		/// Pokud je outerTransaction <c>null</c>, pak zaloí novou transakci s danım IsolationLevelem.
		/// Pokud je outerTransakce zadána, pak se pro zadanı transactionWork pouije zadanı IsolationLevel a pak ho vrátí na pùvodní hodnotu.
		/// </remarks>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají bıt souèástí transakce</param>
		/// <param name="outerTransaction">vnìjší transakce, pokud existuje; jinak <c>null</c></param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork, DbTransaction outerTransaction)
		{
			ExecuteTransaction(transactionWork, outerTransaction, IsolationLevel.Unspecified);
		}

		/// <summary>
		/// Vykoná poadované kroky v rámci transakce s danım isolation-levelem.
		/// </summary>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají bıt souèástí transakce</param>
		/// <param name="isolationLevel">poadovanı <see cref="IsolationLevel"/> transakce</param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork, IsolationLevel isolationLevel)
		{
			ExecuteTransaction(transactionWork, null, isolationLevel);
		}


		/// <summary>
		/// Vykoná poadované kroky v rámci transakce.
		/// Je spuštìna a commitována nová samostatná transakce.
		/// </summary>
		/// <param name="transactionWork"><see cref="DbTransactionDelegate"/> reprezentující s úkony, které mají bıt souèástí transakce</param>
		public void ExecuteTransaction(DbTransactionDelegate transactionWork)
		{
			ExecuteTransaction(transactionWork, null, IsolationLevel.Unspecified);
		}
		#endregion

		/**********************************************************************************************************/

		#region Default (static)
		/// <summary>
		/// Defaultní <see cref="DbConnector"/>. Pokud není nastaven ruènì, pak se vytvoøí pøi prvním pøístupu z defaultního connection-stringu naèteného z .config souboru.
		/// Nastavením na null mohu pro pøíští pøístup vynutit opìtovnou inicializaci z .config souboru.
		/// </summary>
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
		public static DbConnector Default
		{
			get
			{
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
		/// Vrátí <see cref="DbConnection"/> inicializovanı defaulty z .config souboru.
		/// </summary>
		/// <remarks>Viz vlastnost <see cref="DbConnector.Default"/>.</remarks>
		private static DbConnector GetDbConnectorFromDefaultConfig()
		{
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
					throw new InvalidOperationException("Z konfiguraèního souboru se nepodaøilo naèíst defaultní parametry pøipojení k databázi.");
				}
				return new DbConnector(appSettingsConnectionString, "System.Data.SqlClient");
			}
		}
		#endregion

	}
}

