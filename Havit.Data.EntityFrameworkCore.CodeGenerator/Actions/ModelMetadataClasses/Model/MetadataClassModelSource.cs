using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Entity;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model
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
			List<MetadataClass> result = (from registeredEntity in dbContext.Model.GetApplicationEntityTypes()
				select new MetadataClass
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					ClassName = registeredEntity.ClrType.Name + "Metadata",
					MaxLengthConstants = (from property in registeredEntity.GetProperties()
						where property.ClrType == typeof(string)
						select new MetadataClass.MaxLengthConstant
						{
							Name = property.Name + "MaxLength",
							Value = (property.GetMaxLength() == -1)
								? Int32.MaxValue
								: property.GetMaxLength() ?? 0
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
