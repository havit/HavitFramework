using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ModelExtensions
{
	internal class EndToEndTestModelExtensionsDbContext<TEntity> : EndToEndTestDbContext<TEntity>
		where TEntity : class
	{
		private readonly Type[] modelExtenderTypes;

		public EndToEndTestModelExtensionsDbContext(params Type[] modelExtenderTypes)
		{
			this.modelExtenderTypes = modelExtenderTypes;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			IEnumerable<TypeInfo> typeInfos = modelExtenderTypes.Select(t => t.GetTypeInfo());

			// stub out Model Extender types in IModelExtensionsAssembly (used by Model Extensions infrastructure)
			optionsBuilder.SetModelExtenderTypes(typeInfos);
		}
	}
}