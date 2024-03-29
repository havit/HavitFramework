﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Template;

public partial class FakeDataSourceTemplate : ITemplate
{
	protected FakeDataSourceModel Model { get; private set; }

	public FakeDataSourceTemplate(FakeDataSourceModel model)
	{
		this.Model = model;
	}
}
