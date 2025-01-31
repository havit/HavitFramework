﻿using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Templates;

public partial class DbRepositoryTemplate : ITemplate
{
	protected RepositoryModel Model { get; private set; }

	public DbRepositoryTemplate(RepositoryModel model)
	{
		this.Model = model;
	}
}
