using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

public class InterfaceDataSourceModelSource : IModelSource<InterfaceDataSourceModel>
{
	private readonly DbContext dbContext;
	private readonly IProject modelProject;
	private readonly IProject dataLayerProject;

	public InterfaceDataSourceModelSource(DbContext dbContext, IProject modelProject, IProject dataLayerProject)
	{
		this.dbContext = dbContext;
		this.modelProject = modelProject;
		this.dataLayerProject = dataLayerProject;
	}

	public IEnumerable<InterfaceDataSourceModel> GetModels()
	{
		return (from registeredEntity in dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
				select new InterfaceDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					InterfaceDataSourceName = "I" + registeredEntity.ClrType.Name + "DataSource",
					ModelClassFullName = registeredEntity.ClrType.FullName
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
