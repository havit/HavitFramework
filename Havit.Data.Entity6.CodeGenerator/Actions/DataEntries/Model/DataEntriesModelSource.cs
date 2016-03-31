using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model
{
	public class DataEntriesModelSource : IModelSource<DataEntriesModel>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project modelProject;
		private readonly Project dataLayerProject;

		public DataEntriesModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project modelProject, Project dataLayerProject)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
		}

		public IEnumerable<DataEntriesModel> GetModels()
		{
			// TODO: Lock + reuse
			return (from registeredEntity in registeredEntityEnumerator.GetRegisteredEntities()
				let entriesEnumType = GetEntriesEnum(registeredEntity.Type)
				where (entriesEnumType != null)
				select new DataEntriesModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					InterfaceName = "I" + registeredEntity.ClassName + "Entries",
					DbClassName = registeredEntity.ClassName + "Entries",
					ModelClassFullName = registeredEntity.FullName,
					ModelEntriesEnumerationFullName = registeredEntity.FullName + ".Entry",
					Entries = System.Enum.GetNames(entriesEnumType).OrderBy(item => item).ToList()
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
