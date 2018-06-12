using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Writers;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectConstructors
	{
		#region WriteConstructors
		public static void WriteConstructors(CodeWriter writer, string className, string pkFieldName, bool readOnly, bool baseClass)
		{
			writer.WriteOpenRegion("Constructors");

			if (!baseClass)
			{
				writer.WriteCommentSummary("Vytvoří instanci objektu jako nový prvek.");
				// čas od času se může hodit konstruktor bez parametrů
				writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
				writer.WriteGeneratedCodeAttribute();
				writer.WriteLine("protected " + className + "() : this(ConnectionMode.Connected)");
				writer.WriteLine("{");
				writer.WriteLine("}");
				writer.WriteLine();
			}

			writer.WriteCommentSummary("Vytvoří instanci objektu jako nový prvek.");
			writer.WriteCommentLine("<param name=\"connectionMode\">Režim business objektu.</param>");
			writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
			writer.WriteGeneratedCodeAttribute();
			writer.WriteLine("protected " + className + "(ConnectionMode connectionMode) : base(connectionMode)");
			writer.WriteLine("{");
			writer.WriteLine("}");
			writer.WriteLine();

			writer.WriteCommentSummary("Vytvoří instanci existujícího objektu.");
			writer.WriteCommentLine("<param name=\"id\">" + pkFieldName + " (PK).</param>");
			writer.WriteCommentLine("<param name=\"connectionMode\">Režim business objektu.</param>");

			if (!baseClass)
			{
				writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
				writer.WriteGeneratedCodeAttribute();
			}

			if (baseClass)
			{
				writer.WriteLine("protected " + className + "(int id, ConnectionMode connectionMode) : base(id, connectionMode)");
			}
			else
			{
				writer.WriteLine("protected " + className + "(int id, ConnectionMode connectionMode = ConnectionMode.Connected) : base(id, connectionMode)");
			}

			writer.WriteLine("{");
			writer.WriteLine("}");
			writer.WriteLine();

			writer.WriteCommentSummary("Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.");
			writer.WriteCommentLine("<param name=\"id\">" + pkFieldName + " (PK).</param>");
			writer.WriteCommentLine("<param name=\"record\">DataRecord s daty objektu (i částečnými).</param>");

			if (!baseClass)
			{
				writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
				writer.WriteGeneratedCodeAttribute();
			}

			writer.WriteLine("protected " + className + "(int id, DataRecord record) : base(id, record)");
			writer.WriteLine("{");
			writer.WriteLine("}");

			writer.WriteCloseRegion();
		}
		#endregion
	}
}
