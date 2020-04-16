using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Pomocné třídy pro registraci služeb do DI kontejneru.
	/// </summary>
	internal static class EntityPatternsInstallerBaseHelpers
	{
		/// <summary>
		/// Vrací true, pokud daná třída má jako předka (předka předka, předka předka předka) daný typ.
		/// </summary>
		internal static bool HasAncestorOfType(this Type type, Type ancesorType)
		{
			if ((type == ancesorType) || (ancesorType.IsGenericType && type.IsGenericType && type.GetGenericTypeDefinition() == ancesorType))
			{
				return true;
			}

			if (type.BaseType != null)
			{
				return HasAncestorOfType(type.BaseType, ancesorType);
			}

			return false;
		}

		/// <summary>
		/// Pro daný typ a daný open-type interface vrátí typem implementovaný interface closed-type.
		/// K pochopení na příkladu:
		/// Pro daný typ LanguageDataSource a open-type interface IDataSource&lt;&gt; vrátí IDataSource&lt;Language&gt;.
		/// Předpokládá se, že by daný typ implementuje právě jednu variantu close-type daného interface, jinak vyhazuje výjimku.
		/// </summary>
		internal static Type GetSingleConstructedType(this Type type, Type genericType)
		{
			return type.GetInterfaces().Where(typeInterfaceType => typeInterfaceType.IsGenericType && typeInterfaceType.GetGenericTypeDefinition() == genericType).Single();			
		}
	}
}
