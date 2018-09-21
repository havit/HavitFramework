using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre nastavenie defaultov pre stringy (tak ako ich definuje BusinessLayerGenerator): ak stringová property ešte nemá nastavený default, tak jej nastaví ako default prázdny string.
	/// </summary>
	public class DefaultsForStringsConvention : IModelConvention
	{
		/// <inheritdoc />
		public void Apply(ModelBuilder modelBuilder)
		{
			var stringProperties = modelBuilder.Model
				.GetApplicationEntityTypes()
				.SelectMany(entityType => entityType.GetDeclaredProperties().Where(prop => prop.ClrType == typeof(string)));

			foreach (IMutableProperty property in stringProperties)
			{
				if ((property.Relational().DefaultValue == null) && string.IsNullOrEmpty(property.Relational().DefaultValueSql))
				{
					property.Relational().DefaultValue = "";
				}
			}
		}
	}
}