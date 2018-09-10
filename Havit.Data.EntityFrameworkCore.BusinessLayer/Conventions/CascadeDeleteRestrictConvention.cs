using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public class CascadeDeleteRestrictConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableForeignKey foreignKey in modelBuilder.Model.GetEntityTypes()
				.SelectMany(t => t.GetForeignKeys())
				.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade))
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}
	}
}