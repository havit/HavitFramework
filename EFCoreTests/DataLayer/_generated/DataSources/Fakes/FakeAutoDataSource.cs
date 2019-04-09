﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Attributes;

namespace Havit.EFCoreTests.DataLayer.DataSources.Fakes
{
	[Fake]
	[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
	public class FakeAutoDataSource : FakeDataSource<Havit.EFCoreTests.Model.Auto>, Havit.EFCoreTests.DataLayer.DataSources.IAutoDataSource
	{
		public FakeAutoDataSource(params Havit.EFCoreTests.Model.Auto[] data)
			: this((IEnumerable<Havit.EFCoreTests.Model.Auto>)data)
		{			
		}

		public FakeAutoDataSource(IEnumerable<Havit.EFCoreTests.Model.Auto> data, ISoftDeleteManager softDeleteManager = null)
			: base(data, softDeleteManager)
		{
		}
	}
}