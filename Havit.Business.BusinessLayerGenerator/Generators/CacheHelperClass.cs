﻿using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class CacheHelperClass
{
	public static void Generate(List<Table> tables, CsprojFile csprojFile)
	{
		ConsoleHelper.WriteLineInfo("CacheHelper");

		string filename = FileHelper.GetFilename("", "CacheHelper", ".cs", FileHelper.GeneratedFolder);

		if (csprojFile != null)
		{
			csprojFile.Ensures(filename);
		}

		CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(filename));

		Autogenerated.WriteAutogeneratedNoCodeHere(writer);

		BusinessObjectUsings.WriteUsings(writer);

		writer.WriteLine("namespace " + NamespaceHelper.GetDefaultNamespace());
		writer.WriteLine("{");

		writer.WriteCommentSummary("Pomocné metody pro práci s cache.");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine("public static partial class CacheHelper");
		writer.WriteLine("{");

		//writer.WriteOpenRegion("PreloadCachedObjects");
		//writer.WriteCommentSummary("Načte do cache všechny cachované objekty.");

		//writer.WriteLine("public static void PreloadCachedObjects()");
		//writer.WriteLine("{");

		//foreach (Table table in tables)
		//{
		//	if (TableHelper.IsCachable(table))
		//	{
		//		writer.WriteLine(String.Format("{0}.GetAll();", ClassHelper.GetClassFullName(table)));
		//	}
		//}
		//writer.WriteLine("}");
		//writer.WriteCloseRegion();

		writer.WriteOpenRegion("PreloadCachedObjectsAsync");
		writer.WriteCommentSummary("Asynchronně načte do cache všechny cachované objekty.");
		writer.WriteLine("public static void PreloadCachedObjectsAsync()");
		writer.WriteLine("{");
		writer.WriteLine("System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(PreloadCachedObjectsAsync_DoWork));");
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCommentSummary("Asynchronně načte do cache všechny cachované objekty.");
		writer.WriteLine("private static void PreloadCachedObjectsAsync_DoWork(object state)");
		writer.WriteLine("{");
		writer.WriteLine("using (new Havit.Business.IdentityMapScope())");
		writer.WriteLine("{");
		writer.WriteLine("PreloadCachedObjectsAsyncStarting();");
		writer.WriteLine();

		foreach (Table table in tables)
		{
			if (TableHelper.IsCachable(table) // jen pro cachované tabulky
				&& (!ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "Cache_SuppressPreload", table.Name).GetValueOrDefault(false)) // které nemají potlačen preload
				&& (!(LocalizationHelper.IsLocalizationTable(table) /* jde o lokalizační tabulku */ && TableHelper.GetGetAllIncludeLocalizations(LocalizationHelper.GetLocalizationParentTable(table)) /* parent tabulka má načíst lokalizace */)) // a není třeba načítat lokalizace tabulky, která je lokalizovaná a přednačítá lokalizace - její GetAll obsahuje načtení lokalizací (jinak bychom načítali lokalizace 2x)
			)
			{
				writer.WriteLine("try");
				writer.WriteLine("{");
				writer.WriteLine(String.Format("{0}.GetAll();", ClassHelper.GetClassFullName(table)));
				writer.WriteLine("}");
				writer.WriteLine("catch (SqlException)");
				writer.WriteLine("{");
				writer.WriteLine("}");
				writer.WriteLine();
			}
		}

		writer.WriteLine("PreloadCachedObjectsAsyncCompleted();");

		writer.WriteLine("}");
		writer.WriteLine("}");
		writer.WriteCloseRegion();

		writer.WriteLine("static partial void PreloadCachedObjectsAsyncStarting();"); // class			
		writer.WriteLine("static partial void PreloadCachedObjectsAsyncCompleted();"); // class			
		writer.WriteLine();

		Autogenerated.WriteAutogeneratedNoCodeHere(writer);

		writer.WriteLine("}"); // class			

		writer.WriteLine("}"); // namespace

		writer.Save();
	}
}