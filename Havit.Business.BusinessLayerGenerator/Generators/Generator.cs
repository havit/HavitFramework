using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class Generator
{
	public static void Generate(Database database, CsprojFile csprojFile)
	{
		DatabaseRulesChecker.CheckRules(database);

		// nalezneme tabulky, na jejichž základě se budou generovat třídy

		ConsoleHelper.WriteLineInfo("Vyhledávám tabulky");
		List<Table> tables = DatabaseHelper.GetWorkingTables();

		ConsoleHelper.WriteLineInfo("Generuji kód");

		foreach (Table table in tables)
		{
			if (String.IsNullOrEmpty(GeneratorSettings.TableName) || (String.Compare(GeneratorSettings.TableName, table.Name, StringComparison.CurrentCultureIgnoreCase) == 0))
			{
				ConsoleHelper.WriteLineInfo(table.Name);

				if (!TableHelper.IsJoinTable(table))
				{
					// ověříme, že má tabulka právě jeden sloupec primárního klíče
					try
					{
						TableHelper.GetPrimaryKey(table);
					}
					catch (ApplicationException)
					{
						ConsoleHelper.WriteLineWarning("Přeskakuji tabulku {0} - nemá primární klíč nebo je primární klíč složený.", table.Name);
						continue;
					}

					if (!GeneratorSettings.Strategy.IsEntityFrameworkGeneratedDatabaseStrategy())
					{
						// vygenerujeme výchozí hodnoty
						DefaultsBuilders.DefaultsBuilder.CreateDefaults(table);
					}

					// ověříme pravidla struktury databáze nad tabulkou
					RulesChecker.CheckRules(table);

					BusinessObjectBaseClass.Generate(table, csprojFile); // vygeneruje _generated\{Table}Base.cs
					BusinessObjectPartialClass.Generate(table, csprojFile); // vygeneruje _generated\{Table}.partial.cs
					BusinessObjectClass.Generate(table, csprojFile); // vygeneruje {Table}.cs pokud neexistuje (nepřepíše existující soubor)
					CollectionBaseClass.Generate(table, csprojFile); // vygeneruje _generated\{Table}CollectionBase.cs
					CollectionPartialClass.Generate(table, csprojFile); // vygeneruje _generated\{Table}Collection.partial.cs
					CollectionClass.Generate(table, csprojFile); // vygeneruje {Table}Collection.cs pokud neexistuje (nepřepíše existující soubor)
					PropertiesClass.Generate(table, csprojFile); // vygeneruje _generated\{Table}Properties.cs
					PropertiesClassObsolete.RemoveObsolete(table, csprojFile); // odstraňuje soubor _properties\{Table}Properties.cs pokud neexistuje se standardním obsahem

					if (table.Name == "Currency")
					{
						MoneyBaseClass.Generate(table, csprojFile);
						MoneyClass.Generate(table, csprojFile);
						MoneyPartialClass.Generate(table, csprojFile);
					}

					if ((table.Name == "ResourceClass") && (DatabaseHelper.FindTable("ResourceItem", "dbo") != null))
					{
						DbResourcesPartialClass.Generate(table, csprojFile);
					}

				}

				if (!GeneratorSettings.Strategy.IsEntityFrameworkGeneratedDatabaseStrategy())
				{
					// vygenerujeme FK pro cizí klíče
					// nebo pro kolekce...
					IndexBuilders.IndexesBuilder.CreateIndexes(table);
					ConstraintBuilders.ConstraintBuilder.CreateConstraints(table);
				}
			}
		}

		if (String.IsNullOrEmpty(GeneratorSettings.TableName))
		{
			ExtensionMethodsClass.Generate(tables, csprojFile);
			CacheHelperClass.Generate(tables, csprojFile);
		}

	}
}
