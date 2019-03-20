using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectMoneyProperties
	{
		public static void Write(CodeWriter writer, Table table)
		{
			string[] moneys = MoneyHelper.GetListMoneyProperties(table);

			if (moneys.Length == 0)
			{
				return;
			}

			foreach (string moneyProperty in moneys)
			{
				WriteMoneyProperty(writer, table, moneyProperty);
			}

		}

		private static void WriteMoneyProperty(CodeWriter writer, Table table, string moneyProperty)
		{
			string moneyField = MoneyHelper.GetMoneyFieldName(moneyProperty);
			Column amountColumn = MoneyHelper.GetMoneyAmountColumn(table, moneyProperty);
			Column currencyColumn = MoneyHelper.GetMoneyCurrencyColumn(table, moneyProperty);
			string moneyTypeName = MoneyHelper.GetMoneyTypeName();

			bool readOnly = TableHelper.IsReadOnly(table) || ColumnHelper.IsReadOnly(amountColumn) || ColumnHelper.IsReadOnly(currencyColumn);

			string description = ColumnHelper.GetDescription(amountColumn);
			writer.WriteCommentSummary(description);
			writer.WriteLine(String.Format("public virtual {0} {1}", moneyTypeName, moneyProperty));
			writer.WriteLine("{");

			writer.WriteLine("get");
			writer.WriteLine("{");
			writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(moneyTypeName));
			writer.WriteMicrosoftContract("");
			writer.WriteLine("EnsureLoaded();");
			writer.WriteLine(String.Format("if (!{0}IsUpToDate)", moneyField));
			writer.WriteLine("{");
//			if (!amountColumn.Nullable)
//			{
				writer.WriteLine(String.Format("{0} = new {1}({2}, {3});", moneyField, moneyTypeName, PropertyHelper.GetPropertyName(amountColumn), PropertyHelper.GetPropertyName(currencyColumn)));
//			}
//			else
//			{
//				writer.WriteLine(String.Format("{0} = (({2} == null) && ({3} == null)) ? null : new {1}({2}, {3});", moneyField, moneyTypeName, PropertyHelper.GetPropertyName(amountColumn), PropertyHelper.GetPropertyName(currencyColumn)));
//			}

			writer.WriteLine(String.Format("{0}IsUpToDate = true;", moneyField));
			writer.WriteLine("}");
			writer.WriteLine(String.Format("return {0};", moneyField));
			writer.WriteLine("}");

			if (!readOnly)
			{
				writer.WriteLine("set");
				writer.WriteLine("{");
				writer.WriteLine("EnsureLoaded();");

				if (!amountColumn.Nullable)
				{
					writer.WriteLine("if (value == null)");
					writer.WriteLine("{");
					writer.WriteLine(String.Format("throw new InvalidOperationException(\"Value is null but cannot be set to not-null {0} property.\");", PropertyHelper.GetPropertyName(amountColumn)));
					writer.WriteLine("}");

					writer.WriteLine("if (value.Amount == null)");
					writer.WriteLine("{");
					writer.WriteLine(String.Format("throw new InvalidOperationException(\"Amount is null but cannot be set to not-null {0} property.\");", PropertyHelper.GetPropertyName(amountColumn)));
					writer.WriteLine("}");

					writer.WriteLine(String.Format("{0} = value.Amount.Value;", PropertyHelper.GetPropertyName(amountColumn)));
					writer.WriteLine(String.Format("{0} = value.Currency;", PropertyHelper.GetPropertyName(currencyColumn)));
					writer.WriteLine(String.Format("{0} = value;", moneyField));
					writer.WriteLine(String.Format("{0}IsUpToDate = true;", moneyField));
				}
				else
				{
					writer.WriteLine("if (value != null)");
					writer.WriteLine("{");				
					writer.WriteLine(String.Format("{0} = value.Amount;", PropertyHelper.GetPropertyName(amountColumn)));
					writer.WriteLine(String.Format("{0} = value.Currency;", PropertyHelper.GetPropertyName(currencyColumn)));
					writer.WriteLine(String.Format("{0} = value;", moneyField));
					writer.WriteLine(String.Format("{0}IsUpToDate = true;", moneyField));
					writer.WriteLine("}");
					writer.WriteLine("else");
					writer.WriteLine("{");
					writer.WriteLine(String.Format("{0} = null;", PropertyHelper.GetPropertyName(amountColumn)));
					writer.WriteLine(String.Format("{0} = null;", PropertyHelper.GetPropertyName(currencyColumn)));
					writer.WriteLine("}");
				}

				writer.WriteLine("}");
			}
			writer.WriteLine("}");
			writer.WriteLine(String.Format("private {0} {1};", moneyTypeName, moneyField));
			writer.WriteLine(String.Format("private bool {0}IsUpToDate;", moneyField));
			writer.WriteLine();
		}
	}
}