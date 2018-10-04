using System;
using System.Diagnostics;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator
{
	internal static class Program
	{
		#region Main
		private static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// zjistíme parametry příkazové řádky
			CommandLine.Utility.Arguments commandLineArguments = new CommandLine.Utility.Arguments(args);

			// pokud jsou parametry špatně, oznámíme a končíme
			if (commandLineArguments["sqlserver"] == null || commandLineArguments["database"] == null || commandLineArguments["outputpath"] == null)
			{
				ConsoleHelper.WriteLineError(@"BusinessLayerGenerator.exe -sqlserver:<server> -database:<database> [-username:<username>] [-password:<password>] -outputpath:<outputpath> [-namespace:<namespace>] [-strategy=Havit|Exec] [-targetplatform=SqlServer2008|SqlServer2005|SqlServerCe35] [-key:true] [-table:string]");
				return;
			}

			// nastavíme GeneratorSettings na základě parametrù příkazové řádky
			GeneratorSettings.SqlServerName = commandLineArguments["sqlserver"];
			GeneratorSettings.Username = commandLineArguments["username"];
			GeneratorSettings.Password = commandLineArguments["password"];
			GeneratorSettings.DatabaseName = commandLineArguments["database"];
			GeneratorSettings.OutputPath = commandLineArguments["outputpath"];
			GeneratorSettings.Namespace = commandLineArguments["namespace"];
			GeneratorSettings.TableName = commandLineArguments["table"];

			if (!String.IsNullOrEmpty(commandLineArguments["strategy"]))
			{
				GeneratorSettings.Strategy = (GeneratorStrategy)Enum.Parse(typeof(GeneratorStrategy), commandLineArguments["strategy"], true);
			}

			if (!String.IsNullOrEmpty(commandLineArguments["targetplatform"]))
			{
				GeneratorSettings.TargetPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), commandLineArguments["targetplatform"], true);
			}

			if (!String.IsNullOrEmpty(commandLineArguments["collectionsavestrategy"]))
			{
				ConsoleHelper.WriteLineWarning("Parametr CollectionSaveStrategy byl zrušen.");
			}

			CsprojFile csprojFile = CsprojFile.GetByFolder(GeneratorSettings.OutputPath);
			SourceControlClient sourceControlClient = SourceControlClient.GetByFolder(GeneratorSettings.OutputPath);
		
			ConsoleHelper.WriteLineInfo("Business Layer Generator");
			ConsoleHelper.WriteLineInfo("Sql Server: {0}", GeneratorSettings.SqlServerName);
			ConsoleHelper.WriteLineInfo("Database: {0}", GeneratorSettings.DatabaseName);
			if (!String.IsNullOrEmpty(GeneratorSettings.TableName))
			{
				ConsoleHelper.WriteLineInfo("Table: {0}", GeneratorSettings.TableName);
			}
			ConsoleHelper.WriteLineInfo("Target Platform: {0}", GeneratorSettings.TargetPlatform);
			ConsoleHelper.WriteLineInfo("Strategy: {0}", GeneratorSettings.Strategy);
			ConsoleHelper.WriteLineInfo("Output Path: {0}", GeneratorSettings.OutputPath);
			ConsoleHelper.WriteLineInfo("CSPROJ File: {0}", (csprojFile != null) ? System.IO.Path.GetFileName(csprojFile.Filename) : "");
			ConsoleHelper.WriteLineInfo("Workspace: {0}", (sourceControlClient != null) ? sourceControlClient.WorkspaceName : "");
			ConsoleHelper.WriteLineInfo("");

			if (GeneratorSettings.TargetPlatform == TargetPlatform.SqlServer2005)
			{
				ConsoleHelper.WriteLineError("Target Platform SqlServer2005 již není podporován.");
				return;				
			}

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
            // prefetch Table + Column -> 56 sec
            // prefetch Column -> 29 sec
            // žádný prefetch -> 36 sec
		    sqlServer.SetDefaultInitFields(typeof(Column), true);

            Database database = sqlServer.Databases[GeneratorSettings.DatabaseName];

			// pokud jsme databázi nenašli, oznámíme a konříme
			if (database == null)
			{
				ConsoleHelper.WriteLineError("Databáze nebyla nalezena.");
				return;
			}

			database.Parent.SetDefaultInitFields(true);

            try
            {
				DatabaseHelper.Database = database;

				// vygenerujeme, co je potřeba
				Generators.Generator.Generate(database, csprojFile, sourceControlClient);

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

			if (csprojFile != null)
			{
				if (String.IsNullOrEmpty(GeneratorSettings.TableName))
				{
					csprojFile.RemoveUnusedGeneratedFiles();
				}
				csprojFile.SaveChanges();
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
		#endregion
	}
}
