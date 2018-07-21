using System;
using System.Diagnostics;
using Havit.Business.BusinessLayerGenerator.CommandLine.Utility;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using GeneratorSettings = Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings.GeneratorSettings;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator
{
	internal static class Program
	{
		internal static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// zjistíme parametry příkazové řádky
			Arguments commandLineArguments = new Arguments(args);

			// pokud jsou parametry špatně, oznámíme a končíme
			if (commandLineArguments["sqlserver"] == null || commandLineArguments["database"] == null || commandLineArguments["solutionpath"] == null)
			{
				ConsoleHelper.WriteLineError(@"BusinessLayerToEntityFrameworkGenerator.exe -sqlserver:<server> -database:<database> [-username:<username>] [-password:<password>] -solutionpath:<outputpath>");
				return;
			}

			// nastavíme GeneratorSettings na základě parametrù příkazové řádky
			BusinessLayerGenerator.Settings.GeneratorSettings.SqlServerName = GeneratorSettings.SqlServerName = commandLineArguments["sqlserver"];
			GeneratorSettings.Username = commandLineArguments["username"];
			GeneratorSettings.Password = commandLineArguments["password"];
			BusinessLayerGenerator.Settings.GeneratorSettings.DatabaseName = GeneratorSettings.DatabaseName = commandLineArguments["database"];
			BusinessLayerGenerator.Settings.GeneratorSettings.Strategy = GeneratorStrategy.HavitCodeFirst;
			GeneratorSettings.SolutionPath = commandLineArguments["solutionpath"];
			GeneratorSettings.Namespace = commandLineArguments["namespace"];

			CsprojFile modelCsprojFile = CsprojFile.GetByFolder(System.IO.Path.Combine(GeneratorSettings.SolutionPath, "Model"), "HavitBusinessLayerToEntityFrameworkGenerator");
			CsprojFile entityCsprojFile = CsprojFile.GetByFolder(System.IO.Path.Combine(GeneratorSettings.SolutionPath, "Entity"), "HavitBusinessLayerToEntityFrameworkGenerator");
			SourceControlClient sourceControlClient = SourceControlClient.GetByFolder(GeneratorSettings.SolutionPath);

			ConsoleHelper.WriteLineInfo("Business Layer To Entity Framework Generator");
			ConsoleHelper.WriteLineInfo("Sql Server: {0}", GeneratorSettings.SqlServerName);
			ConsoleHelper.WriteLineInfo("Database: {0}", GeneratorSettings.DatabaseName);
			ConsoleHelper.WriteLineInfo("Solution Path: {0}", GeneratorSettings.SolutionPath);
			ConsoleHelper.WriteLineInfo("Model CSPROJ File: {0}", (modelCsprojFile != null) ? System.IO.Path.GetFileName(modelCsprojFile.Filename) : "");
			ConsoleHelper.WriteLineInfo("Entity CSPROJ File: {0}", (entityCsprojFile != null) ? System.IO.Path.GetFileName(entityCsprojFile.Filename) : "");
			ConsoleHelper.WriteLineInfo("Workspace: {0}", (sourceControlClient != null) ? sourceControlClient.WorkspaceName : "");
			ConsoleHelper.WriteLineInfo("");

			// připojíme se k databázi
			ServerConnection connection;
			if (String.IsNullOrEmpty(GeneratorSettings.Username))
			{
				connection = new ServerConnection(GeneratorSettings.SqlServerName);
			}
			else
			{
				connection = new ServerConnection(GeneratorSettings.SqlServerName, GeneratorSettings.Username, GeneratorSettings.Password);
			}
			connection.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
			connection.ApplicationName = "BusinessLayerGenerator";
			connection.Connect();

			Server sqlServer = new Server(connection);
			Database database = sqlServer.Databases[GeneratorSettings.DatabaseName];

			// pokud jsme databázi nenašli, oznámíme a konříme
			if (database == null)
			{
				ConsoleHelper.WriteLineError("Databáze nebyla nalezena.");
				return;
			}

			// přednačteme obsah databáze pro zrychlení vlastního generování
			ConsoleHelper.WriteLineInfo("Načítám strukturu databáze");
			database.Parent.SetDefaultInitFields(true);

			//database.PrefetchObjects(); // zbytecne
			//database.PrefetchObjects(typeof(Table));
			//database.PrefetchObjects(typeof(StoredProcedure));

			try
			{
				DatabaseHelper.Database = database;

				// vygenerujeme, co je potřeba
				Generators.EfCoreGenerator.Generate(database, modelCsprojFile, entityCsprojFile, sourceControlClient);

			}
			catch (ApplicationException e)
			{
				ConsoleHelper.WriteLineError(e.Message);
			}
			catch (Exception e)
			{
				ConsoleHelper.WriteLineError(e.ToString());
			}

			connection.Disconnect();

			if (modelCsprojFile != null)
			{
				modelCsprojFile.RemoveUnusedGeneratedFiles();
				modelCsprojFile.SaveChanges();
			}

			if (entityCsprojFile != null)
			{
				entityCsprojFile.RemoveUnusedGeneratedFiles();
				entityCsprojFile.SaveChanges();
			}
			stopwatch.Stop();

			// pokud jsme o to byli žádání z příkazové řádky, počkáme na stisk klávesy
			bool keyRequiredByCommandLine = commandLineArguments["key"] == "true";
			bool keyRequiredByWarningsAndErrors = (ConsoleHelper.WarningCount > 0) || (ConsoleHelper.ErrorCount > 0);

			if ((ConsoleHelper.WarningCount > 0) && (ConsoleHelper.ErrorCount == 0))
			{
				// Pozor na sideeffect - voláním metody WriteLineWarning zvyšujeme čítač ConsoleHelper.WarningCount!
				ConsoleHelper.WriteLineWarning(String.Format("Výpis obsahuje {0} upozornění.", ConsoleHelper.WarningCount));
			}
			ConsoleHelper.WriteLineInfo("Hotovo.");
#if DEBUG
			ConsoleHelper.WriteLineInfo(stopwatch.Elapsed.ToString());
#endif

			if (keyRequiredByCommandLine || keyRequiredByWarningsAndErrors)
			{
				Console.ReadKey();
			}

			ConsoleHelper.WriteLineInfo("");

		}
	}
}
