﻿using Havit.Business.BusinessLayerGenerator.Writers;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class Autogenerated
{
	/// <summary>
	/// Zapíše kód identifikující automaticky generovaný kód.
	/// </summary>
	public static void WriteAutogeneratedNoCodeHere(CodeWriter writer)
	{
		writer.WriteLine("//------------------------------------------------------------------------------");
		writer.WriteLine("// <auto-generated>");
		writer.WriteLine("//     This code was generated by a tool.");
		writer.WriteLine("//     Changes to this file will be lost if the code is regenerated.");
		writer.WriteLine("// </auto-generated>");
		writer.WriteLine("//------------------------------------------------------------------------------");
		writer.WriteLine();
	}
}
