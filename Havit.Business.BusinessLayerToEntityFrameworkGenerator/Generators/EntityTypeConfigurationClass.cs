using System;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;
using FileHelper = Havit.Business.BusinessLayerGenerator.Helpers.FileHelper;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
    public static class EntityTypeConfigurationClass
	{
		public static void Generate(GeneratedModel model, GeneratedModelClass modelClass, CsprojFile entityCsprojFile)
		{
			Table table = modelClass.Table;

			string fileName = Path.Combine("Configurations", FileHelper.GetFilename(Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Entity.Configurations", false), ClassHelper.GetClassName(table) + "Configuration", ".cs", ""));

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, @"Entity", fileName), true);

			WriteUsings(writer, table);
			WriteNamespaceClassConstructorBegin(writer, modelClass, false);

			bool shouldSave = WriteTablePKs(writer, modelClass);
			// configuration directives for collections shouldn't be necessary, they're covered by EF Core conventions
			shouldSave |= WritePrincipals(writer, modelClass);
			shouldSave |= WriteCustomIndexes(writer, modelClass);
            shouldSave |= WritePropertyMetadata(writer, modelClass);

			WriteNamespaceClassConstructorEnd(writer);

			if (shouldSave)
			{
				if (entityCsprojFile != null)
				{
					entityCsprojFile.Ensures(fileName);
				}
				writer.Save();
			}
			else
			{
				Console.WriteLine($"Skipping EntityTypeConfiguration class for entity {table.Name}");
			}
		}

        private static bool WritePropertyMetadata(CodeWriter writer, GeneratedModelClass modelClass)
        {
            bool result = false;
            foreach (EntityProperty columnProperty in modelClass.GetColumnProperties())
            {
                if (!string.IsNullOrEmpty(columnProperty.Column.ComputedText))
                {
                    writer.WriteLine(String.Format("builder.Property({0} => {0}.{1})",
                        ConventionsHelper.GetCammelCase(modelClass.Name),
                        columnProperty.Name));
                    writer.Indent();

                    // Fluent API does not allow setting PERSISTED flag. It can be however placed directly into the SQL statement (officially endorsed workaround)
                    // https://github.com/aspnet/EntityFrameworkCore/issues/6682#issuecomment-459093219
                    var computeSql = columnProperty.Column.ComputedText;
                    if (columnProperty.Column.IsPersisted)
                    {
                        computeSql += " PERSISTED";
                    }
                    writer.WriteLine(String.Format(".HasComputedColumnSql(\"{0}\")", computeSql));
                    writer.WriteLine(".HasConventionSuppressed<StringPropertiesDefaultValueConvention>()");

                    writer.EndPreviousStatement();
                    writer.Unindent();
                    writer.WriteLine();

                    result = true;
                }
            }

            return result;
        }

        /// <summary>
		/// Zapíše usings na všechny možné potřebné namespace.
		/// </summary>
		public static void WriteUsings(CodeWriter writer, Table table)
		{
			writer.WriteLine("using System;");
			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine("using System.Collections.Specialized;");
			writer.WriteLine("using System.Globalization;");
			writer.WriteLine("using System.Linq;");
			writer.WriteLine("using System.Text;");
			writer.WriteLine("using Microsoft.EntityFrameworkCore;");
			writer.WriteLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
			writer.WriteLine($"using {Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Model")};");

			writer.WriteLine();
		}

		public static void WriteNamespaceClassConstructorBegin(CodeWriter writer, GeneratedModelClass modelClass, bool includeAttributes)
		{
			writer.WriteLine("namespace " + Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(modelClass.Table, "Entity.Configurations"));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("public class {0}Configuration : IEntityTypeConfiguration<{0}>", modelClass.Name));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("public void Configure(EntityTypeBuilder<{0}> builder)", modelClass.Name));
			writer.WriteLine("{");
		}

		private static bool WritePrincipals(CodeWriter writer, GeneratedModelClass modelClass)
		{
			Table table = modelClass.Table;

			bool result = false;

			// select for configuration those FKs, which reference same table
			var fksToConfigure = modelClass.ForeignKeys.GroupBy(fk => ColumnHelper.GetReferencedTable(fk.Column)).Where(g => g.Count() >= 2).SelectMany(g => g);

			// then we need to configure FKs, which are part of circular reference (Table1 <-> Table2)
			var circularReferences = modelClass.ForeignKeys
				.Where(fk => ColumnHelper.GetReferencedTable(fk.Column).ForeignKeys.Cast<ForeignKey>()
					.Any(fk1 => fk1.Parent != table && fk1.ReferencedTable == modelClass.Table.Name))
				.ToArray();

			var cascadeActions = modelClass.ForeignKeys
				.Where(fk => table.ForeignKeys.AsEnumerable()
								.Where(item => item.Columns.Contains(fk.Column.Name))
								.Where(item => item.DeleteAction == ForeignKeyAction.Cascade)
								.Any())
				.ToList();

			fksToConfigure = fksToConfigure.Union(circularReferences).Union(cascadeActions).ToList();
			

			// TODO: if the check for JoinTable above is removed, need to carefully handle FKs that are part of composite PK
			// (i.e. don't generate HasOne statements)
			foreach (EntityForeignKey foreignKey in fksToConfigure)
			{
				Column column = foreignKey.Column;				

				Table referencedTable = ColumnHelper.GetReferencedTable(column);
				//if (referencedTable.Columns.Cast<Column>().Any(referencedTableColumn => // v tabulce, kam se odkazujeme existuje sloupec
				//		TypeHelper.IsBusinessObjectReference(referencedTableColumn) // který je cizím klíčem
				//		&& (ColumnHelper.GetReferencedTable(referencedTableColumn) == table))  // do této tabulky
				//	&& !TableHelper.GetCollectionColumns(referencedTable).Any(item => item.ReferenceColumn == column)) // v cílové tabulce ke sloupci není kolekce						
				{
					var propertyName = foreignKey.NavigationProperty.Name;

					writer.WriteLine(String.Format("builder.HasOne({0} => {0}.{1})",
						ConventionsHelper.GetCammelCase(modelClass.Name),
						propertyName));
					writer.Indent();

					//Column referencedCollectionColumn = referencedTable.Columns.Cast<Column>().FirstOrDefault(referencedTableColumn => // v tabulce, kam se odkazujeme existuje sloupec
					//	TypeHelper.IsBusinessObjectReference(referencedTableColumn) // který je cizím klíčem
					//	&& (ColumnHelper.GetReferencedTable(referencedTableColumn) == table)); // do této tabulky;
					var collectionProperty = TableHelper.GetCollectionColumns(referencedTable).FirstOrDefault(item => item.ReferenceColumn == column);
					if (collectionProperty != null) // v cílové tabulce ke sloupci není kolekce	
					{
						writer.WriteLine(String.Format(".WithMany({0} => {0}.{1})",
							ConventionsHelper.GetCammelCase(collectionProperty.ParentTable.Name),
							collectionProperty.PropertyName
						));
						writer.WriteLine(String.Format(".HasForeignKey({0} => {0}.{1})",
							ConventionsHelper.GetCammelCase(modelClass.Name),
							foreignKey.ForeignKeyProperty.Name
						));
					}
					else
					{
						writer.WriteLine(".WithMany()");
					}

					if (!column.Nullable)
					{
						writer.WriteLine(".IsRequired()");
					}

					if (cascadeActions.Contains(foreignKey))
					{ 
						writer.WriteLine(".OnDelete(DeleteBehavior.Cascade)");
					}

					writer.EndPreviousStatement();

					writer.Unindent();
					writer.WriteLine();

					result = true;
				}
			}
			return result;
		}

		private static bool WriteTablePKs(CodeWriter writer, GeneratedModelClass modelClass)
		{
			if (TableHelper.IsJoinTable(modelClass.Table) && modelClass.Properties.Count > 2)
			{
				string columns = String.Join(", ", modelClass.PrimaryKeyParts
					.Select(pk => String.Format("{0}.{1}",
						ConventionsHelper.GetCammelCase(modelClass.Name),
						pk.Property.Name)));

				writer.WriteLine(String.Format("builder.HasKey({0} => new {{ {1} }});",
					ConventionsHelper.GetCammelCase(modelClass.Name),
					columns));

				return true;
			}

			return false;
		}

		private static bool WriteCustomIndexes(CodeWriter writer, GeneratedModelClass modelClass)
		{
			bool result = false;
			foreach (Microsoft.SqlServer.Management.Smo.Index index in modelClass.Table.Indexes)
			{
				// přeskakujeme indexy primárního klíče a automaticky vygenerované indexy
				if (index.Name.StartsWith("PK_") || index.Name.StartsWith("FKX_"))
				{
					continue;
				}

				if (index.IsUnique && !index.Name.StartsWith("UIDX_"))
				{
					ConsoleHelper.WriteLineWarning(String.Format("Název unikátního indexu {0} na tabulce {1} nezačíná UIDX_.", index.Name, modelClass.Table.Name));
				}

				if (!index.IsUnique && !index.Name.StartsWith("IDX_"))
				{
					ConsoleHelper.WriteLineWarning(String.Format("Název indexu {0} na tabulce {1} nezačíná IDX_.", index.Name, modelClass.Table.Name));
				}

				//if (LocalizationHelper.IsLocalizationTable(modelClass.Table))
				//{
				//	if (index.IsUnique && (index.IndexedColumns.Count == 2)
				//		&& index.IndexedColumns.Cast<IndexedColumn>().All(indexedColumn => !indexedColumn.IsIncluded)
				//		&& index.IndexedColumns.Cast<IndexedColumn>().All(indexedColumn => !indexedColumn.IsIncluded)
				//		)
				//}

				var indexedColumns = index.IndexedColumns.Cast<IndexedColumn>().Where(column => !column.IsIncluded).ToList();
				var includedIndexedColumns = index.IndexedColumns.Cast<IndexedColumn>().Where(column => column.IsIncluded).ToList();

				writer.WriteLine("builder");
				writer.Indent();

				WriteIndexesColumns(writer, modelClass, indexedColumns, ".HasIndex");
				if (includedIndexedColumns.Count > 0)
				{
					WriteIndexesColumns(writer, modelClass, includedIndexedColumns, ".IncludeProperties");
				}

				if (index.IsUnique)
				{
					writer.WriteLine(".IsUnique()");
				}

				if (!String.IsNullOrEmpty(index.FilterDefinition))
				{
					writer.WriteLine($".HasFilter(\"{index.FilterDefinition}\")");
				}
				else if (index.IsUnique && index.IndexedColumns.Cast<IndexedColumn>().Any(ic => modelClass.Table.Columns[ic.Name].Nullable))
				{
					writer.WriteLine($".HasFilter(null)");
				}
				writer.WriteLine($".HasName(\"{index.Name}\");");
				writer.Unindent();
				writer.WriteLine();

				result = true;
			}

			return result;
		}

		private static void WriteIndexesColumns(CodeWriter writer, GeneratedModelClass modelClass, System.Collections.Generic.List<IndexedColumn> indexedColumns, string methodName)
		{
			string entityCammelCase = ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(modelClass.Table));
			if (indexedColumns.Count == 1)
			{
				Column column = modelClass.Table.Columns[indexedColumns.Single().Name];
				writer.WriteLine(String.Format("{0}({1} => {1}.{2})", methodName, entityCammelCase, modelClass.GetPropertyFor(column).Name));
			}
			else
			{
				writer.WriteLine(String.Format("{0}({1} => new", methodName, ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(modelClass.Table))));
				writer.WriteLine("{");
				for (int i = 0; i < indexedColumns.Count; i++)
				{
					IndexedColumn indexedColumn = indexedColumns[i];
					Column column = modelClass.Table.Columns[indexedColumn.Name];
					writer.WriteLine(String.Format("{0}.{1}{2}",
						entityCammelCase, //0
                        modelClass.GetPropertyFor(column).Name, // 1
						(i < (indexedColumns.Count - 1)) ? "," : "")); // 2
				}
				writer.Unindent();
				writer.WriteLine("})");
			}
		}

		public static void WriteNamespaceClassConstructorEnd(CodeWriter writer)
		{
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("}");
		}

	}
}