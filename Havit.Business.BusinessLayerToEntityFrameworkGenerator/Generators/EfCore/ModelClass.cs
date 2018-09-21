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
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
	public static class ModelClass
	{
		#region Generate

		public static GeneratedModelClass Generate(GeneratedModelClass modelClass, CsprojFile modelCsprojFile, SourceControlClient sourceControlClient)
		{
			string fileName = Helpers.FileHelper.GetFilename(modelClass.Table, "Model", ".cs", "");

			//if (modelCsprojFile != null)
			//{
			//	modelCsprojFile.Ensures(fileName);
			//}

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, fileName), sourceControlClient, true);

			WriteUsings(writer, modelClass);
			WriteNamespaceClassBegin(writer, modelClass, false);
			WriteMembers(writer, modelClass);

			WriteEnumClassMembers(writer, modelClass);
			WriteNamespaceClassEnd(writer);

			writer.Save();

			return modelClass;
		}

		#endregion

		#region WriteUsings

		/// <summary>
		/// Zapíše usings na všechny možné potřebné namespace.
		/// </summary>
		public static void WriteUsings(CodeWriter writer, GeneratedModelClass modelClass)
		{
			writer.WriteLine("using System;");
			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine("using System.Collections.Specialized;");
			writer.WriteLine("using System.ComponentModel;");
			writer.WriteLine("using System.ComponentModel.DataAnnotations;");
			writer.WriteLine("using System.ComponentModel.DataAnnotations.Schema;");
			writer.WriteLine("using System.Globalization;");
			writer.WriteLine("using System.Linq;");
			writer.WriteLine("using System.Text;");
			writer.WriteLine("using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;");
			writer.WriteLine("using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.Metadata;");
			writer.WriteLine("using ReadOnlyAttribute = Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties.ReadOnlyAttribute;");

			if (LocalizationHelper.IsLocalizationTable(modelClass.Table) || LocalizationHelper.IsLocalizedTable(modelClass.Table))
			{
				writer.WriteLine($"using {Helpers.NamingConventions.NamespaceHelper.GetDefaultNamespace("Model")}.Localizations;");
			}

			if (modelClass.Name == "Language")
			{
				writer.WriteLine("using Havit.Model.Localizations;");
			}

			writer.WriteLine();
		}

		#endregion

		#region WriteEnumClassMembers

		private static void WriteEnumClassMembers(CodeWriter writer, GeneratedModelClass modelClass)
		{
			if (TableHelper.GetEnumMode(modelClass.Table) != EnumMode.EnumClass)
			{
				return;
			}

			writer.WriteLine("public enum Entry");
			writer.WriteLine("{");
			List<EnumMember> enumMembers = TableHelper.GetEnumMembers(modelClass.Table);

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
					writer.WriteLine(String.Format("{0} = {1},", enumMember.MemberName, enumMember.MemberID));
				}
				else
				{
					writer.WriteLine(String.Format("{0} = {1}", enumMember.MemberName, enumMember.MemberID));
				}
			}
			writer.WriteLine("}");
		}

		#endregion

		#region WriteNamespaceClassBegin

		public static void WriteNamespaceClassBegin(CodeWriter writer, GeneratedModelClass modelClass, bool includeAttributes)
		{
			writer.WriteLine("namespace " + modelClass.Namespace);
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

			if (TableHelper.GetBoolExtendedProperty(modelClass.Table, "Ignored") == true)
			{
				writer.WriteLine("[Ignored]");
			}

			if (TableHelper.GetBoolExtendedProperty(modelClass.Table, "ReadOnly") == true)
			{
				writer.WriteLine("[ReadOnly]");
			}

			if (TableHelper.GetEnumMode(modelClass.Table) == EnumMode.EnumClass)
			{
				var attributeBuilder = new AttributeStringBuilder("EnumClass");
			    string enumPropertyName = TableHelper.GetStringExtendedProperty(modelClass.Table, "EnumPropertyNameField");
			    if (!string.IsNullOrEmpty(enumPropertyName))
			    {
			        if (modelClass.Properties.Any(prop => prop.Name == enumPropertyName))
			        {
			            enumPropertyName = $"nameof({enumPropertyName})";
			        }
			        else
			        {
			            enumPropertyName = $"\"{enumPropertyName}\"";
			        }

			        attributeBuilder.AddParameter("EnumPropertyName", enumPropertyName);
                }

				writer.WriteLine(attributeBuilder.ToString());
			}

			if (TableHelper.GetBoolExtendedProperty(modelClass.Table, "Cache") == true)
			{
				var attributeBuilder = new AttributeStringBuilder("Cache");
				attributeBuilder.AddBoolExtendedProperty(modelClass.Table, "Cache", "SuppressPreload");
				attributeBuilder.AddExtendedProperty(modelClass.Table, "Cache", "Priority", priority => $"CacheItemPriority.{priority}");
				attributeBuilder.AddExtendedProperty(modelClass.Table, "Cache", "AbsoluteExpiration");
				attributeBuilder.AddExtendedProperty(modelClass.Table, "Cache", "SlidingExpiration");

				writer.WriteLine(attributeBuilder.ToString());
			}

			if (TableHelper.GetBoolExtendedProperty(modelClass.Table, "GenerateIndexes") == false)
			{
				writer.WriteLine("[GenerateIndexes(false)]");
			}

			string businessObjectBaseType = TableHelper.GetStringExtendedProperty(modelClass.Table, "BusinessObjectBaseType");
			if (!string.IsNullOrEmpty(businessObjectBaseType))
			{
				writer.WriteLine($"[BusinessObjectBaseType(\"{businessObjectBaseType}\")]");
			}

			if (TableHelper.GetBoolExtendedProperty(modelClass.Table, "CloneMethod") == true)
			{
				var attributeBuilder = new AttributeStringBuilder("CloneMethod");
				string accessModifier = TableHelper.GetStringExtendedProperty(modelClass.Table, "CloneMethodAccessModifier");
				if (!string.IsNullOrWhiteSpace(accessModifier))
				{
					attributeBuilder.AddParameter("AccessModifier", $"\"{accessModifier}\"");
				}

				writer.WriteLine(attributeBuilder.ToString());
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
				foreach (EntityForeignKey fk in modelClass.ForeignKeys)
				{
					EntityPrimaryKeyPart pk = modelClass.GetPrimaryKeyPartFor(fk.Column);
					if (pk == null)
					{
						// FKs that are not part of PK are handled below
						continue;
					}

					if (ColumnHelper.IsIgnored(pk.Property.Column))
					{
						writer.WriteLine("[Ignored]");
					}
					writer.WriteLine(String.Format("public {0} {1} {{ get; set; }}", pk.Property.TypeName, pk.Property.Name));
					writer.WriteLine(String.Format("public {0} {1} {{ get; set; }}", fk.NavigationProperty.TypeName, fk.NavigationProperty.Name));
					writer.WriteLine();
				}
			}
			else
			{
				EntityPrimaryKeyPart pk = modelClass.PrimaryKeyParts.First();
				if (ColumnHelper.GetBoolExtendedProperty(pk.Property.Column, "Ignored") == true)
				{
					writer.WriteLine("[Ignored]");
				}				
				if (!pk.Property.Column.Identity)
				{
					writer.WriteLine("[DatabaseGenerated(DatabaseGeneratedOption.None)]");
				}
				writer.WriteLine(String.Format("public {0} Id {{ get; set; }}", pk.Property.TypeName));
				writer.WriteLine();
			}

			foreach (EntityProperty entityProperty in modelClass.GetColumnProperties().Where(prop => !prop.Column.InPrimaryKey))
			{
				Column column = entityProperty.Column;

				string description = ColumnHelper.GetDescription(entityProperty.Column);
				if (column.Name == "PropertyName")
				{
					description = "Symbol.";
				}

				writer.WriteCommentSummary(description);
				string accesssModifierText = "public"; //PropertyHelper.GetPropertyAccessModifier(column);

				// DatabaseGenerated (není třeba)
				// ILocalized + ILocalization

				EntityForeignKey fk = modelClass.GetForeignKeyForColumn(column);
				if (fk != null)
				{
					writer.WriteLine(String.Format("{0} {1} {2} {{ get; set; }}", accesssModifierText, fk.NavigationProperty.TypeName, fk.NavigationProperty.Name));
				}

				if (ColumnHelper.GetBoolExtendedProperty(column, "Ignored") == true)
				{
					writer.WriteLine("[Ignored]");
				}

				if (ColumnHelper.GetBoolExtendedProperty(column, "ReadOnly") == true)
				{
					writer.WriteLine("[ReadOnly]");
				}

				Type type = Helpers.TypeHelper.GetPropertyType(fk?.ForeignKeyProperty ?? entityProperty);
				if (!column.Nullable && type?.IsValueType == false)
				{
					writer.WriteLine("[Required]");
				}

				// Generate HasDefaultValueSql. For String columns, generate only if if default is not empty (we use convention for such columns)
				if ((column.DefaultConstraint != null) && (type != typeof(string) || (column.DefaultConstraint.Text != "('')")))
				{
					string defaultValue = new DefaultValueParser().GetDefaultValue(column);

					writer.WriteLine($"[DefaultValue(\"{defaultValue}\")]");
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

				string columnType = null;

				if (BusinessLayerGenerator.Helpers.TypeHelper.IsDateOnly(column.DataType))
				{
					columnType = "Date";
				}
				else if (column.DataType.SqlDataType == SqlDataType.Money)
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
					Type propertyType = Helpers.TypeHelper.GetPropertyType(entityProperty);
					if (propertyType != null)
					{
						RelationalTypeMapping mapping = Helpers.TypeHelper.GetMapping(propertyType);
						if (mapping == null || !column.DataType.IsSameAsTypeMapping(mapping))
						{
							columnType = column.DataType.GetStringRepresentation();
						}
					}
				}

				if (columnType != null)
				{
					writer.WriteLine($"[Column(TypeName = \"{columnType}\")]");
				}

				writer.WriteLine(String.Format("{0} {1} {2} {{ get; set; }}", accesssModifierText, entityProperty.TypeName, entityProperty.Name));

				writer.WriteLine();
			}

			foreach (EntityCollectionProperty collectionProperty in modelClass.CollectionProperties)
			{
				var collection = collectionProperty.CollectionProperty;

				writer.WriteCommentSummary(collection.Description);

			    var attributeBuilder = new AttributeStringBuilder("Collection");
			    attributeBuilder.AddBoolExtendedProperty(table, $"Collection_{collectionProperty.Name}", "IncludeDeleted");
			    attributeBuilder.AddBoolExtendedProperty(table, $"Collection_{collectionProperty.Name}", "LoadAll");
			    attributeBuilder.AddStringExtendedProperty(table, $"Collection_{collectionProperty.Name}", "Sorting");
			    attributeBuilder.AddStringExtendedProperty(table, $"Collection_{collectionProperty.Name}", "CloneMode");

			    if (attributeBuilder.Parameters.Any())
			    {
			        writer.WriteLine(attributeBuilder.ToString());
			    }

				string className;
				if (Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, "Model") == Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(collectionProperty.TargetTable, "Model"))
				{
					className = ClassHelper.GetClassName(collectionProperty.TargetTable);
				}
				else
				{
					className = Helpers.NamingConventions.ClassHelper.GetClassFullName(collectionProperty.TargetTable, "Model");
				}
				writer.WriteLine($"public List<{className}> {collection.PropertyName} {{ get; }} = new List<{className}>();");
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