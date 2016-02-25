using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model
{
	public class MetadataClassModelSource : IModelSource<MetadataClass>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project project;

		public MetadataClassModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project project)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.project = project;
		}

		public IEnumerable<MetadataClass> GetModels()
		{
			List<MetadataClass> result = (from registeredEntity in registeredEntityEnumerator.GetRegisteredEntities()
				select new MetadataClass
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					ClassName = registeredEntity.ClassName + "Metadata",
					MaxLengthConstants = (from property in registeredEntity.Properties
						where property.Type == typeof(string)
						select new MetadataClass.MaxLengthConstant
						{
							Name = property.PropertyName + "MaxLength",
							Value = property.MaxLength ?? 0
						}).ToList()
				}).ToList();
			return result;
		}

		private string GetNamespaceName(string namespaceName)
		{
			string projectNamespace = project.GetProjectRootNamespace();
			if (namespaceName.StartsWith(projectNamespace))
			{
				return projectNamespace + ".Metadata" + namespaceName.Substring(projectNamespace.Length);
			}
			else
			{
				return namespaceName + ".Metadata";
			}
		}
	}
}
