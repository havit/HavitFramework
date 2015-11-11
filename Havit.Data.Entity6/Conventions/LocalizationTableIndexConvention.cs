using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Havit.Data.Entity.Internal;
using Havit.Data.Entity.ModelConfiguration.Edm;
using Havit.Model.Localizations;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Zajišťuje tvorbu unikátního indexu na LanguageId a ParentId u tabulek, které jsou lokalizacemi.
	/// </summary>
	public class LocalizationTableIndexConvention : IStoreModelConvention<EntitySet>
	{
		/// <summary>
		/// Aplikuje konvenci na model.
		/// </summary>
		public void Apply(EntitySet entitySet, DbModel model)
		{
			if (!entitySet.ElementType.IsConventionSuppressed(typeof(LocalizationTableIndexConvention)))
			{
				EntityType entityTypeCSpace = model.ConceptualModel.EntityTypes.FirstOrDefault(item => item.Name == entitySet.Name);
				if (entityTypeCSpace != null)
				{
					Type entityClrType = entityTypeCSpace.GetClrType(); // získáme CLR typ reprezentovaný tímto entitySetem

					bool isLocalizationType = entityClrType.GetInterfaces().Any(item => item.IsGenericType && (item.GetGenericTypeDefinition() == typeof(ILocalization<,>))); // zjistíme, zda entityTypeCSpace implementuje typeof(ILocalization<,>
					if (isLocalizationType)
					{
						EdmProperty languageIdProperty = entitySet.ElementType.DeclaredProperties.Where(item => item.Name == "LanguageId").FirstOrDefault();
						EdmProperty parentIdProperty = entitySet.ElementType.DeclaredProperties.Where(item => item.Name == "ParentId").FirstOrDefault();

						// pokud máme k dispozici vlastnosti (sloupce) LanguageId a ParentId (teoreticky mohou být v předkovi nebo nemusí vůbec existovat, protože interface ILocalization<,> je nepředepisuje, apod.)
						if ((languageIdProperty != null) && (parentIdProperty != null))
						{
							IndexHelper.AddIndex(new EdmProperty[] { parentIdProperty, languageIdProperty }, true);
						}
					}
				}
			}
		}
	}
}