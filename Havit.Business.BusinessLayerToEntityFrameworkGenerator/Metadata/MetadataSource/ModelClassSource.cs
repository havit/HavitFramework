using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata.MetadataSource
{
	public class ModelClassSource
	{
		public List<GeneratedModelClass> GetModelClasses(Database database)
		{
			ConsoleHelper.WriteLineInfo("Vyhledávám tabulky");
			var tables = database.Tables.Cast<Table>()
				.Where(t => !t.IsSystemObject)
				.OrderBy(t => t.Name, StringComparer.InvariantCultureIgnoreCase)
				.ToArray();

            // First pass: discover tables, references, etc.
			var modelClasses = new Dictionary<string, GeneratedModelClass>(tables.Length);
			foreach (Table table in tables)
            {
                if (table.Name.StartsWith("TulipReports_") || table.Name.StartsWith("__") || table.Name.StartsWith("QRTZ_"))
				{
					ConsoleHelper.WriteLineInfo("Ignoring table: {0}", table.Name);
					continue;
				}

				ConsoleHelper.WriteLineInfo(table.Name);

				CheckRules(table);

                GeneratedModelClass generatedModelClass = GetModelClass(table);
                modelClasses.Add(generatedModelClass.Name, generatedModelClass);
			}

            // Second pass: store references to target tables in collection properties
            foreach (GeneratedModelClass modelClass in modelClasses.Values)
            {
                foreach (EntityCollectionProperty collectionProperty in modelClass.CollectionProperties)
                {
                    collectionProperty.TargetClass = modelClasses[ClassHelper.GetClassName(collectionProperty.TargetTable)];
                }
            }

			return modelClasses.Values.ToList();
		}

		private void CheckRules(Table table)
		{
			if (!TableHelper.GetGenerateIndexes(table))
			{
				ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0} má vypnuto generování indexů, což není podporováno.", table.Name));
			}

			foreach (Column column in table.Columns)
			{
				if (PropertyHelper.IsString(column) && (column.DataType.SqlDataType != SqlDataType.NVarChar) && (column.DataType.SqlDataType != SqlDataType.NVarCharMax))
				{
					ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0} obsahuje sloupec {1} s nepodporovaným typem {2}.",
						table.Name, column.Name, column.DataType));
				}
			}
		}

		private static GeneratedModelClass GetModelClass(Table table)
		{
			var modelClass = new GeneratedModelClass
			{
				Table = table,
				Name = ClassHelper.GetClassName(table),
                Namespace = Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Model")
            };

			DiscoverPrimaryKeys(modelClass);
			DiscoverProperties(modelClass);
			DiscoverCollections(modelClass);

			return modelClass;
		}

		private static void DiscoverPrimaryKeys(GeneratedModelClass modelClass)
		{
			if (TableHelper.IsJoinTable(modelClass.Table))
			{
				foreach (Column column in modelClass.Table.Columns.Cast<Column>().Where(c => c.InPrimaryKey))
				{
					Column referencedColumn = ColumnHelper.GetReferencedColumn(column);
					var pk = new EntityPrimaryKeyPart
					{
						Property = new EntityProperty
						{
							Column = column,
							TypeName = TypeHelper.GetFieldSystemTypeName(referencedColumn)
						}
					};

				    if (column.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
				    {
				        pk.Property.Name = column.Name.Substring(0, column.Name.Length - 2) + "Id";
				    }
				    else
				    {
				        pk.Property.Name = $"{ColumnHelper.GetReferencedTable(column).Name}Id";
				    }
                    var fk = new EntityForeignKey
					{
						Column = column,
						ForeignKeyProperty = pk.Property,
						NavigationProperty = new EntityProperty
						{
							Column = column,
							TypeName = TypeHelper.GetPropertyTypeName(column).Replace("BusinessLayer", "Model")
						},
					};
				    if (column.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
				    {
				        fk.NavigationProperty.Name = column.Name.Substring(0, column.Name.Length - 2);
				    }
				    else
				    {
				        fk.NavigationProperty.Name = ColumnHelper.GetReferencedTable(column).Name;
				    }

				    modelClass.PrimaryKeyParts.Add(pk);
					modelClass.ForeignKeys.Add(fk);
					modelClass.Properties.Add(pk.Property);
				}
			}
			else
			{
				var pk = new EntityPrimaryKeyPart
				{
					Property = new EntityProperty
					{
						Column = TableHelper.GetPrimaryKey(modelClass.Table),
						Name = "Id",
					}
				};
				pk.Property.TypeName = TypeHelper.GetFieldSystemTypeName(pk.Property.Column);
				modelClass.PrimaryKeyParts.Add(pk);
				modelClass.Properties.Add(pk.Property);
			}
		}

		private static void DiscoverProperties(GeneratedModelClass modelClass)
		{
			Table table = modelClass.Table;

			foreach (Column column in table.Columns.Cast<Column>().Where(c => !c.InPrimaryKey))
			{
				var entityProperty = new EntityProperty
				{
					Column = column,
					Name = PropertyHelper.GetPropertyName(column, "Id")
				};
				modelClass.Properties.Add(entityProperty);

				if (entityProperty.Name == "UICulture")
				{
					entityProperty.Name = "UiCulture";
				}

			    var isLocalizationColumn = false;
				if (LocalizationHelper.IsLocalizationTable(table) && (LocalizationHelper.GetParentLocalizationColumn(table)) == column)
				{
				    isLocalizationColumn = true;
					entityProperty.Name = "Parent";
				}

				entityProperty.TypeName = Helpers.TypeHelper.GetPropertyTypeName(column).Replace("BusinessLayer", "Model");

				if (TypeHelper.IsBusinessObjectReference(column))
				{
					Column referencedColumn = ColumnHelper.GetReferencedColumn(column);
					string referencedColumnType = TypeHelper.GetFieldSystemTypeName(referencedColumn).Trim('?');
					if (column.Nullable)
					{
						referencedColumnType += '?';
					}

					var fkProperty = new EntityProperty
					{
						Column = column,
						TypeName = referencedColumnType
					};
                    if (!isLocalizationColumn && column.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
				    {
				        fkProperty.Name = column.Name.Substring(0, column.Name.Length - 2) + "Id";
				    }
                    else
                    {
                        fkProperty.Name = $"{entityProperty.Name}Id";
                    }

                    var fk = new EntityForeignKey
					{
						Column = column,
						ForeignKeyProperty = fkProperty,
						NavigationProperty = entityProperty
					};

					modelClass.ForeignKeys.Add(fk);
					modelClass.Properties.Add(fkProperty);
				}
			}
		}

		private static void DiscoverCollections(GeneratedModelClass modelClass)
		{
			modelClass.CollectionProperties.AddRange(
				TableHelper.GetCollectionColumns(modelClass.Table)
					.Select(collectionProperty => new EntityCollectionProperty
					{
						Name = collectionProperty.PropertyName,
						CollectionProperty = collectionProperty
					}));
		}
	}
}