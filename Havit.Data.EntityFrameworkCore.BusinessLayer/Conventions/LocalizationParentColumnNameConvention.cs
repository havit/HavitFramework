using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Lokalizačním tabulkám nastaví jméno sloupce pro vlastnost Parent. Nastavuje název cílové tabulky + "ID".
	/// </summary>
	public class LocalizationParentColumnNameConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetApplicationEntityTypes())
			{
				Type interfaceType = entityType.ClrType.GetInterfaces().FirstOrDefault(itype => itype.IsGenericType && itype.GetGenericTypeDefinition() == typeof(Havit.Model.Localizations.ILocalization<,>));
				if (interfaceType != null)
				{
					var parentProperty = entityType.FindProperty("Parent");
					if (parentProperty != null)
					{
						parentProperty.Relational().ColumnName =
							modelBuilder.Model.FindEntityType(interfaceType.GetGenericArguments().First()).Relational().TableName + "ID";
					}
				}
			}
		}
	}
}

