﻿using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses;

public class MetadataClassFileNamingService : FileNamingServiceBase<MetadataClass>
{
	public MetadataClassFileNamingService(IMetadataProject metadataProject)
		: base(metadataProject)
	{

	}

	protected override string GetClassName(MetadataClass model)
	{
		return model.ClassName;
	}

	protected override string GetNamespaceName(MetadataClass model)
	{
		return model.NamespaceName;
	}
}
