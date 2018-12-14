using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Settings;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{	
	public static class ModelClass
	{
		public static GeneratedModelClass Generate(GeneratedModelClass modelClass, CsprojFile modelCsprojFile)
		{
			string fileName = Helpers.FileHelper.GetFilename(modelClass.Table, "Model", ".cs", "");

			//if (modelCsprojFile != null)
			//{
			//	modelCsprojFile.Ensures(fileName);
			//}

			CodeWriter writer = new CodeWriter(Path.Combine(GeneratorSettings.SolutionPath, fileName), true);

			WriteUsings(writer, modelClass);
			WriteNamespaceClassBegin(writer, modelClass, false);
			WriteMembers(writer, modelClass);

			WriteEnumClassMembers(writer, modelClass);
			WriteNamespaceClassEnd(writer);

			writer.Save();

			return modelClass;
		}

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
			writer.WriteLine("using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes;");
			writer.WriteLine("using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;");
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

		public static void WriteNamespaceClassBegin(CodeWriter writer, GeneratedModelClass modelClass, bool includeAttributes)
		{
			writer.WriteLine("namespace " + modelClass.Namespace);
			writer.WriteLine("{");

			string comment = TableHelper.GetDescription(modelClass.Table, suppressDefaults: true);
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

			string businessObjectBaseType = TableHelper.GetStringExtendedProperty(modelClass.Table, "BusinessObjectBaseType");
			if (!string.IsNullOrEmpty(businessObjectBaseType))
			{
				writer.WriteLine($"[BusinessObjectBaseType(\"{businessObjectBaseType}\")]");
			}

			if (TableHelper.GetCreateObjectAccessModifier(modelClass.Table) != "public")
			{
				writer.WriteLine(String.Format("[CreateObjectAccessModifier(\"{0}\")]", TableHelper.GetCreateObjectAccessModifier(modelClass.Table)));
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

				string description = ColumnHelper.GetDescription(pk.Property.Column, suppressDefaults: true);
				writer.WriteCommentSummary(description); // vypisuje jen neprázdné

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

				string description = ColumnHelper.GetDescription(entityProperty.Column, suppressDefaults: true);

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

				if (ColumnHelper.GetBoolExtendedProperty(column, "ReadOnly") == false)
				{
					writer.WriteLine("[ReadOnly(false)]");
				}

				Type type = Helpers.TypeHelper.GetPropertyType(fk?.ForeignKeyProperty ?? entityProperty);
				if (!column.Nullable && type?.IsValueType == false)
				{
					writer.WriteLine("[Required]");
				}

				string propertyAccessModifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "PropertyAccessModifier");
				string getAccessModifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "GetAccessModifier");
				string setAccessModifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "SetAccessModifier");
				var accessModifierAttributeBuilder = new AttributeStringBuilder("AccessModifier");
				if (!String.IsNullOrEmpty(propertyAccessModifier))
				{
					accessModifierAttributeBuilder.AddParameter("PropertyAccessModifier", $"\"{propertyAccessModifier}\"");
				}
				if (!String.IsNullOrEmpty(getAccessModifier))
				{
					accessModifierAttributeBuilder.AddParameter("GetAccessModifier", $"\"getAccessModifier\"");
				}

				if (!String.IsNullOrEmpty(setAccessModifier))
				{
					accessModifierAttributeBuilder.AddParameter("SetAccessModifier", $"\"setAccessModifier\"");
				}

				if (accessModifierAttributeBuilder.Parameters.Count > 0)
				{
					writer.WriteLine(accessModifierAttributeBuilder.ToString());
				}

				var cloneMode = ColumnHelper.GetCloneMode(column);
				if (cloneMode != CloneMode.Shallow)
				{
					writer.WriteLine($"[CloneMode(CloneMode.{cloneMode.ToString()})]");
				}

				WriteDefault(writer, modelClass.Table, column, type);

				if (PropertyHelper.IsString(column))
				{
					int maxLength = column.DataType.MaximumLength;
					writer.WriteLine((maxLength == -1) ? "[MaxLength]" : $"[MaxLength({maxLength})]");
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

				if (BusinessLayerGenerator.Helpers.TypeHelper.IsNonstandardType(column))
				{
					writer.WriteLine($"[PropertyType(\"{BusinessLayerGenerator.Helpers.TypeHelper.GetPropertyTypeName(column)}\")]");
				}

				if (ColumnHelper.GetBoolExtendedProperty(column, "CheckForeignKeyName") == false)
				{
					writer.WriteLine("[CheckForeignKeyName(false)]");
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

			    if (attributeBuilder.Parameters.Any())
			    {
			        writer.WriteLine(attributeBuilder.ToString());
			    }

				if ((collectionProperty.CollectionProperty.IsOneToMany && (collectionProperty.CollectionProperty.CloneMode != CloneMode.No))
					|| (collectionProperty.CollectionProperty.IsManyToMany && (collectionProperty.CollectionProperty.CloneMode != CloneMode.Shallow)))
				{
					writer.WriteLine($"[CloneMode(CloneMode.{collectionProperty.CollectionProperty.CloneMode.ToString()})]");
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

		private static void WriteDefault(CodeWriter writer, Table table, Column column, Type type)
		{
			if ((column.DefaultConstraint != null) && ((type != typeof(string)) || (column.DefaultConstraint.Text != "('')")))
			{
				string defaultValue = column.DefaultConstraint.Text;
				Action warningAction = () => ConsoleHelper.WriteLineWarning("Tabulka {0}, sloupec {1}: Výchozí hodnotu {2} se nepodařilo zpracovat.", table.Name, column.Name, defaultValue);

				string defaultValueTrimmed = defaultValue.TrimStart('(').TrimEnd(')');

				if (column.DataType.SqlDataType == SqlDataType.Bit)
				{
					switch (defaultValueTrimmed)
					{
						case "0":
							writer.WriteLine("[DefaultValue(false)]");
							return;
						case "1":
							writer.WriteLine("[DefaultValue(true)]");
							return;
						default:
							// NOOP
							break; // spadne do warningu níže
					}
				}

				if (column.Nullable && ((column.DataType.SqlDataType == SqlDataType.Decimal) || (column.DataType.SqlDataType == SqlDataType.Money)))
				{
					// [DefaultValue(0)]
					// public Decimal? TotalAmountAmount { get; set; }
					// Cannot set default value '0' of type 'System.Int32' on property 'TotalAmountAmount' of type 'System.Nullable`1[System.Decimal]' in entity type '
					// workaround:
					writer.WriteLine($"[DefaultValue(typeof(Decimal), \"{defaultValueTrimmed}\")]");
					return;
				}

				if ((column.DataType.SqlDataType == SqlDataType.Int) || (column.DataType.SqlDataType == SqlDataType.SmallInt) || (column.DataType.SqlDataType == SqlDataType.Float) || (column.DataType.SqlDataType == SqlDataType.Decimal) || (column.DataType.SqlDataType == SqlDataType.Money))
				{
					// u floatu, decimalu a money spoléháme, že je zapsáno rozumně (neotřebujeme f či M na konci, tj. stačí 0, 0.0 a netřeba 0f, 0.0f, 0M, 0.0M);
					writer.WriteLine($"[DefaultValue({defaultValueTrimmed})]");
					return;
				}
			
				if (defaultValue.ToLower() == "(getdate())")
				{
					writer.WriteLine("[DefaultValueSql(DefaultValueSql.GetDate)]");
					return;
				}

				if ((column.DataType.SqlDataType == SqlDataType.DateTime) || (column.DataType.SqlDataType == SqlDataType.SmallDateTime) || (column.DataType.SqlDataType == SqlDataType.Date))
				{
					var dateTimeDefault = defaultValueTrimmed.Trim('\'');
					writer.WriteLine($"[DefaultValue(typeof(DateTime), \"{dateTimeDefault}\")]");
					return;
				}

				if ((column.DataType.SqlDataType == SqlDataType.NVarChar) || (column.DataType.SqlDataType == SqlDataType.NVarCharMax))
				{
					var stringDefault = defaultValueTrimmed.TrimStart('N').Trim('\'').Replace("\\", "\\\\").Replace("\"", "\\\"");
					writer.WriteLine($"[DefaultValue(\"{stringDefault}\")]");
					return;
				}

				warningAction();
			}
		}

		public static void WriteNamespaceClassEnd(CodeWriter writer)
		{
			writer.WriteLine("}");
			writer.WriteLine("}");
		}
	}
}