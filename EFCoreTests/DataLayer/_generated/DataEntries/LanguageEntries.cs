﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Havit.EFCoreTests.DataLayer.DataEntries;

[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public class LanguageEntries : Havit.Data.Patterns.DataEntries.DataEntries<Havit.EFCoreTests.Model.Language, System.Int32>, ILanguageEntries
{
	private Havit.EFCoreTests.Model.Language czech;
	private Havit.EFCoreTests.Model.Language english;

	public Havit.EFCoreTests.Model.Language Czech => czech ??= GetEntry(Havit.EFCoreTests.Model.Language.Entry.Czech);
	public Havit.EFCoreTests.Model.Language English => english ??= GetEntry(Havit.EFCoreTests.Model.Language.Entry.English);

	public LanguageEntries(Havit.Data.Patterns.DataEntries.IDataEntrySymbolService<Havit.EFCoreTests.Model.Language, System.Int32> dataEntrySymbolService, Havit.EFCoreTests.DataLayer.Repositories.ILanguageRepository repository)
		: base(dataEntrySymbolService, repository)
	{
	}
}
