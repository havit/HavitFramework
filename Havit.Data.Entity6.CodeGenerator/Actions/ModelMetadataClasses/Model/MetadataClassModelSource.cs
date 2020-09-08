using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Mapping.Internal;

namespace Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model
{
	public class MetadataClassModelSource : IModelSource<MetadataClass>
	{
		private readonly DbContext dbContext;
		private readonly IProject project;

		public MetadataClassModelSource(DbContext dbContext, IProject project)
		{
			this.dbContext = dbContext;
			this.project = project;
		}

		public IEnumerable<MetadataClass> GetModels()
		{
			List<MetadataClass> result = (from registeredEntity in dbContext.GetRegisteredEntities()
				select new MetadataClass
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					ClassName = registeredEntity.ClassName + "Metadata",
					MaxLengthConstants = (from property in registeredEntity.Properties
						where property.Type == typeof(string)
						select new MetadataClass.MaxLengthConstant
						{
							Name = property.PropertyName + "MaxLength",
							Value = (property.MaxLength == -1)
								? Int32.MaxValue
								: property.MaxLength ?? 0
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
