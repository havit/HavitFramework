using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

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
		return (from registeredEntity in dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
				select new DbDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					InterfaceDataSourceFullName = "I" + registeredEntity.ClrType.Name + "DataSource",
					DbDataSourceClassName = registeredEntity.ClrType.Name + "DbDataSource",
					ModelClassFullName = registeredEntity.ClrType.FullName
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
