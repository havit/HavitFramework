using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.ConstraintBuilders;

public static class DateConstraintBuilder
{
	public static void CreateConstraints(Table table)
	{
		if (GeneratorSettings.Strategy != GeneratorStrategy.Havit)
		{
			return;
		}

		if (DatabaseHelper.Database.CompatibilityLevel != Microsoft.SqlServer.Management.Smo.CompatibilityLevel.Version90)
		{
			return;
		}

		foreach (Column column in TableHelper.GetNotIgnoredColumns(table))
		{
			if (TypeHelper.IsDateTime(column))
			{
				string constraintName = String.Format("CKX_{0}_{1}_DateOnly", table.Name, column.Name);
				Check constraint = table.Checks[constraintName];

				if (column.DataType.Name == "TDate")
				{
					string constraintText = String.Format("([{0}] IS NULL OR CONVERT([float],[{0}],0)=CONVERT([int],[{0}],0))", column.Name);
					if (constraint == null)
					{
						constraint = new Check(table, constraintName);
						constraint.Text = constraintText;
						try
						{
							constraint.Create();
						}
						catch (FailedOperationException)
						{
							ConsoleHelper.WriteLineWarning("Tabulka {0} sloupec {1}: Nepodařilo se vytvořit constraint {2}.", table.Name, column.Name, constraintName);
						}
					}
					else
					{
						if (constraint.Text != constraintText)
						{
							try
							{
								constraint.Drop();
							}
							catch (FailedOperationException)
							{
								ConsoleHelper.WriteLineWarning("Tabulka {0} sloupec {1}: Nepodařilo se odstranit constraint {2}.", table.Name, column.Name, constraintName);
							}

							constraint = new Check(table, constraintName);
							constraint.Text = constraintText;
							try
							{
								constraint.Create();
							}
							catch (FailedOperationException)
							{
								ConsoleHelper.WriteLineWarning("Tabulka {0} sloupec {1}: Nepodařilo se vytvořit constraint {2}.", table.Name, column.Name, constraintName);
							}
						}
					}
				}
				else
				{
					if (constraint != null)
					{
						try
						{
							constraint.Drop();
						}
						catch (FailedOperationException)
						{
							ConsoleHelper.WriteLineWarning("Tabulka {0} sloupec {1}: Nepodařilo se odstranit constraint {2}.", table.Name, column.Name, constraintName);
						}
					}
				}
			}
		}
	}
}
