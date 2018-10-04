using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Settings;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class SqlCe35Localizations
	{
		#region Write
		public static void Write(CodeWriter writer, Table table)
		{
			if (GeneratorSettings.TargetPlatform != TargetPlatform.SqlServerCe35)
			{
				return;
			}

			if (!LocalizationHelper.IsLocalizedTable(table))
			{
				return;
			}

			Table localizationTable = LocalizationHelper.GetLocalizationTable(table);
			writer.WriteLine(String.Format("public virtual {0} Localizations", ClassHelper.GetCollectionClassFullName(localizationTable)));
			writer.WriteLine("{");
			writer.WriteLine("get");
			writer.WriteLine("{");

			writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetCollectionClassFullName(localizationTable)));
			writer.WriteMicrosoftContract("");

			if (TableHelper.IsCachable(localizationTable))
			{
				writer.WriteLine("if (_localizations == null)");
				writer.WriteLine("{");
				writer.WriteLine(String.Format("_localizations = {0}.GetAll().FindAll(delegate({0} item) {{ return item.{1} == this; }});",
					ClassHelper.GetClassFullName(localizationTable),
					PropertyHelper.GetPropertyName(LocalizationHelper.GetParentLocalizationColumn(localizationTable))));
				writer.WriteLine("}");
				writer.WriteLine("return _localizations;");
			}
			else
			{
				throw new NotImplementedException("Není implementováno generování property Localizations pro necachovanou lokalizační tabulku a target platform SqlServerCe35.");
			}
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine(String.Format("private {0} _localizations;", ClassHelper.GetCollectionClassFullName(localizationTable)));
			writer.WriteLine();
		}
		#endregion
	}
}
