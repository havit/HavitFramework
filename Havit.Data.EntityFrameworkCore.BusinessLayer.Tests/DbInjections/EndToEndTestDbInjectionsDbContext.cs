using System;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.DbInjections
{
    internal class EndToEndTestDbInjectionsDbContext<TEntity> : EndToEndTestDbContext<TEntity>
		where TEntity : class
	{
		private readonly Type[] dbInjectorTypes;

		public EndToEndTestDbInjectionsDbContext(params Type[] dbInjectorTypes)
		{
			this.dbInjectorTypes = dbInjectorTypes;
		}

		protected override void ModelCreatingCompleting(ModelBuilder modelBuilder)
		{
			modelBuilder.ForDbInjections(this.GetService<IDbInjectionAnnotationProvider>(), dbInjectorTypes);
		}
	}
}