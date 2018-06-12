using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectProperties
	{
		#region WriteProperties
		public static void WriteProperties(CodeWriter writer, Table table)
		{
			bool readonlyTable = TableHelper.IsReadOnly(table);

			foreach (Column column in TableHelper.GetPropertyColumns(table))
			{
				string columnDescription = ColumnHelper.GetDescription(column);
				string dataTypeDescription = column.DataType.ToString();
				if (String.IsNullOrEmpty(dataTypeDescription)) // datový typ dává prázdný řetězec
				{
					dataTypeDescription = column.DataType.SqlDataType.ToString();
				}

				if (PropertyHelper.IsString(column) && !TypeHelper.IsNonstandardType(column))
				{
					if (ColumnHelper.IsLengthUnlimitedTextColumn(column))
					{
						dataTypeDescription += "(MAX)";
					}
					else
					{
						dataTypeDescription += String.Format("({0})", column.DataType.MaximumLength);
					}
				}
				string nullableDescription = column.Nullable ? "nullable" : "not-null";
				string readonlyDescription = ColumnHelper.IsReadOnly(column) ? ", read-only" : "";
				string defaultText = ColumnHelper.GetColumnDefaultValueText(column);
				string defaultDescrption = String.IsNullOrEmpty(defaultText) ? "" : ", default " + defaultText;

				string description = String.Format(
					"{0} [{1}, {2}{3}{4}]",
					columnDescription, // 0
					dataTypeDescription, // 1
					nullableDescription, // 2
					readonlyDescription, // 3
					defaultDescrption); // 4

				WritePropertyHolder(writer, table, column, PropertyHelper.GetPropertyName(column), TypeHelper.GetPropertyTypeName(column), readonlyTable || ColumnHelper.IsReadOnly(column), description);
			}

			foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(table))
			{
				WriteCollectionProperty(writer, table, collectionProperty);
			}
		}
		#endregion

		#region WritePropertyHolder
		public static void WritePropertyHolder(CodeWriter writer, Table table, Column column, string propertyName, string typeName, bool isReadOnly, string description)
		{
			writer.WriteCommentSummary(description);
			string accessModifierText = PropertyHelper.GetPropertyAccessModifier(column);
			string getterAccessModifierText = PropertyHelper.GetPropertyGetterAccessModifier(column);
			string setterAccessModifierText = (isReadOnly || ColumnHelper.IsReadOnly(column)) ? "private" : PropertyHelper.GetPropertySetterAccessModifier(column);

			if (!String.IsNullOrEmpty(getterAccessModifierText) && !String.IsNullOrEmpty(setterAccessModifierText))
			{
				accessModifierText = getterAccessModifierText; // předpokládáme vyšší viditelnost getteru
				getterAccessModifierText = "";
			}

			if (accessModifierText != "private")
			{
				if (PropertyHelper.ExistsPropertyInBaseType(table, propertyName))
				{
					accessModifierText += " override";
				}
				else
				{
					accessModifierText += " virtual";
				}
			}

			string propertyHolderAccessModifier = "protected";
			if (TypeHelper.IsBusinessObjectReference(column)
				&& ((TableHelper.GetAccessModifier(ColumnHelper.GetReferencedTable(column)) == "internal")
				|| (PropertyHelper.IsInternalDueCollectionClonning(column))))
			{
				propertyHolderAccessModifier = "internal";
			}

			writer.WriteLine(String.Format("{0} {1} {2}", accessModifierText, typeName, propertyName));
			writer.WriteLine("{");

			writer.WriteLine(String.Format("{0} get", getterAccessModifierText).Trim());
			writer.WriteLine("{");

			if ((typeName == "string") && !PropertyHelper.ExistsPropertyInBaseType(table, propertyName)) // pokud existuje, musí se contract definovat v base
			{
				writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull("string"));
			}
			writer.WriteLine("EnsureLoaded();");
			writer.WriteLine("return " + PropertyHelper.GetPropertyHolderName(propertyName) + ".Value;");
			writer.WriteLine("}");

			writer.WriteLine(String.Format("{0} set", setterAccessModifierText).Trim());
			writer.WriteLine("{");
			writer.WriteLine("EnsureLoaded();");
			if (TypeHelper.IsXml(column))
			{
				writer.WriteLine(String.Format("if ({0}.Value != null)", PropertyHelper.GetPropertyHolderName(propertyName)));
				writer.WriteLine("{");
				writer.WriteLine(String.Format("{0}.Value.NodeChanged -= {1}_NodeChanged;", PropertyHelper.GetPropertyHolderName(propertyName), propertyName));
				writer.WriteLine("}");
			}

			if (typeName == "string")
			{
				writer.WriteLine("if (value == null)");
				writer.WriteLine("{");
				writer.WriteLine(PropertyHelper.GetPropertyHolderName(propertyName) + ".Value = String.Empty;");
				writer.WriteLine("}");
				writer.WriteLine("else");
				writer.WriteLine("{");
				string trim = "";
				if (ColumnHelper.IsStringTrimming(column))
				{
					trim = ".Trim()";
				}
				writer.WriteLine(String.Format("{0}.Value = value{1};", PropertyHelper.GetPropertyHolderName(propertyName), trim));
				writer.WriteLine("}");
			}
			else
				if (TypeHelper.IsDateTime(column) && TypeHelper.IsDateOnly(column.DataType))
				{
					if (column.Nullable)
					{
						writer.WriteLine(PropertyHelper.GetPropertyHolderName(propertyName) + ".Value = (value == null) ? (DateTime?)null : value.Value.Date;");
					}
					else
					{
						writer.WriteLine(PropertyHelper.GetPropertyHolderName(propertyName) + ".Value = value.Date;");
					}
				}
				else
				{
					if (MoneyHelper.FormsMoneyStructure(column))
					{

						writer.WriteLine(String.Format("if (value != {0}.Value)", PropertyHelper.GetPropertyHolderName(column)));
						writer.WriteLine("{");
						writer.WriteLine(String.Format("{0}IsUpToDate = false;", MoneyHelper.GetMoneyFieldName(MoneyHelper.ShortcutColumnNameToMoneyPropertyName(column.Name))));
						writer.WriteLine("}");
					}

					writer.WriteLine(PropertyHelper.GetPropertyHolderName(propertyName) + ".Value = value;");
				}

			if (TypeHelper.IsXml(column))
			{
				writer.WriteLine("if (value != null)");
				writer.WriteLine("{");
				writer.WriteLine(String.Format("value.NodeChanged += {0}_NodeChanged;", propertyName));
				writer.WriteLine("}");
			}

			writer.WriteLine("}");

			writer.WriteLine("}");
			writer.WriteCommentSummary(String.Format("PropertyHolder pro vlastnost {0}.", propertyName));
			writer.WriteLine("[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]");
			writer.WriteLine(String.Format("{0} PropertyHolder<{1}> {2};",
				propertyHolderAccessModifier,
				typeName,
				PropertyHelper.GetPropertyHolderName(propertyName)));

			writer.WriteLine();

			if (TypeHelper.IsXml(column))
			{
				writer.WriteCommentSummary(String.Format("Obsluha události změny XmlDocumentu vlastnosti {0}.", propertyName));
				writer.WriteLine(String.Format("private void {0}_NodeChanged(object sender, XmlNodeChangedEventArgs e)", propertyName));
				writer.WriteLine("{");
				if (isReadOnly)
				{
					writer.WriteLine(String.Format("throw new InvalidOperationException(\"Vlastnost \\\"{0}\\\" je určena jen ke čtení.\");", propertyName));
				}
				else
				{
					writer.WriteLine(String.Format("{0}.IsDirty = true;", PropertyHelper.GetPropertyHolderName(propertyName)));
				}
				writer.WriteLine("}");
				writer.WriteLine();
			}

		}
		#endregion

		#region WriteCollectionProperty
		public static void WriteCollectionProperty(CodeWriter writer, Table table, CollectionProperty collectionProperty)
		{
			writer.WriteCommentSummary(collectionProperty.Description);

			writer.WriteLine(String.Format("{0} {1}{2} {3}",
				collectionProperty.PropertyAccessModifier,
				collectionProperty.PropertyAccessModifier == "private" ? "" : "virtual ",
				ClassHelper.GetCollectionClassFullName(collectionProperty.TargetTable),
				collectionProperty.PropertyName));
			writer.WriteLine("{");
			writer.WriteLine("get");
			writer.WriteLine("{");

			writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetCollectionClassFullName(collectionProperty.TargetTable)));
			// Negenerovat test prvků kolekce pomocí All! Výkon!
			//writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultCollectionDoesNotContainNull(ClassHelper.GetCollectionClassFullName(collectionProperty.TargetTable)));
			writer.WriteMicrosoftContract("");

			writer.WriteLine("EnsureLoaded();");
			if (collectionProperty.LoadAll)
			{
				writer.WriteLine(String.Format("if (!_{0}LoadAllPerformed)", ConventionsHelper.GetCammelCase(collectionProperty.PropertyName)));
				writer.WriteLine("{");
				writer.WriteLine(String.Format("{0}.Value.LoadAll();", PropertyHelper.GetPropertyHolderName(collectionProperty.PropertyName)));
				writer.WriteLine(String.Format("_{0}LoadAllPerformed = true;", ConventionsHelper.GetCammelCase(collectionProperty.PropertyName)));
				writer.WriteLine("}");
			}
			writer.WriteLine("return " + PropertyHelper.GetPropertyHolderName(collectionProperty.PropertyName) + ".Value;");
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteCommentSummary(String.Format("PropertyHolder pro vlastnost {0}.", collectionProperty.PropertyName));
			writer.WriteLine("[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]");
			writer.WriteLine(String.Format("{0} CollectionPropertyHolder<{1}, {2}> {3};",
				TableHelper.GetAccessModifier(collectionProperty.TargetTable) == "internal" ? "internal" : "protected",
				ClassHelper.GetCollectionClassFullName(collectionProperty.TargetTable),
				ClassHelper.GetClassFullName(collectionProperty.TargetTable),
				PropertyHelper.GetPropertyHolderName(collectionProperty.PropertyName)));

			if (collectionProperty.LoadAll)
			{
				writer.WriteLine(String.Format("private bool _{0}LoadAllPerformed = false;", ConventionsHelper.GetCammelCase(collectionProperty.PropertyName)));
			}

			if (collectionProperty.IsOneToMany && !TableHelper.IsReadOnly(table))
			{
				// zapíšeme proměnnou, ve které budeme držet objeky načtené z databáze
				writer.WriteLine(String.Format("private {0} _loaded{1}Values;",
					ClassHelper.GetCollectionClassFullName(collectionProperty.TargetTable),
					collectionProperty.PropertyName));
			}
			writer.WriteLine();
		}
		#endregion
	}
}
