using Havit.Data.Entity.Patterns.Transactions.Internal;
using Havit.Data.Patterns.DataSeeds;

namespace Havit.Data.Entity.Patterns.DataSeeds;

/// <inheritdoc />
public class DbDataSeedRunner : DataSeedRunner
{
	private readonly ITransactionWrapper transactionWrapper;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbDataSeedRunner(IEnumerable<IDataSeed> dataSeeds, IDataSeedRunDecision dataSeedRunDecision, IDataSeedPersisterFactory dataSeedPersisterFactory, ITransactionWrapper transactionWrapper) : base(dataSeeds, dataSeedRunDecision, dataSeedPersisterFactory)
	{
		this.transactionWrapper = transactionWrapper;
	}

	/// <inheritdoc />
	public override void SeedData(Type dataSeedProfileType, bool forceRun = false)
	{
		transactionWrapper.ExecuteWithTransaction(() =>
		{
			base.SeedData(dataSeedProfileType, forceRun);
		});
	}
}
