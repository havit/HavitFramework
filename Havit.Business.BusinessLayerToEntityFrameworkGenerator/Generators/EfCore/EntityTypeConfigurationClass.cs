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