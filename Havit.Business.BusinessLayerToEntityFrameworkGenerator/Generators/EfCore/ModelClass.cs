using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
	public static class ModelClass
	{
		#region Generate

		public static GeneratedModelClass Generate(Table table, CsprojFile modelCsprojFile, SourceControlClient sourceControlClient)
		{
			string fileName = Helpers.FileHelper.GetFilename(table, "Model", ".cs", "");

			//if (modelCsprojFile != null)
			//{
			//	modelCsprojFile.Ensures(fileName);
			//}

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, fileName), sourceControlClient, true);

			var modelClass = new GeneratedModelClass
			{
				Table = table
			};

			WriteUsings(writer, table);
			WriteNamespaceClassBegin(writer, modelClass, false);
			WriteMembers(writer, modelClass);

			WriteEnumClassMembers(writer, table);
			WriteNamespaceClassEnd(writer);

			writer.Save();

			return modelClass;
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

		public static void WriteNamespaceClassBegin(CodeWriter writer, GeneratedModelClass modelClass, bool includeAttributes)
		{
			modelClass.Name = ClassHelper.GetClassName(modelClass.Table);

			writer.WriteLine("namespace " + Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(modelClass.Table, "Model"));
			writer.WriteLine("{");

			string comment = TableHelper.GetDescription(modelClass.Table);
			writer.WriteCommentSummary(comment);

			string interfaceString = "";
			if (LocalizationHelper.IsLocalizedTable(modelClass.Table))
			{
				interfaceString = String.Format("ILocalized<{0}>", ClassHelper.GetClassName(LocalizationHelper.GetLocalizationTable(modelClass.Table)));
			}
			else if (LocalizationHelper.IsLocalizationTable(modelClass.Table))
			{
				interfaceString = String.Format("ILocalization<{0}>", ClassHelper.GetClassName(LocalizationHelper.GetLocalizationParentTable(modelClass.Table)));
			}
			else if (modelClass.Name == "Language")
			{
				interfaceString = "ILanguage";
			}

			writer.WriteLine(String.Format("{0} class {1}{2}",
				TableHelper.GetAccessModifier(modelClass.Table),
				modelClass.Name,
				String.IsNullOrEmpty(interfaceString) ? "" : " : " + interfaceString));
			writer.WriteLine("{");
		}

		#endregion

		#region WriteMembers

		private static void WriteMembers(CodeWriter writer, GeneratedModelClass modelClass)
		{
			Table table = modelClass.Table;

			if (TableHelper.IsJoinTable(table))
			{
				foreach (Column column in table.Columns.Cast<Column>().Where(c => c.InPrimaryKey))
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
						ForeignKeyPropertyName = pk.Property.Name,
						NavigationPropertyName = ColumnHelper.GetReferencedTable(column).Name,
					};

					writer.WriteLine(String.Format("public {0} {1} {{ get; set; }}", pk.Property.TypeName, pk.Property.Name));
					writer.WriteLine(String.Format("public {0} {1} {{ get; set; }}", TypeHelper.GetPropertyTypeName(column).Replace("BusinessLayer", "Model"), fk.NavigationPropertyName));
					writer.WriteLine();

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
						Column = TableHelper.GetPrimaryKey(table),
						Name = "Id",
					}
				};
				pk.Property.TypeName = TypeHelper.GetFieldSystemTypeName(pk.Property.Column);
				writer.WriteLine(String.Format("public {0} Id {{ get; set; }}", pk.Property.TypeName));
				writer.WriteLine();
				modelClass.PrimaryKeyParts.Add(pk);
				modelClass.Properties.Add(pk.Property);
			}

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

				string description = ColumnHelper.GetDescription(column);

				writer.WriteCommentSummary(description);
				string accesssModifierText = "public"; //PropertyHelper.GetPropertyAccessModifier(column);

				// DatabaseGenerated (není třeba)
				// ILocalized + ILocalization

				entityProperty.TypeName = TypeHelper.GetPropertyTypeName(column).Replace("BusinessLayer", "Model");

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

				writer.WriteLine(String.Format("{0} {1} {2} {{ get; set; }}", accesssModifierText, entityProperty.TypeName, entityProperty.Name));

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
						ForeignKeyPropertyName = fkProperty.Name,
						NavigationPropertyName = entityProperty.Name
					};

					writer.WriteLine(String.Format("{0} {1} {2} {{ get; set; }}", accesssModifierText, fkProperty.TypeName, fk.ForeignKeyPropertyName));
					modelClass.ForeignKeys.Add(fk);
					modelClass.Properties.Add(fkProperty);
				}

				writer.WriteLine();
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
				writer.WriteCommentSummary("Symbol.");
				writer.WriteLine((symbolColumn.DataType.MaximumLength == -1) ? "[MaxLength(Int32.MaxValue)]" : $"[MaxLength({symbolColumn.DataType.MaximumLength})]");
				writer.WriteLine("public string Symbol { get; set; }");
				writer.WriteLine();
			}

			foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(table))
			{
				var entityCollectionProperty = new EntityCollectionProperty
				{
					Name = collectionProperty.PropertyName,
					CollectionProperty = collectionProperty
				};
				modelClass.CollectionProperties.Add(entityCollectionProperty);

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