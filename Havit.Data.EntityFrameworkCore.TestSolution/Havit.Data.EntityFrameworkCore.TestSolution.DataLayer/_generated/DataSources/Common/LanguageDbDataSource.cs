﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

namespace Havit.Data.EntityFrameworkCore.TestSolution.DataLayer.DataSources.Common;

[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public partial class LanguageDbDataSource : DbDataSource<Havit.Data.EntityFrameworkCore.TestSolution.Model.Common.Language>, ILanguageDataSource
{
	public LanguageDbDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
		: base(dbContext, softDeleteManager)
	{
	}
}
