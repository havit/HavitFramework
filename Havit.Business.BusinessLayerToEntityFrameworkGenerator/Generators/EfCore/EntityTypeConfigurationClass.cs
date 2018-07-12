using System;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
	public static class EntityTypeConfigurationClass
	{
		#region Generate
		public static void Generate(Table table, CsprojFile entityCsprojFile, SourceControlClient sourceControlClient)
		{
			string fileName = Path.Combine("Configurations", FileHelper.GetFilename(Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Entity.Configurations", false), ClassHelper.GetClassName(table), ".cs", ""));

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, @"Entity", fileName), sourceControlClient, true);

			WriteUsings(writer, table);
			WriteNamespaceClassConstructorBegin(writer, table, false);

			bool shouldSave = WritePrecisions(writer, table);
			shouldSave |= WriteCollections(writer, table);
			shouldSave |= WriteJoinTablePKs(writer, table);
			shouldSave |= WritePrincipals(writer, table);
			WriteNamespaceClassConstructorEnd(writer);

			if (true)
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

		#endregion

		#region WriteUsings

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

		#endregion

		#region WriteNamespaceClassConstructorBegin
		public static void WriteNamespaceClassConstructorBegin(CodeWriter writer, Table table, bool includeAttributes)
		{
			writer.WriteLine("namespace " + Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Entity.Configurations"));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("public class {0}Configuration : IEntityTypeConfiguration<{0}>", ClassHelper.GetClassName(table)));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("public void Configure(EntityTypeBuilder<{0}> builder)", ClassHelper.GetClassName(table)));
			writer.WriteLine("{");
		}
		#endregion

		private static bool WritePrecisions(CodeWriter writer, Table table)
		{
			bool result = false;
			foreach (Column column in TableHelper.GetPropertyColumns(table))
			{
				if (column.DataType.SqlDataType == SqlDataType.Money)
				{
					writer.WriteLine(String.Format("builder.Property({0} => {0}.{1}).HasColumnType(\"Money\");",
						ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)),
						PropertyHelper.GetPropertyName(column)));
					result = true;
				}
				else if ((column.DataType.SqlDataType == SqlDataType.Decimal) && ((column.DataType.NumericPrecision != 18) && column.DataType.NumericScale != 2))
				{
					writer.WriteLine(String.Format("builder.Property({0} => {0}.{1}).HasColumnType(\"decimal({2}, {3})\");",
						ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)),
						PropertyHelper.GetPropertyName(column),
						column.DataType.NumericPrecision,
						column.DataType.NumericScale));
					result = true;
				}
			}

			if (result)
			{
				writer.WriteLine();
			}
			return result;
		}

		private static bool WriteCollections(CodeWriter writer, Table table)
		{
			bool result = false;
			foreach (CollectionProperty collection in TableHelper.GetCollectionColumns(table))
			{
				string hasMany = String.Format("builder.HasMany({0} => {0}.{1})",
					ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)),
					collection.PropertyName);

				if (collection.IsOneToMany)
				{
					if (LocalizationHelper.IsLocalizedTable(table) && (collection.PropertyName == "Localizations"))
					{
						continue;
					}

					// pokud je v cílové tabulce jen jeden klíč, není třeba
					//               if (collection.TargetTable.Columns
					//	.Cast<Column>()
					//	.Any(column => TypeHelper.IsBusinessObjectReference(column) 
					//		&& (ColumnHelper.GetReferencedTable(column) == table)
					//		/*&& String.Equals(column.Name, table.Name + "Id", StringComparison.InvariantCultureIgnoreCase))*/))
					//{ 
					//	continue;						
					//}

					writer.WriteLine(hasMany);

					writer.Indent();
					writer.WriteLine(String.Format(".WithOne({0} => {0}.{1})",
						ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(collection.TargetTable)),
						PropertyHelper.GetPropertyName(collection.ReferenceColumn)));
					if (!collection.ReferenceColumn.Nullable)
					{
						writer.WriteLine(".IsRequired();");
					}
					else
					{
						writer.EndPreviousStatement();
					}
					writer.Unindent();

					result = true;
				}

				//if (collection.IsManyToMany)
				//{
				//	writer.WriteLine(hasMany);

				//	CollectionProperty reverseDirectionProperty = TableHelper.GetCollectionColumns(collection.TargetTable).Find(item => (item.JoinTable == collection.JoinTable));
				//	writer.Indent();
				//	if (reverseDirectionProperty != null)
				//	{
				//		writer.WriteLine(String.Format(".WithOne({0} => {0}.{1})",
				//			ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(collection.TargetTable)),
				//			reverseDirectionProperty.PropertyName));
				//	}
				//	else
				//	{
				//		writer.WriteLine(".WithOne()");
				//	}
				//	//writer.WriteLine(".Map(m =>");
				//	//writer.WriteLine("{");
				//	//writer.WriteLine(String.Format("m.ToTable(\"{1}\", \"{0}\");", collection.JoinTable.Schema, collection.JoinTable.Name));
				//	//writer.Unindent();
				//	//writer.WriteLine("});");
				//	writer.Unindent();

				//	result = true;
				//}
				writer.WriteLine();
			}
			return result;
		}

		private static bool WritePrincipals(CodeWriter writer, Table table)
		{
			if (TableHelper.IsJoinTable(table))
			{
				return false;
			}

			bool result = false;

			foreach (Column column in table.Columns)
			{
				if (TypeHelper.IsBusinessObjectReference(column))
				{
					Table referencedTable = ColumnHelper.GetReferencedTable(column);
					//if (referencedTable.Columns.Cast<Column>().Any(referencedTableColumn => // v tabulce, kam se odkazujeme existuje sloupec
					//		TypeHelper.IsBusinessObjectReference(referencedTableColumn) // který je cizím klíčem
					//		&& (ColumnHelper.GetReferencedTable(referencedTableColumn) == table))  // do této tabulky
					//	&& !TableHelper.GetCollectionColumns(referencedTable).Any(item => item.ReferenceColumn == column)) // v cílové tabulce ke sloupci není kolekce						
					{
						var propertyName = PropertyHelper.GetPropertyName(column);
						if (LocalizationHelper.IsLocalizationTable(table) && (LocalizationHelper.GetParentLocalizationColumn(table)) == column)
						{
							propertyName = "Parent";
						}

						writer.WriteLine(String.Format("builder.HasOne({0} => {0}.{1})",
							ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)),
							propertyName));
						writer.Indent();

						//Column referencedCollectionColumn = referencedTable.Columns.Cast<Column>().FirstOrDefault(referencedTableColumn => // v tabulce, kam se odkazujeme existuje sloupec
						//	TypeHelper.IsBusinessObjectReference(referencedTableColumn) // který je cizím klíčem
						//	&& (ColumnHelper.GetReferencedTable(referencedTableColumn) == table)); // do této tabulky;
						var collectionProperty = TableHelper.GetCollectionColumns(referencedTable).FirstOrDefault(item => item.ReferenceColumn == column);
						if (collectionProperty != null) // v cílové tabulce ke sloupci není kolekce	
						{
							writer.WriteLine(String.Format(".WithMany({0} => {0}.{1})",
								ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(referencedTable)),
								collectionProperty.PropertyName
							));
							writer.WriteLine(String.Format(".HasForeignKey({0} => {0}.{1})",
								ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)),
								propertyName + "Id"
							));
						}
						else
						{
							writer.WriteLine(".WithMany()");
						}

						// if there are multiple FKs to same table, it is possible it might trigger this error:
						//	Introducing FOREIGN KEY constraint 'FK_Delegation_Employee_DelegatingEmployeeId' on table 'Delegation' may cause cycles or multiple cascade paths. 
						//	Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
						//	Could not create constraint or index.

						Column[] columns = table.Columns
							.Cast<Column>()
							.Where(tableColumn => TypeHelper.IsBusinessObjectReference(tableColumn) && ColumnHelper.GetReferencedTable(tableColumn) == referencedTable)
							.ToArray();
						//if (columns.Length > 1)
						{
							writer.WriteLine(".OnDelete(DeleteBehavior.Restrict)");
						}

						if (!column.Nullable)
						{
							writer.WriteLine(".IsRequired();");
						}
						else
						{
							writer.EndPreviousStatement();
						}

						writer.Unindent();
						writer.WriteLine();

						result = true;
					}
				}
			}
			return result;
		}

		private static bool WriteJoinTablePKs(CodeWriter writer, Table table)
		{
			if (!TableHelper.IsJoinTable(table))
			{
				return false;
			}

			string columns = String.Join(", ", table.Columns.Cast<Column>()
				.Select(column => String.Format("{0}.{1}Id", ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)), ColumnHelper.GetReferencedTable(column).Name)));

			writer.WriteLine(String.Format("builder.HasKey({0} => new {{ {1} }});",
				ConventionsHelper.GetCammelCase(ClassHelper.GetClassName(table)),
				columns));

			return true;
		}

		#region WriteNamespaceClassConstructorEnd
		public static void WriteNamespaceClassConstructorEnd(CodeWriter writer)
		{
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("}");
		}
		#endregion

	}
}