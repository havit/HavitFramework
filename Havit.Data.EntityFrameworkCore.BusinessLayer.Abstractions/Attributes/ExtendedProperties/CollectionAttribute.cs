using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
    /// <summary>
    /// Extended property pro vygenerování kolekce.
    /// </summary>
    /// <remarks>
    /// Collection_Xyz_IncludeDeleted<br/>
    /// Collection_Xyz_Sorting<br/>
    /// Collection_Xyz_CloneMode<br/>
    /// Collection_Xyz_PropertyAccessModifier<br/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
	public class CollectionAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Je-li nastaveno na true, načítají se do kolekce i (příznakem) smazané záznamy.
		/// </summary>
		public bool IncludeDeleted { get; set; } = false;

		///// <summary>
		///// Vlastnost říká, že se automaticky načtou všechny prvky kolekce v okamžiku prvního přístupu ke kolekci (zavolá se na kolekci metoda LoadAll).
		///// </summary>
		//public bool? LoadAll { get; set; }

		/// <summary>
		/// Pořadí, v jakém jsou položky kolekce načteny z databáze do paměti. Obsahuje SQL výraz, který se vloží do části ORDER BY. Názvy sloupců musí být vloženy do složených závorek! Příklady: "{Order}", "COALESCE({Order}, {TabulkaID})
		/// </summary>
		public string Sorting { get; set; } = null;

        /// <summary>
        /// Výchozí viditelnost pro property kolekce. Nesmí být private, pokud jiné třídy tuto vlastnost používají (např. CreateObject(owner)).
        /// </summary>
        public AccessModifier PropertyAccessModifier { get; set; } = AccessModifier.Public;

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>()
			.AddIfNotDefault($"Collection_{memberInfo.Name}_IncludeDeleted", IncludeDeleted, false)
			//.AddIfNotDefault($"Collection_{memberInfo.Name}_LoadAll", LoadAll)
            .AddIfNotDefault($"Collection_{memberInfo.Name}_PropertyAccessModifier", PropertyAccessModifier, AccessModifier.Public)
			.AddIfNotDefault($"Collection_{memberInfo.Name}_Sorting", Sorting, null);
	}
}