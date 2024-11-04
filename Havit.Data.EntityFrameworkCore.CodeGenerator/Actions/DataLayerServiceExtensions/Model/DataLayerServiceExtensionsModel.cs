using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;

public class DataLayerServiceExtensionsModel
{
	public string NamespaceName { get; set; }
	public List<DbDataSourceModel> DataSourceModels { get; set; }
	public List<RepositoryModel> RepositoryModels { get; set; }
}
