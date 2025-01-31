﻿using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories;

public class DbRepositoryBaseGeneratedFileNamingService : FileNamingServiceBase<RepositoryModel>
{
	public DbRepositoryBaseGeneratedFileNamingService(IProject project)
		: base(project)
	{

	}

	protected override string GetClassName(RepositoryModel model)
	{
		return model.DbRepositoryBaseName;
	}

	protected override string GetNamespaceName(RepositoryModel model)
	{
		return model.NamespaceName;
	}
}
