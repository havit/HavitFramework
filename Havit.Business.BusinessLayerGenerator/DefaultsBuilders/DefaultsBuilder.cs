using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.DefaultsBuilders
{
	public static class DefaultsBuilder
	{
		#region CreateDefaults
		/// <summary>
		/// Vytvoří výchozí hodnoty k tabulce.
		/// </summary>
		public static void CreateDefaults(Table table)
		{
			if (GeneratorSettings.Strategy != GeneratorStrategy.Havit)
			{
				return;
			}

			foreach (Column column in TableHelper.GetDbReadWriteColumns(table))
			{
				if (PropertyHelper.IsString(column) && (column.DefaultConstraint == null) && String.IsNullOrEmpty(column.Default))
				{
					column.AddDefaultConstraint();
					column.DefaultConstraint.Text = "('')";
					column.DefaultConstraint.Create();
				}
			}
		}
		#endregion

	}
}
