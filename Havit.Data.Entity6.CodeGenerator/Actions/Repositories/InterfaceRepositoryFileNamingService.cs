﻿using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories;

public class InterfaceRepositoryFileNamingService : InterfaceRepositoryGeneratedFileNamingService
{
	public InterfaceRepositoryFileNamingService(IProject project)
		: base(project)
	{

	}

	protected override bool UseGeneratedFolder => false;
}
