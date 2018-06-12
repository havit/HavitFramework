using System;
using System.Collections.Generic;
using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
	public static class ModelClass
	{
		#region Generate

		public static void Generate(Table table, CsprojFile modelCsprojFile, SourceControlClient sourceControlClient)
		{
			string fileName = FileHelper.GetFilename(table, ".cs", "");

			if (modelCsprojFile != null)
			{
				modelCsprojFile.Ensures(fileName);
			}

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, "Model", fileName), sourceControlClient, true);

			WriteUsings(writer, table);
			WriteNamespaceClassBegin(writer, table, false);
			WriteMembers(writer, table);
			WriteEnumClassMembers(writer, table);
			WriteNamespaceClassEnd(writer);

			writer.Save();
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
			writer.WriteLine("using System.ComponentModel.DataAnnotations;");			
			writer.WriteLine("using System.Globalization;");
			writer.WriteLine("using System.Linq;");
			writer.WriteLine("using System.Text;");

			if (LocalizationHelper.IsLocalizationTable(table) || LocalizationHelper.IsLocalizedTable(table))
			{
				writer.WriteLine($"using {Helpers.NamingConventions.NamespaceHelper.GetDefaultNamespace("Model")}.Localizations;");
			}

			if (table.Name == "Language")
			{
				writer.WriteLine("using Havit.Model.Localizations;");
			}

			writer.WriteLine();
		}

		#endregion

		#region WriteEnumClassMembers

		private static void WriteEnumClassMembers(CodeWriter writer, Table table)
		{
			if (TableHelper.GetEnumMode(table) != EnumMode.EnumClass)
			{
				return;
			}

			writer.WriteLine("public enum Entry");
			writer.WriteLine("{");
			List<EnumMember> enumMembers = TableHelper.GetEnumMembers(table);

			for (int i = 0; i < enumMembers.Count; i++)
			{
				EnumMember enumMember = enumMembers[i];
				string comment = !String.IsNullOrEmpty(enumMember.Comment) ? enumMember.Comment : enumMember.MemberName;
				if (!String.IsNullOrEmpty(comment) && (comment != enumMember.MemberName))
				{
					if (i > 0)
					{
						writer.WriteLine();
					}
					writer.WriteCommentSummary(comment);
				}
				if (i < (enumMembers.Count - 1))
				{
					writer.WriteLine(enumMember.MemberName + ",");
				}
				else
				{
					writer.WriteLine(enumMember.MemberName);
				}
			}
			writer.WriteLine("}");
		}

		#endregion

		#region WriteNamespaceClassBegin

		public static void WriteNamespaceClassBegin(CodeWriter writer, Table table, bool includeAttributes)
		{
			writer.WriteLine("namespace " + Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Model"));
			writer.WriteLine("{");

			string comment = TableHelper.GetDescription(table);
			writer.WriteCommentSummary(comment);

			string interfaceString = "";
			if (LocalizationHelper.IsLocalizedTable(table))
			{
				interfaceString = String.Format("ILocalized<{0}>", ClassHelper.GetClassName(LocalizationHelper.GetLocalizationTable(table)));
			}
			else if (LocalizationHelper.IsLocalizationTable(table))
			{
				interfaceString = String.Format("ILocalization<{0}>", ClassHelper.GetClassName(LocalizationHelper.GetLocalizationParentTable(table)));
			}
			else if (table.Name == "Language")
			{
				interfaceString = "ILanguage";
			}

			writer.WriteLine(String.Format("{0} class {1}{2}",
				TableHelper.GetAccessModifier(table),
				ClassHelper.GetClassName(table),
				String.IsNullOrEmpty(interfaceString) ? "" : " : " + interfaceString));
			writer.WriteLine("{");
		}

		#endregion

		#region WriteMembers

		private static void WriteMembers(CodeWriter writer, Table table)
		{
			writer.WriteLine("public int Id { get; set; }");
			writer.WriteLine();
			foreach (Column column in TableHelper.GetPropertyColumns(table))
			{
				string description = ColumnHelper.GetDescription(column);

				writer.WriteCommentSummary(description);
				string accesssModifierText = PropertyHelper.GetPropertyAccessModifier(column);

				// DatabaseGenerated (není třeba)
				// ILocalized + ILocalization

				string propertyName = PropertyHelper.GetPropertyName(column, "Id");
				string propertyTypeName = TypeHelper.GetPropertyTypeName(column).Replace("BusinessLayer", "Model");

				if (propertyName == "UICulture")
				{
					propertyName = "UiCulture";
				}

				if (TypeHelper.IsDateOnly(column.DataType))
				{
					writer.WriteLine("[DataType(DataType.Date)]");
				}

				if (PropertyHelper.IsString(column))
				{
					int maxLength = column.DataType.MaximumLength;
					writer.WriteLine((maxLength == -1) ? "[MaxLength(Int32.MaxValue)]" : $"[MaxLength({maxLength})]");

					//if (column.Nullable)
					//{
					//	writer.WriteLine("[Required(AllowEmptyStrings = true)]");					
					//}
				}

				if (LocalizationHelper.IsLocalizationTable(table) && (LocalizationHelper.GetParentLocalizationColumn(table)) == column)
				{
					propertyName = "Parent";
				}

				writer.WriteLine(String.Format("{0} {1} {2} {{ get; set; }}", accesssModifierText, propertyTypeName, propertyName));

				if (TypeHelper.IsBusinessObjectReference(column))
				{
					string foreignKeyTypeName = column.Nullable ? "int?" : "int";
					string foreignKeyPropertyName = propertyName + "Id";
					writer.WriteLine(String.Format("{0} {1} {2} {{ get; set; }}", accesssModifierText, foreignKeyTypeName, foreignKeyPropertyName));
				}

				writer.WriteLine();
			}

			Column symbolColumn = table.Columns["PropertyName"];
			if (symbolColumn != null)
			{
				writer.WriteCommentSummary("Symbol.");
				writer.WriteLine((symbolColumn.DataType.MaximumLength == -1) ? "[MaxLength(Int32.MaxValue)]" : $"[MaxLength({symbolColumn.DataType.MaximumLength})]");
				writer.WriteLine("public string Symbol { get; set; }");
				writer.WriteLine();
			}

			foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(table))
			{
				writer.WriteCommentSummary(collectionProperty.Description);

				if (Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Model") == Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(collectionProperty.TargetTable, "Model"))
				{
					writer.WriteLine($"public List<{ClassHelper.GetClassName(collectionProperty.TargetTable)}> {collectionProperty.PropertyName} {{ get; set; }}");
				}
				else
				{
					writer.WriteLine($"public List<{Helpers.NamingConventions.ClassHelper.GetClassFullName(collectionProperty.TargetTable, "Model")}> {collectionProperty.PropertyName} {{ get; set; }}");
				}
				writer.WriteLine();
			}

		}

		#endregion

		#region WriteNamespaceClassEnd

		public static void WriteNamespaceClassEnd(CodeWriter writer)
		{
			writer.WriteLine("}");
			writer.WriteLine("}");
		}

		#endregion

	}
}