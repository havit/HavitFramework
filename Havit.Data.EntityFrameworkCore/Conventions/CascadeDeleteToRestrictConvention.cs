using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Všem cizím klíčům s nastaví DeleteBehavior na Restrict, čímž zamezí kaskádnímu delete.
	/// </summary>
	public class CascadeDeleteToRestrictConvention : IModelConvention
	{
		/// <summary>
		/// Aplikuje konvenci.
		/// </summary>
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableForeignKey foreignKey in modelBuilder.Model
				.GetApplicationEntityTypes()
				.WhereNotConventionSuppressed(typeof(CascadeDeleteToRestrictConvention)) // testujeme entity types
				.SelectMany(entityType => entityType.GetForeignKeys())
				.WhereNotConventionSuppressed(typeof(CascadeDeleteToRestrictConvention)) // testujeme foreign keys (byť zatím není jak nastavit)
				.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)) // díky použití GetApplicationEntityTypes by test na !IsOwnership měl být zbytečný
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}
	}
}