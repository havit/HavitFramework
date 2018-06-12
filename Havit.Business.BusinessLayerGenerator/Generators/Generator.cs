using System;
using System.Collections.Generic;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class Generator
	{
		#region Generate
		public static void Generate(Database database, CsprojFile csprojFile, SourceControlClient sourceControlClient)
		{
			DatabaseRulesChecker.CheckRules(database);

			// nalezneme tabulky, na jejichž základě se budou generovat třídy
			Console.BufferHeight = Int16.MaxValue - 1;

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

						// vygenerujeme výchozí hodnoty
						DefaultsBuilders.DefaultsBuilder.CreateDefaults(table);

						// ověříme pravidla struktury databáze nad tabulkou
						RulesChecker.CheckRules(table);

						BusinessObjectBaseClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje _generated\{Table}Base.cs
						BusinessObjectPartialClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje _generated\{Table}.partial.cs
						BusinessObjectClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje {Table}.cs pokud neexistuje (nepřepíše existující soubor)
						CollectionBaseClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje _generated\{Table}CollectionBase.cs
						CollectionPartialClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje _generated\{Table}Collection.partial.cs
						CollectionClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje {Table}Collection.cs pokud neexistuje (nepřepíše existující soubor)
						PropertiesClass.Generate(table, csprojFile, sourceControlClient); // vygeneruje _generated\{Table}Properties.cs
						PropertiesClassObsolete.RemoveObsolete(table, csprojFile, sourceControlClient); // odstraňuje soubor _properties\{Table}Properties.cs pokud neexistuje se standardním obsahem

						if (table.Name == "Currency")
						{
							MoneyBaseClass.Generate(table, csprojFile, sourceControlClient);
							MoneyClass.Generate(table, csprojFile, sourceControlClient);
							MoneyPartialClass.Generate(table, csprojFile, sourceControlClient);
						}

						if ((table.Name == "ResourceClass") && (DatabaseHelper.FindTable("ResourceItem", "dbo") != null))
						{
							DbResourcesPartialClass.Generate(table, csprojFile, sourceControlClient);
						}

					}
					// vygenerujeme FK pro cizí klíče
					// nebo pro kolekce...
					IndexBuilders.IndexesBuilder.CreateIndexes(table);
					ConstraintBuilders.ConstraintBuilder.CreateConstraints(table);
				}
			}

			if (String.IsNullOrEmpty(GeneratorSettings.TableName))
			{
				ExtensionMethodsClass.Generate(tables, csprojFile, sourceControlClient);
				CacheHelperClass.Generate(tables, csprojFile, sourceControlClient);
			}

		}
		#endregion
	}
}
