using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Havit.Data.EntityFrameworkCore.Threading;

/// <inheritdoc />
public class DbLockedCriticalSection : IDbLockedCriticalSection
{
	private const int LockTimeoutMilliseconds = 10 * 60 * 1000 /* ms*/; // 10 minut - maximální doba čekání na získání zámku
	private const int GetLockCommandTimeoutSeconds = (LockTimeoutMilliseconds / 1000) + 30 /* seconds */; // s rezervou nad LockTimeoutMilliseconds, aby vypršení čekání na zámek skončilo result codem Timeout (a tedy DbLockedCriticalSectionException), nikoliv SqlException z timeoutu commandu

	private readonly Func<SqlConnection> _sqlConnectionFactory;
	private readonly bool _ownsConnection;

	/// <summary>
	/// Constructor.
	/// </summary>
	public DbLockedCriticalSection(string connectionString)
	{
		_sqlConnectionFactory = () => new SqlConnection(connectionString);
		_ownsConnection = true;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public DbLockedCriticalSection(SqlConnection sqlConnection)
	{
		_sqlConnectionFactory = () => sqlConnection;
		_ownsConnection = false;
	}

	/// <inheritdoc />
	public IDisposable EnterScope(string lockValue)
	{
		SqlConnection sqlConnection = _sqlConnectionFactory();

		bool mustClose = false;
		if (sqlConnection.State != System.Data.ConnectionState.Open)
		{
			sqlConnection.Open();
			mustClose = true;
		}

		try
		{
			GetLock(lockValue, sqlConnection);
		}
		catch
		{
			// v případě chyby získání zámku zavřeme spojení
			// a to opatrně, abychom nezamaskovali výjimku v případě selhání získání zámku z důvodu nefunkčního spojení
			try
			{
				if (mustClose)
				{
					sqlConnection.Close();
				}

				if (_ownsConnection)
				{
					sqlConnection.Dispose();
				}
			}
			catch
			{
				// NOOP
			}

			throw;
		}

		return new Scope(() =>
		{
			try
			{
				ReleaseLock(lockValue, sqlConnection);
			}
			finally
			{
				try
				{
					if (mustClose)
					{
						sqlConnection.Close();
					}

					if (_ownsConnection)
					{
						sqlConnection.Dispose();
					}
				}
				catch
				{
					// NOOP
				}
			}
		});
	}

	/// <inheritdoc />
	public async Task<IAsyncDisposable> EnterScopeAsync(string lockValue, CancellationToken cancellationToken = default)
	{
		SqlConnection sqlConnection = _sqlConnectionFactory();

		bool mustClose = false;
		if (sqlConnection.State != System.Data.ConnectionState.Open)
		{
			await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
			mustClose = true;
		}

		try
		{
			await GetLockAsync(lockValue, sqlConnection, cancellationToken).ConfigureAwait(false);
		}
		catch
		{
			// v případě chyby získání zámku zavřeme spojení
			// a to opatrně, abychom nezamaskovali výjimku v případě selhání získání zámku z důvodu nefunkčního spojení
			try
			{
				if (mustClose)
				{
					await sqlConnection.CloseAsync().ConfigureAwait(false);
				}

				if (_ownsConnection)
				{
					await sqlConnection.DisposeAsync().ConfigureAwait(false);
				}
			}
			catch
			{
				// NOOP
			}

			throw;
		}

		return new AsyncScope(async () =>
		{
			try
			{
				await ReleaseLockAsync(lockValue, sqlConnection).ConfigureAwait(false); // NO CANCELLATION TOKEN (uvolnit zámek chceme bez ohledu na cancellation token)
			}
			finally
			{
				try
				{
					if (mustClose)
					{
						await sqlConnection.CloseAsync().ConfigureAwait(false);
					}

					if (_ownsConnection)
					{
						await sqlConnection.DisposeAsync().ConfigureAwait(false);
					}
				}
				catch
				{
					// NOOP
				}
			}
		});
	}

	/// <inheritdoc />
	public void ExecuteAction(string lockValue, Action criticalSection)
	{
		using (EnterScope(lockValue))
		{
			criticalSection();
		}
	}

	/// <inheritdoc />
	public async Task ExecuteActionAsync(string lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default)
	{
		IAsyncDisposable scope = await EnterScopeAsync(lockValue, cancellationToken).ConfigureAwait(false);
		await using (scope.ConfigureAwait(false))
		{
			await criticalSection().ConfigureAwait(false);
		}
	}

	private void GetLock(string lockValue, DbConnection sqlConnection)
	{
		using DbCommand sqlCommand = GetLock_PrepareCommand(lockValue, sqlConnection, out DbParameter resultCodeSqlParameter);
		sqlCommand.ExecuteNonQuery();
		GetLock_VerifyResultCode(GetLock_GetResultCode(resultCodeSqlParameter), lockValue);
	}

	private async Task GetLockAsync(string lockValue, DbConnection sqlConnection, CancellationToken cancellationToken)
	{
		using DbCommand sqlCommand = GetLock_PrepareCommand(lockValue, sqlConnection, out DbParameter resultCodeSqlParameter);
		await sqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
		GetLock_VerifyResultCode(GetLock_GetResultCode(resultCodeSqlParameter), lockValue);
	}

	private DbCommand GetLock_PrepareCommand(string lockValue, DbConnection sqlConnection, out DbParameter resultCodeSqlParameter)
	{
		DbCommand sqlCommand = sqlConnection.CreateCommand();
		sqlCommand.CommandType = System.Data.CommandType.Text;
		sqlCommand.CommandText = "EXEC @ResultCode = sp_getapplock @Resource, 'Exclusive', 'Session', @LockTimeout";

		DbParameter resourceParameter = sqlCommand.CreateParameter();
		resourceParameter.ParameterName = "@Resource";
		resourceParameter.DbType = System.Data.DbType.String;
		resourceParameter.Value = lockValue;

		DbParameter lockTimeoutParameter = sqlCommand.CreateParameter();
		lockTimeoutParameter.ParameterName = "@LockTimeout";
		lockTimeoutParameter.DbType = System.Data.DbType.Int32;
		lockTimeoutParameter.Value = LockTimeoutMilliseconds;

		resultCodeSqlParameter = sqlCommand.CreateParameter();
		resultCodeSqlParameter.ParameterName = "@ResultCode";
		resultCodeSqlParameter.DbType = System.Data.DbType.Int32;
		resultCodeSqlParameter.Direction = System.Data.ParameterDirection.Output;

		sqlCommand.Parameters.Add(resourceParameter);
		sqlCommand.Parameters.Add(lockTimeoutParameter);
		sqlCommand.Parameters.Add(resultCodeSqlParameter);

		sqlCommand.CommandTimeout = GetLockCommandTimeoutSeconds;
		return sqlCommand;
	}

	private static SpGetAppLockResultCode GetLock_GetResultCode(DbParameter resultCodeSqlParameter)
	{
		// při selhání commandu nemusí být output parametr nastaven (DBNull)
		if (resultCodeSqlParameter.Value is int resultCode)
		{
			return (SpGetAppLockResultCode)resultCode;
		}

		throw new InvalidOperationException("sp_getapplock did not return a result code.");
	}

	private void GetLock_VerifyResultCode(SpGetAppLockResultCode getAppLockResultCode, string lockValue)
	{
		switch (getAppLockResultCode)
		{
			case SpGetAppLockResultCode.Locked:
			case SpGetAppLockResultCode.LockedAfterWaiting:
				return;
			case SpGetAppLockResultCode.Timeout:
			case SpGetAppLockResultCode.Cancelled:
			case SpGetAppLockResultCode.DeadlockVictim:
			case SpGetAppLockResultCode.Error:
				throw new DbLockedCriticalSectionException($"Unable to get lock for resource '{lockValue}'. Result code: '{Enum.GetName(typeof(SpGetAppLockResultCode), getAppLockResultCode)}'");
			default:
				throw new InvalidOperationException($"Unknown SpGetAppLockResultCode: {Enum.GetName(typeof(SpGetAppLockResultCode), getAppLockResultCode)}");
		}
	}

	private void ReleaseLock(string lockValue, DbConnection sqlConnection)
	{
		if ((sqlConnection.State == System.Data.ConnectionState.Closed) || (sqlConnection.State == System.Data.ConnectionState.Broken))
		{
			// Pokud je spojení zavřené (či rozbité), nemůžeme na něm spouštět dotazy do databáze.
			// Pokud je spojení zavřené (či rozbité), jsme si jisti, že zámek byl uvolněn (zámek je vlastněn session), takže nemusíme nic dělat.
			// JK: Jak se stane, že je spojení zavřené, je mi záhadou. Stává se jen v testech na build serveru, jinak se nepodařilo zreprodukovat.
			return;
		}

		using (DbCommand command = ReleaseLock_PrepareCommand(lockValue, sqlConnection, out DbParameter resultCodeSqlParameter))
		{
			command.ExecuteNonQuery();
			ReleaseLock_VerifyResultCode(ReleaseLock_GetResultCode(resultCodeSqlParameter), lockValue);
		}
	}

	private async Task ReleaseLockAsync(string lockValue, DbConnection sqlConnection)
	{
		if ((sqlConnection.State == System.Data.ConnectionState.Closed) || (sqlConnection.State == System.Data.ConnectionState.Broken))
		{
			// Pokud je spojení zavřené (či rozbité), nemůžeme na něm spouštět dotazy do databáze.
			// Pokud je spojení zavřené (či rozbité), jsme si jisti, že zámek byl uvolněn (zámek je vlastněn session), takže nemusíme nic dělat.
			// JK: Jak se stane, že je spojení zavřené, je mi záhadou. Stává se jen v testech na build serveru, jinak se nepodařilo zreprodukovat.
			return;
		}

		using (DbCommand command = ReleaseLock_PrepareCommand(lockValue, sqlConnection, out DbParameter resultCodeSqlParameter))
		{
			await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
			ReleaseLock_VerifyResultCode(ReleaseLock_GetResultCode(resultCodeSqlParameter), lockValue);
		}
	}

	private DbCommand ReleaseLock_PrepareCommand(string lockValue, DbConnection sqlConnection, out DbParameter resultCodeSqlParameter)
	{
		DbCommand sqlCommand = sqlConnection.CreateCommand();
		sqlCommand.CommandType = System.Data.CommandType.Text;
		sqlCommand.CommandText = "EXEC @ResultCode = sp_releaseapplock @Resource, 'Session'";

		DbParameter resourceParameter = sqlCommand.CreateParameter();
		resourceParameter.ParameterName = "@Resource";
		resourceParameter.Value = lockValue;

		resultCodeSqlParameter = sqlCommand.CreateParameter();
		resultCodeSqlParameter.ParameterName = "@ResultCode";
		resultCodeSqlParameter.DbType = System.Data.DbType.Int32;
		resultCodeSqlParameter.Direction = System.Data.ParameterDirection.Output;

		sqlCommand.Parameters.Add(resourceParameter);
		sqlCommand.Parameters.Add(resultCodeSqlParameter);

		sqlCommand.CommandTimeout = 10 * 60; // 10 minut
		return sqlCommand;
	}

	private static SpReleaseAppLockResultCode ReleaseLock_GetResultCode(DbParameter resultCodeSqlParameter)
	{
		// při selhání commandu nemusí být output parametr nastaven (DBNull)
		if (resultCodeSqlParameter.Value is int resultCode)
		{
			return (SpReleaseAppLockResultCode)resultCode;
		}

		throw new InvalidOperationException("sp_releaseapplock did not return a result code.");
	}

	private void ReleaseLock_VerifyResultCode(SpReleaseAppLockResultCode releaseAppLockResultCode, string lockValue)
	{
		switch (releaseAppLockResultCode)
		{
			case SpReleaseAppLockResultCode.Released:
				return;
			case SpReleaseAppLockResultCode.Error:
				throw new DbLockedCriticalSectionException($"Unable to release lock for resource '{lockValue}'. Result code: '{Enum.GetName(typeof(SpReleaseAppLockResultCode), releaseAppLockResultCode)}'");
			default:
				throw new InvalidOperationException($"Unknown SpReleaseAppLockResultCode: {Enum.GetName(typeof(SpReleaseAppLockResultCode), releaseAppLockResultCode)}");
		}
	}

	/// <summary>
	/// Result (code) of calling sp_getapplock.
	/// </summary>
	public enum SpGetAppLockResultCode
	{
		/// <summary>
		/// The lock was successfully granted synchronously.
		/// </summary>
		Locked = 0,

		/// <summary>
		/// The lock was granted successfully after waiting for other incompatible locks to be released.
		/// </summary>
		LockedAfterWaiting = 1,

		/// <summary>
		/// The lock request timed out.
		/// </summary>
		Timeout = -1,

		/// <summary>
		/// The lock request was canceled.
		/// </summary>
		Cancelled = -2,

		/// <summary>
		/// The lock request was chosen as a deadlock victim.
		/// </summary>
		DeadlockVictim = -3,

		/// <summary>
		/// Indicates a parameter validation or other call error.
		/// </summary>
		Error = -999
	}

	/// <summary>
	/// Result (code) of calling sp_releaseapplock.
	/// </summary>
	public enum SpReleaseAppLockResultCode
	{
		/// <summary>
		/// Lock was successfully released.
		/// </summary>
		Released = 0,

		/// <summary>
		/// Indicates a parameter validation or other call error.
		/// </summary>
		Error = -999
	}

	internal class Scope : IDisposable
	{
		private readonly Action _disposeAction;

		public Scope(Action disposeAction)
		{
			_disposeAction = disposeAction;
		}

		void IDisposable.Dispose()
		{
			_disposeAction();
		}
	}

	internal class AsyncScope : IAsyncDisposable
	{
		private Func<Task> _disposeAction;

		public AsyncScope(Func<Task> disposeAction)
		{
			_disposeAction = disposeAction;
		}

		async ValueTask IAsyncDisposable.DisposeAsync()
		{
			await _disposeAction().ConfigureAwait(false);
		}
	}
}