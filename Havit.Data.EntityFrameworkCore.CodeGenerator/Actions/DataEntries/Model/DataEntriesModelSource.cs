using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model
{
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
			return (from registeredEntity in dbContext.Model.GetEntityTypes()
				let entriesEnumType = GetEntriesEnum(registeredEntity.ClrType)
				where (entriesEnumType != null)
				select new DataEntriesModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					InterfaceName = "I" + registeredEntity.ClrType.Name + "Entries",
					DbClassName = registeredEntity.ClrType.Name + "Entries",
					ModelClassFullName = registeredEntity.ClrType.FullName,
					ModelEntriesEnumerationFullName = registeredEntity.ClrType.FullName + ".Entry",
					Entries = System.Enum.GetNames(entriesEnumType).OrderBy(item => item).Select(item => new DataEntriesModel.Entry
					{
						PropertyName = item,
						FieldName = cammelCaseNamingStrategy.GetCammelCase(item)
					}).ToList()
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
	}
}
