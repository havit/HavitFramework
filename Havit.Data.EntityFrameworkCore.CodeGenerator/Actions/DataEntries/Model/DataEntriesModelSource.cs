using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;

public class DataEntriesModelSource : IModelSource<DataEntriesModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	private List<DataEntriesModel> _models;

	public DataEntriesModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
	}

	public List<DataEntriesModel> GetModels()
	{
		return _models ??= (
			from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
			let entriesEnumType = GetEntriesEnum(registeredEntity.ClrType)
			where (entriesEnumType != null)
			select new DataEntriesModel
			{
				UseDataEntrySymbolStorage = registeredEntity.FindPrimaryKey().Properties.Any(property =>
						// Snaha o identifikaci použití sloupce Identity
						// viz DbDataSeedProvider.PropertyIsIdentity
						property.ClrType == typeof(Int32) // Identity definujeme jen na typu Int32
						&& property.ValueGenerated.HasFlag(ValueGenerated.OnAdd) // Je zajištěno, že hodnotu generuje SQL Server
						&& String.IsNullOrEmpty(property.GetDefaultValueSql())), // Identita není použita, pokud je na sloupci definována výchozí hodnota pomocí SQL.
				NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
				InterfaceName = "I" + registeredEntity.ClrType.Name + "Entries",
				DbClassName = registeredEntity.ClrType.Name + "Entries",
				ModelClassFullName = registeredEntity.ClrType.FullName,
				ModelEntriesEnumerationFullName = registeredEntity.ClrType.FullName + ".Entry",
				RepositoryDependencyFullName = GetRepositoryDependencyFullName(registeredEntity.ClrType),
				Entries = System.Enum.GetNames(entriesEnumType)
					.OrderBy(item => item, StringComparer.InvariantCulture)
					.Select(item => new DataEntriesModel.Entry
					{
						PropertyName = item,
						FieldName = CammelCaseNamingStrategy.GetCammelCase(item),
						IsObsolete = IsValueObsolete(entriesEnumType, item),
						ObsoleteMessage = GetValueObsoleteMessage(entriesEnumType, item)
					})
					.ToList()
			}).ToList();
	}

	private Type GetEntriesEnum(Type type)
	{
		Type entriesType = type.GetNestedType("Entry");
		if ((entriesType != null) && (entriesType.IsEnum))
		{
			return entriesType;
		}
		return null;
	}

	private string GetNamespaceName(string namespaceName)
	{
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();
		if (namespaceName.StartsWith(modelProjectNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + ".DataEntries" + namespaceName.Substring(modelProjectNamespace.Length);
		}
		else
		{
			return namespaceName + ".DataSources";
		}
	}

	private string GetRepositoryDependencyFullName(Type entityType)
	{
		string entityNamespaceName = entityType.Namespace;
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();

		string repositoryNamespace = entityNamespaceName.StartsWith(modelProjectNamespace)
			? _dataLayerProject.GetProjectRootNamespace() + ".Repositories" + entityNamespaceName.Substring(modelProjectNamespace.Length)
			: entityNamespaceName + ".Repositories";

		return repositoryNamespace + ".I" + entityType.Name + "Repository";
	}

	private static bool IsValueObsolete(Type type, string value)
	{
		var fi = type.GetField(value);
		var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
		return (attributes != null) && (attributes.Length > 0);
	}

	private static string GetValueObsoleteMessage(Type type, string value)
	{
		var fi = type.GetField(value);
		var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
		return attributes.FirstOrDefault()?.Message;
	}
}
