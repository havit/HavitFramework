﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Attributes;

namespace Havit.EFCoreTests.DataLayer.DataSources.Fakes;

[Fake]
[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public class FakeStateLocalizationDataSource : FakeDataSource<Havit.EFCoreTests.Model.StateLocalization>, Havit.EFCoreTests.DataLayer.DataSources.IStateLocalizationDataSource
{
	public FakeStateLocalizationDataSource(params Havit.EFCoreTests.Model.StateLocalization[] data)
		: this((IEnumerable<Havit.EFCoreTests.Model.StateLocalization>)data)
	{			
	}

	public FakeStateLocalizationDataSource(IEnumerable<Havit.EFCoreTests.Model.StateLocalization> data, ISoftDeleteManager softDeleteManager = null)
		: base(data, softDeleteManager)
	{
	}
}