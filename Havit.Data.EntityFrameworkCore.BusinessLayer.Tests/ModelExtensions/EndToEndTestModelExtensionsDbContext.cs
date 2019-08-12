using System;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

		protected override void ModelCreatingCompleting(ModelBuilder modelBuilder)
		{
			modelBuilder.ForModelExtensions(this.GetService<IModelExtensionAnnotationProvider>(), modelExtenderTypes);
		}
	}
}