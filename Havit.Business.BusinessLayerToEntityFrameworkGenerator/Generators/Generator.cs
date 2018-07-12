using System;
using System.Collections.Generic;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
	public static class Generator
	{
		#region Generate
		public static void Generate(Database database, CsprojFile modelCsprojFile, CsprojFile entityCsprojFile, SourceControlClient sourceControlClient)
		{
			// nalezneme tabulky, na jejichž základě se budou generovat třídy
			Console.BufferHeight = Int16.MaxValue - 1;

			ConsoleHelper.WriteLineInfo("Vyhledávám tabulky");
			List<Table> tables = DatabaseHelper.GetWorkingTables();

			foreach (Table table in tables)
			{
				ConsoleHelper.WriteLineInfo(table.Name);

				//if (!TableHelper.IsJoinTable(table))
				{
					EfCore.ModelClass.Generate(table, modelCsprojFile, sourceControlClient);
					EfCore.EntityTypeConfigurationClass.Generate(table, entityCsprojFile, sourceControlClient);
				}
			}
		}
		#endregion
	}
}
