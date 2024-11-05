using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;

public class DataLayerServiceExtensionsModel
{
	public string NamespaceName { get; set; }
	public List<DataEntriesModel> DataEntries { get; set; }
	public List<DbDataSourceModel> DataSources { get; set; }
	public List<RepositoryModel> Repositories { get; set; }
}
