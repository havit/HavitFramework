using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class LocalizationGenerator
	{
		public static void WriteLocalization(CodeWriter writer, Table table)
		{
			if (LocalizationHelper.IsLocalizedTable(table))
			{
				Table localizationTable = LocalizationHelper.GetLocalizationTable(table);
				writer.WriteOpenRegion("CreateLocalization");
				writer.WriteCommentSummary("Vytvoří položku lokalizace pro daný jazyk.");
				writer.WriteLine(String.Format("public {0} CreateLocalization({1} language)", ClassHelper.GetClassName(localizationTable), ClassHelper.GetClassFullName(LanguageHelper.GetLanguageTable())));
				writer.WriteLine("{");
				if (!TableHelper.IsReadOnly(localizationTable) && !TableHelper.OmitCreateObjectMethod(localizationTable))
				{
					writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetClassName(localizationTable)));
					writer.WriteMicrosoftContract("");
					writer.WriteLine(String.Format("return {0}.CreateObject(({1})this, language);", ClassHelper.GetClassName(localizationTable), ClassHelper.GetClassName(table)));
				}
				else
				{
					writer.WriteLine("throw new InvalidOperationException(\"Metoda CreateLocalization není podporována na read-only objektech a objektech s negenerovanou metodou CreateObject.\");");
				}
				writer.WriteLine("}");
				writer.WriteCloseRegion();

				writer.WriteOpenRegion("ILocalizable interface implementation");
				writer.WriteCommentSummary("Vytvoří položku lokalizace pro daný jazyk.");
				writer.WriteLine("BusinessObjectBase ILocalizable.CreateLocalization(ILanguage language)");
				writer.WriteLine("{");
				writer.WriteLine(String.Format("return this.CreateLocalization(({0})language);", ClassHelper.GetClassFullName(LanguageHelper.GetLanguageTable())));
				writer.WriteLine("}");

				writer.WriteCommentSummary("Vytvoří položku lokalizace pro daný jazyk.");
				writer.WriteLine("ILocalizationCollection ILocalizable.Localizations");
				writer.WriteLine("{");
				writer.WriteLine("get");
				writer.WriteLine("{");
				writer.WriteLine("return this.Localizations;");
				writer.WriteLine("}");
				writer.WriteLine("}");
				writer.WriteCloseRegion();
			}
		}
	}
}
