using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Tests.Localizations.Model;

namespace Havit.Data.Patterns.Tests.DataSeeds;

[TestClass]
public class DataSeedForTests
{
	[TestMethod]
	public void DataSeedFor_AndForAll_LocalizationsSharesConfigurationWithDefault()
	{
		// Arrange
		var dataSeedForLocalizedEntity = new DataSeedFor<LocalizedEntity>(new LocalizedEntity[] { });

		// počet výchozích konfigurací (očekává se jedna)
		int defaultChildDataForRegistryCount = dataSeedForLocalizedEntity._childDataForsRegistry.Count;

		// Act			
		dataSeedForLocalizedEntity.AndForAll(le1 => le1.Localizations, configuration => { /* NOOP */ }); // zkusíme použít konfiguraci pro lokalizace, kterou očekáváme ve výchozích konfiguracích

		// Assert
		Assert.HasCount(defaultChildDataForRegistryCount, dataSeedForLocalizedEntity._childDataForsRegistry, "Došlo k přidání další konfigurace."); // nechceme, aby Act změnil počet konfigurací
	}

	[TestMethod]
	public void DataSeedFor_AndForAll_ReusesConfigurationForSameProperties()
	{
		// Arrange
		IDataSeedFor<LocalizedEntityLocalization> firstDataSeedForLocalization = null;
		IDataSeedFor<LocalizedEntityLocalization> secondDataSeedForLocalization = null;

		var dataSeedForLocalizedEntity = new DataSeedFor<LocalizedEntity>(new LocalizedEntity[] { });

		// Act
		dataSeedForLocalizedEntity.AndForAll(le1 => le1.Localizations, dataSeedForLocalization1 => { firstDataSeedForLocalization = dataSeedForLocalization1; });
		dataSeedForLocalizedEntity.AndForAll(le2 => le2.Localizations, dataSeedForLocalization2 => { secondDataSeedForLocalization = dataSeedForLocalization2; });

		// Assert
		Assert.IsNotNull(firstDataSeedForLocalization);
		Assert.IsNotNull(secondDataSeedForLocalization);

		Assert.IsTrue(Object.ReferenceEquals(firstDataSeedForLocalization, secondDataSeedForLocalization));
	}

	[TestMethod]
	public void DataSeedFor_AndFor_ReusesConfigurationForSameProperties()
	{
		// Arrange
		IDataSeedFor<Language> firstDataSeedForLanguage = null;
		IDataSeedFor<Language> secondDataSeedForLanguage = null;

		var dataSeedForLocalizedEntityLocalization = new DataSeedFor<LocalizedEntityLocalization>(new LocalizedEntityLocalization[] { });

		// Act
		dataSeedForLocalizedEntityLocalization.AndFor(l1 => l1.Language, dataSeedForLanguage1 => { firstDataSeedForLanguage = dataSeedForLanguage1; });
		dataSeedForLocalizedEntityLocalization.AndFor(l2 => l2.Language, dataSeedForLanguage2 => { secondDataSeedForLanguage = dataSeedForLanguage2; });

		// Assert
		Assert.IsNotNull(firstDataSeedForLanguage);
		Assert.IsNotNull(secondDataSeedForLanguage);

		Assert.IsTrue(Object.ReferenceEquals(firstDataSeedForLanguage, secondDataSeedForLanguage));
	}

	[TestMethod]
	public void DataSeedFor_AndFor_SupportsMultipleAndForMethod()
	{
		// Arrange
		var dataSeedForLocalizedEntityLocalization = new DataSeedFor<LocalizedEntityLocalization>(new LocalizedEntityLocalization[] { });

		// Act
		dataSeedForLocalizedEntityLocalization.AndFor(l => l.Language, cf => { /* NOOP */ });
		dataSeedForLocalizedEntityLocalization.AndFor(l => l.Parent, cf => { /* NOOP */ });

		// Assert
		// no exception was throws
	}

}
