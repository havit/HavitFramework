using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator;

internal static class Program
{
	private static void Main(string[] args)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		// zjistíme parametry příkazové řádky
		CommandLine.Utility.Arguments commandLineArguments = new CommandLine.Utility.Arguments(args);

		// pokud jsou parametry špatně, oznámíme a končíme
		var missingConnectionParameters = commandLineArguments["sqlserver"] == null || commandLineArguments["database"] == null;
		var missingWebConfigPath = commandLineArguments["webconfig"] == null;
		if ((missingConnectionParameters && missingWebConfigPath) || commandLineArguments["outputpath"] == null)
		{
			ConsoleHelper.WriteLineError("Missing connection parameters: either webconfig or sqlserver&database not specified... or output path is missing");
			ConsoleHelper.WriteLineError(@"BusinessLayerGenerator.exe [-webconfig:<webconfig> | -sqlserver:<server> -database:<database> [-username:<username>] [-password:<password>]] -outputpath:<outputpath> [-namespace:<namespace>] [-strategy=Havit|Exec] [-targetplatform=SqlServer2008|SqlServer2005|SqlServerCe35] [-systemmemoryspansupported:false] [-key:true] [-table:string]");
			return;
		}

		ConnectionParameters connectionParameters;
		if (commandLineArguments["webconfig"] != null)
		{
			if (commandLineArguments["sqlserver"] != null)
			{
				ConsoleHelper.WriteLineError("Invalid connection parameters: both webconfig and sqlserver&database is specified");
                    ConsoleHelper.WriteLineError(@"BusinessLayerGenerator.exe [-webconfig:<webconfig> | -sqlserver:<server> -database:<database> [-username:<username>] [-password:<password>]] -outputpath:<outputpath> [-namespace:<namespace>] [-strategy=Havit|Exec] [-targetplatform=SqlServer2008|SqlServer2005|SqlServerCe35] [-systemmemoryspansupported:false] [-key:true] [-table:string]");
				return;
			}

			try
			{
				connectionParameters = ConnectionParametersParser.ParseParametersFromWebConfig(commandLineArguments["webconfig"]);
			}
			catch (Exception e)
			{
				ConsoleHelper.WriteLineError(e.ToString());
				return;
			}
		}
		else
		{
			connectionParameters = new ConnectionParameters
			{
				ServerName = commandLineArguments["sqlserver"],
				Username = commandLineArguments["username"],
				Password = commandLineArguments["password"],
				DatabaseName = commandLineArguments["database"]
			};
		}

		// nastavíme GeneratorSettings na základě parametrù příkazové řádky
		GeneratorSettings.SqlServerName = connectionParameters.ServerName;
		GeneratorSettings.Username = connectionParameters.Username;
		GeneratorSettings.Password = connectionParameters.Password;
		GeneratorSettings.DatabaseName = connectionParameters.DatabaseName;
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

            if (!String.IsNullOrEmpty(commandLineArguments["systemmemoryspansupported"]))
            {
                GeneratorSettings.SystemMemorySpanSupported = bool.Parse(commandLineArguments["systemmemoryspansupported"]);
            }


            if (!String.IsNullOrEmpty(commandLineArguments["collectionsavestrategy"]))
		{
			ConsoleHelper.WriteLineWarning("Parametr CollectionSaveStrategy byl zrušen.");
		}

		CsprojFile csprojFile = new CsprojFileFactory().GetByFolder(GeneratorSettings.OutputPath, "HavitBusinessLayerGenerator");

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
		ConsoleHelper.WriteLineInfo("");

		if (GeneratorSettings.TargetPlatform == TargetPlatform.SqlServer2005)
		{
			ConsoleHelper.WriteLineError("Target Platform SqlServer2005 již není podporován.");
			return;				
		}

		if (GeneratorSettings.TargetPlatform == TargetPlatform.SqlServerCe35)
		{
			ConsoleHelper.WriteLineError("Target Platform SqlServerCe35 již není podporován.");
			return;
		}

		// připojíme se k databázi
		ServerConnection connection;
		SqlConnection sqlConnection;
		if (String.IsNullOrEmpty(GeneratorSettings.Username))
		{
			connection = new ServerConnection(GeneratorSettings.SqlServerName);
			sqlConnection = new SqlConnection($"Data Source={GeneratorSettings.SqlServerName};Initial Catalog={GeneratorSettings.DatabaseName};Integrated Security=SSPI");
		}
		else
		{
			connection = new ServerConnection(GeneratorSettings.SqlServerName, GeneratorSettings.Username, GeneratorSettings.Password);
			sqlConnection = new SqlConnection($"Data Source={GeneratorSettings.SqlServerName};Initial Catalog={GeneratorSettings.DatabaseName};User ID={GeneratorSettings.Username};Pwd={GeneratorSettings.Password}");
		}
		connection.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;			
		connection.ApplicationName = "BusinessLayerGenerator";
		connection.Connect();
		sqlConnection.Open();

		Server sqlServer = new Server(connection);
            // prefetch Table + Column -> 56 sec
            // prefetch Column -> 29 sec
            // žádný prefetch -> 36 sec		    
		sqlServer.SetDefaultInitFields(typeof(Column), true);
		sqlServer.SetDefaultInitFields(typeof(ForeignKey), true);
		
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
			ConnectionHelper.SqlConnection = sqlConnection;

			// vygenerujeme, co je potřeba
			Generators.Generator.Generate(database, csprojFile);

		}
		catch (ApplicationException e)
		{
			ConsoleHelper.WriteLineError(e.Message);
		}
		catch (Exception e)
		{
			ConsoleHelper.WriteLineError(e.ToString());
		}

		sqlConnection.Close();
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
}
