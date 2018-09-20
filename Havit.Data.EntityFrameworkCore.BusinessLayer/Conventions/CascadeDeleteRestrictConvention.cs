using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public class CascadeDeleteRestrictConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableForeignKey foreignKey in modelBuilder.Model.GetEntityTypesExcludingSystemTypes()
				.SelectMany(entityType => entityType.GetForeignKeys())
				.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade))
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}
	}
}