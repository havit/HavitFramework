using System;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Management.Smo;
using FileHelper = Havit.Business.BusinessLayerGenerator.Helpers.FileHelper;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
	public static class EntityTypeConfigurationClass
	{
		#region Generate
		public static void Generate(GeneratedModel model, GeneratedModelClass modelClass, CsprojFile entityCsprojFile, SourceControlClient sourceControlClient)
		{
			Table table = modelClass.Table;

			string fileName = Path.Combine("Configurations", FileHelper.GetFilename(Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Entity.Configurations", false), ClassHelper.GetClassName(table) + "Configuration", ".cs", ""));

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, @"Entity", fileName), sourceControlClient, true);

			WriteUsings(writer, table);
			WriteNamespaceClassConstructorBegin(writer, modelClass, false);

			bool shouldSave = WriteTablePKs(writer, modelClass);
			shouldSave |= WriteColumnMetadata(writer, modelClass);
			shouldSave |= WritePrecisions(writer, modelClass);
			// configuration directives for collections shouldn't be necessary, they're covered by EF Core conventions
			shouldSave |= WritePrincipals(writer, modelClass);
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
		public static void WriteNamespaceClassConstructorBegin(CodeWriter writer, GeneratedModelClass modelClass, bool includeAttributes)
		{
			writer.WriteLine("namespace " + Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(modelClass.Table, "Entity.Configurations"));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("public class {0}Configuration : IEntityTypeConfiguration<{0}>", modelClass.Name));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("public void Configure(EntityTypeBuilder<{0}> builder)", modelClass.Name));
			writer.WriteLine("{");
		}
		#endregion

		private static bool WritePrecisions(CodeWriter writer, GeneratedModelClass modelClass)
		{
			bool result = false;
			foreach (EntityProperty property in modelClass.GetColumnProperties())
			{
				Column column = property.Column;

				string columnType = null;

				if (column.DataType.SqlDataType == SqlDataType.Money)
				{
					columnType = "Money";
				}
				else if ((column.DataType.SqlDataType == SqlDataType.Decimal) && ((column.DataType.NumericPrecision != 18) && column.DataType.NumericScale != 2))
				{
					columnType = String.Format("decimal({0}, {1})",
						column.DataType.NumericPrecision,
						column.DataType.NumericScale);
				}
				else if (!column.DataType.IsStringType)
				{
					Type type = Helpers.TypeHelper.GetPropertyType(property);
					if (type != null)
					{
						RelationalTypeMapping mapping = Helpers.TypeHelper.GetMapping(type);
						if (mapping == null || !column.DataType.IsSameAsTypeMapping(mapping))
						{
							columnType = column.DataType.GetStringRepresentation();
						}
					}
				}

				if (columnType != null)
				{
					writer.WriteLine(String.Format("builder.Property({0} => {0}.{1}).HasColumnType(\"{2}\");",
						ConventionsHelper.GetCammelCase(modelClass.Name),
						property.Name, columnType));
					result = true;
				}
			}

			if (result)
			{
				writer.WriteLine();
			}
			return result;
		}

		private static bool WritePrincipals(CodeWriter writer, GeneratedModelClass modelClass)
		{
			Table table = modelClass.Table;

			bool result = false;

			var fksToConfigure = modelClass.ForeignKeys.GroupBy(fk => ColumnHelper.GetReferencedTable(fk.Column)).Where(g => g.Count() >= 2).SelectMany(g => g);

			var circularReferences = modelClass.ForeignKeys.Where(fk => ColumnHelper.GetReferencedTable(fk.Column).ForeignKeys.Cast<ForeignKey>().Any(fk1 => fk1.Parent != table && fk1.ReferencedTable == modelClass.Table.Name)).ToArray();
			if (circularReferences.Length > 0)
			{
				fksToConfigure = fksToConfigure.Concat(circularReferences);
			}

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
			return result;
		}

		private static bool WriteColumnMetadata(CodeWriter writer, GeneratedModelClass modelClass)
		{
			Table table = modelClass.Table;

			bool shouldSave = false;

			foreach (EntityProperty property in modelClass.GetColumnProperties())
			{

				Column column = property.Column;

				var pkPart = modelClass.GetPrimaryKeyPartFor(column);
				if (pkPart != null && !column.Identity)
				{
					writer.WriteLine(String.Format("builder.Property({0} => {0}.{1})",
						ConventionsHelper.GetCammelCase(modelClass.Name),
						property.Name));
					writer.Indent();
					writer.WriteLine(".ValueGeneratedNever();");
					writer.Unindent();
					writer.WriteLine();

					shouldSave = true;
				}

				if (LocalizationHelper.IsLocalizationTable(table) && (LocalizationHelper.GetParentLocalizationColumn(table)) == column)
				{
					writer.WriteLine(String.Format("builder.Property({0} => {0}.ParentId)",
						ConventionsHelper.GetCammelCase(modelClass.Name)));
					writer.Indent();
					writer.WriteLine(String.Format(".HasColumnName(\"{0}\");", column.Name));
					writer.Unindent();
					writer.WriteLine();

					shouldSave = true;
				}
			}

			return shouldSave;
		}

		private static bool WriteTablePKs(CodeWriter writer, GeneratedModelClass modelClass)
		{
			if (TableHelper.IsJoinTable(modelClass.Table))
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