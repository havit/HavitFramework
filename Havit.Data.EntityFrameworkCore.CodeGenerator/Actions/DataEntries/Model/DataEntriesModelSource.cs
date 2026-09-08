using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;

public class DataEntriesModelSource : IModelSource<DataEntriesModel>, IModelSourceErrorsProvider
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	// Tento model source je DI singleton sdílený mezi generátory, které běží paralelně (viz DataLayerGeneratorRunner.Parallel.ForEachAsync).
	// Lazy (výchozí ExecutionAndPublication) zajistí, že se modely vyhodnotí právě jednou i při souběžném přístupu z více generátorů.
	private readonly Lazy<List<DataEntriesModel>> _models;

	public DataEntriesModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
		_models = new Lazy<List<DataEntriesModel>>(GetModelsCore);
	}

	public List<DataEntriesModel> GetModels() => _models.Value;

	private List<DataEntriesModel> GetModelsCore()
	{
		return (
			from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
			let entriesEnumType = GetEntriesEnum(registeredEntity.ClrType)
			where (entriesEnumType != null)
			// Entity s data entries (vnořeným enumem Entry) musí mít právě jeden primární klíč; ostatní jsou hlášeny v GetModelErrors a do generování nevstupují.
			where (registeredEntity.FindPrimaryKey()?.Properties.Count == 1)
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
				ModelClassPrimaryKeyTypeName = registeredEntity.FindPrimaryKey().Properties.Single().ClrType.FullName,
				ModelEntriesEnumerationFullName = registeredEntity.ClrType.FullName + ".Entry",
				RepositoryDependencyFullName = GetRepositoryDependencyFullName(registeredEntity.ClrType),
				Entries = System.Enum.GetNames(entriesEnumType)
					.OrderBy(item => item, StringComparer.InvariantCulture)
					.Select(item => new DataEntriesModel.Entry
					{
						PropertyName = item,
						FieldName = FieldNamingStrategy.GetFieldName(item),
						IsObsolete = IsValueObsolete(entriesEnumType, item),
						ObsoleteMessage = GetValueObsoleteMessage(entriesEnumType, item)
					})
					.ToList()
			}).ToList();
	}

	public IEnumerable<string> GetModelErrors()
	{
		return from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
			   where GetEntriesEnum(registeredEntity.ClrType) != null
			   where registeredEntity.FindPrimaryKey()?.Properties.Count != 1
			   select $"Entity {registeredEntity.ClrType.FullName} has a nested Entry enum (data entries) but does not have exactly one primary key property.";
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
		if (NamespaceHelper.TryGetRelativeNamespace(namespaceName, modelProjectNamespace, out string relativeNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + ".DataEntries" + relativeNamespace;
		}
		else
		{
			return namespaceName + ".DataEntries";
		}
	}

	private string GetRepositoryDependencyFullName(Type entityType)
	{
		string entityNamespaceName = entityType.Namespace;
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();

		string repositoryNamespace = NamespaceHelper.TryGetRelativeNamespace(entityNamespaceName, modelProjectNamespace, out string relativeNamespace)
			? _dataLayerProject.GetProjectRootNamespace() + ".Repositories" + relativeNamespace
			: entityNamespaceName + ".Repositories";

		return repositoryNamespace + ".I" + entityType.Name + "Repository";
	}

	private static bool IsValueObsolete(Type type, string value)
	{
		var fi = type.GetField(value);
		var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);
		return attributes.Length > 0;
	}

	private static string GetValueObsoleteMessage(Type type, string value)
	{
		var fi = type.GetField(value);
		var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);
		return attributes.FirstOrDefault()?.Message;
	}
}
