﻿using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Mapping.Internal;
using Havit.Data.Entity.Patterns.SoftDeletes;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;

public class InterfaceDataSourceModelSource : IModelSource<InterfaceDataSourceModel>
{
	private readonly DbContext dbContext;
	private readonly IProject modelProject;
	private readonly IProject dataLayerProject;
	private readonly ISoftDeleteManager softDeleteManager;

	public InterfaceDataSourceModelSource(DbContext dbContext, IProject modelProject, IProject dataLayerProject, ISoftDeleteManager softDeleteManager)
	{
		this.dbContext = dbContext;
		this.modelProject = modelProject;
		this.dataLayerProject = dataLayerProject;
		this.softDeleteManager = softDeleteManager;
	}

	public IEnumerable<InterfaceDataSourceModel> GetModels()
	{
		return (from registeredEntity in dbContext.GetRegisteredEntities()
				select new InterfaceDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					InterfaceDataSourceName = "I" + registeredEntity.ClassName + "DataSource",
					ModelClassFullName = registeredEntity.FullName
				}).ToList();
	}

	private string GetNamespaceName(string namespaceName)
	{
		string modelProjectNamespace = modelProject.GetProjectRootNamespace();
		if (namespaceName.StartsWith(modelProjectNamespace))
		{
			return dataLayerProject.GetProjectRootNamespace() + ".DataSources" + namespaceName.Substring(modelProjectNamespace.Length);
		}
		else
		{
			return namespaceName + ".DataSources";
		}
	}

}
