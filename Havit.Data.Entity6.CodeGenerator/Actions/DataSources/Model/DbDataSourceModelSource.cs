using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Mapping.Internal;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;

public class DbDataSourceModelSource : IModelSource<DbDataSourceModel>
{
	private readonly DbContext dbContext;
	private readonly IProject modelProject;
	private readonly IProject dataLayerProject;

	public DbDataSourceModelSource(DbContext dbContext, IProject modelProject, IProject dataLayerProject)
	{
		this.dbContext = dbContext;
		this.modelProject = modelProject;
		this.dataLayerProject = dataLayerProject;
	}

	public IEnumerable<DbDataSourceModel> GetModels()
	{
		return (from registeredEntity in dbContext.GetRegisteredEntities()
				select new DbDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					InterfaceDataSourceFullName = "I" + registeredEntity.ClassName + "DataSource",
					DbDataSourceClassName = registeredEntity.ClassName + "DbDataSource",
					ModelClassFullName = registeredEntity.FullName
				}).ToList();
	}

	// TODO: Opakující se kód (obdobný)
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
