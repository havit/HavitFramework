using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Common.Specialized;

/// <summary>
/// Třída zapouzdřuje předaný DbProviverFactory a deleguje veškeré činnosti na něj.
/// Avšak metoda CreateCommand navíc nastaví vlastnost CommandTimeout na zvolenou hodnotu.
/// Metoda CreateCommandBuilder se neřeší.
/// </summary>
public class CustomCommandTimeoutWrappingDbProviderFactory : DbProviderFactory
{
	private readonly DbProviderFactory wrappedDbProviderFactory;

	private readonly int commandTimeout;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="wrappedDbProviderFactory">Zapouzdřený DbProviderFactory, na který jsou delegována volání jednotlivých metod.</param>
	/// <param name="commandTimeout">Timeout, který je nastaven metodou CreateCommand.</param>
	public CustomCommandTimeoutWrappingDbProviderFactory(DbProviderFactory wrappedDbProviderFactory, int commandTimeout)
	{
		this.wrappedDbProviderFactory = wrappedDbProviderFactory;
		this.commandTimeout = commandTimeout;
	}

	/// <inheritdoc />
	public override bool CanCreateDataSourceEnumerator
	{
		get { return wrappedDbProviderFactory.CanCreateDataSourceEnumerator; }
	}

	/// <inheritdoc />
	public override DbCommand CreateCommand()
	{
		var cmd = wrappedDbProviderFactory.CreateCommand();
		cmd.CommandTimeout = commandTimeout;
		return cmd;
	}

	/// <inheritdoc />
	public override DbCommandBuilder CreateCommandBuilder()
	{
		return wrappedDbProviderFactory.CreateCommandBuilder();
	}

	/// <inheritdoc />
	public override DbConnection CreateConnection()
	{
		return wrappedDbProviderFactory.CreateConnection();
	}

	/// <inheritdoc />
	public override DbConnectionStringBuilder CreateConnectionStringBuilder()
	{
		return wrappedDbProviderFactory.CreateConnectionStringBuilder();
	}

	/// <inheritdoc />
	public override DbDataAdapter CreateDataAdapter()
	{
		return wrappedDbProviderFactory.CreateDataAdapter();
	}

	/// <inheritdoc />
	public override DbDataSourceEnumerator CreateDataSourceEnumerator()
	{
		return wrappedDbProviderFactory.CreateDataSourceEnumerator();
	}

	/// <inheritdoc />
	public override DbParameter CreateParameter()
	{
		return wrappedDbProviderFactory.CreateParameter();
	}

#if NETFRAMEWORK
	/// <inheritdoc />
	public override CodeAccessPermission CreatePermission(PermissionState state)
	{
		return wrappedDbProviderFactory.CreatePermission(state);
	}
#endif
}
