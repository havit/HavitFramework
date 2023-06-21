using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;

public class DataEntriesModelSource : IModelSource<DataEntriesModel>
{
	private readonly DbContext dbContext;
	private readonly IProject modelProject;
	private readonly IProject dataLayerProject;
	private readonly CammelCaseNamingStrategy cammelCaseNamingStrategy;

	public DataEntriesModelSource(DbContext dbContext, IProject modelProject, IProject dataLayerProject, CammelCaseNamingStrategy cammelCaseNamingStrategy)
	{
		this.dbContext = dbContext;
		this.modelProject = modelProject;
		this.dataLayerProject = dataLayerProject;
		this.cammelCaseNamingStrategy = cammelCaseNamingStrategy;
	}

	public IEnumerable<DataEntriesModel> GetModels()
	{
		return (from registeredEntity in dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
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
							FieldName = cammelCaseNamingStrategy.GetCammelCase(item),
							IsObsolete = IsValueObsolete(entriesEnumType, item)
						})
						.ToList()
				}).ToList();
	}

	private Type GetEntriesEnum(Type type)
	{
		Type entriesType = type.GetNestedType("Entry"); // TODO: Duplikovaný kód
		if ((entriesType != null) && (entriesType.IsEnum))
		{
			return entriesType;
		}
		return null;
	}

	private string GetNamespaceName(string namespaceName)
	{
		string modelProjectNamespace = modelProject.GetProjectRootNamespace();
		if (namespaceName.StartsWith(modelProjectNamespace))
		{
			return dataLayerProject.GetProjectRootNamespace() + ".DataEntries" + namespaceName.Substring(modelProjectNamespace.Length);
		}
		else
		{
			return namespaceName + ".DataSources";
		}
	}

	private string GetRepositoryDependencyFullName(Type entityType)
	{
		string entityNamespaceName = entityType.Namespace;
		string modelProjectNamespace = modelProject.GetProjectRootNamespace();

		string repositoryNamespace = entityNamespaceName.StartsWith(modelProjectNamespace)
			? dataLayerProject.GetProjectRootNamespace() + ".Repositories" + entityNamespaceName.Substring(modelProjectNamespace.Length)
			: entityNamespaceName + ".Repositories";

		return repositoryNamespace + ".I" + entityType.Name + "Repository";
	}

	private static bool IsValueObsolete(Type type, string value)
	{
		var fi = type.GetField(value);
		var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
		return (attributes != null) && (attributes.Length > 0);
	}
}
