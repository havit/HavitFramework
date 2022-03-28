﻿using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model
{
	public class MetadataClassModelSource : IModelSource<MetadataClass>
	{
		private readonly DbContext dbContext;
		private readonly IProject metadataProject;
		private readonly IProject modelProject;
        private readonly CodeGeneratorConfiguration configuration;

        public MetadataClassModelSource(DbContext dbContext, IProject metadataProject, IProject modelProject, CodeGeneratorConfiguration configuration)
		{
			this.dbContext = dbContext;
			this.metadataProject = metadataProject;
			this.modelProject = modelProject;
            this.configuration = configuration;
        }

		public IEnumerable<MetadataClass> GetModels()
		{
			List<MetadataClass> result = (from registeredEntity in dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
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
			string metadataProjectNamespace = metadataProject.GetProjectRootNamespace();
			string modelProjectNamespace = modelProject.GetProjectRootNamespace();
			if (namespaceName.StartsWith(modelProjectNamespace))
			{
				return metadataProjectNamespace + "." + configuration.MetadataNamespace + namespaceName.Substring(modelProjectNamespace.Length);
			}
			else
			{
				return namespaceName + "." + configuration.MetadataNamespace;
			}
		}
	}
}
