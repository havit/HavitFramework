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

			var modelClasses = new List<GeneratedModelClass>(tables.Length);
			foreach (Table table in tables)
			{
				if (table.Name.StartsWith("TulipReports_"))
				{
					ConsoleHelper.WriteLineWarning("Ignoring table: {0}", table.Name);
					continue;
				}

				ConsoleHelper.WriteLineInfo(table.Name);

				modelClasses.Add(GetModelClass(table));
			}

			return modelClasses;
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
							Name = String.Format("{0}Id", ColumnHelper.GetReferencedTable(column).Name),
							TypeName = TypeHelper.GetFieldSystemTypeName(referencedColumn)
						}
					};
					var fk = new EntityForeignKey
					{
						Column = column,
						ForeignKeyProperty = pk.Property,
						NavigationProperty = new EntityProperty
						{
							Column = column,
							Name = ColumnHelper.GetReferencedTable(column).Name,
							TypeName = TypeHelper.GetPropertyTypeName(column).Replace("BusinessLayer", "Model")
						},
					};

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
				if (column.Name == "PropertyName")
				{
					// PropertyName is column for enum tables, it's handled further below (PropertyName -> Symbol)
					continue;
				}

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

				if (LocalizationHelper.IsLocalizationTable(table) && (LocalizationHelper.GetParentLocalizationColumn(table)) == column)
				{
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
						Name = $"{entityProperty.Name}Id",
						TypeName = referencedColumnType
					};

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

			Column symbolColumn = table.Columns["PropertyName"];
			if (symbolColumn != null)
			{
				var symbolProperty = new EntityProperty
				{
					Column = symbolColumn,
					Name = "Symbol",
					TypeName = "string"
				};
				modelClass.Properties.Add(symbolProperty);
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