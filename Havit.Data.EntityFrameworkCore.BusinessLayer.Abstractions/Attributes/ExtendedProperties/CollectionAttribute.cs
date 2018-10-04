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
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public class CollectionAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Je-li nastaveno na true, načítají se do kolekce i (příznakem) smazané záznamy.
		/// </summary>
		public bool? IncludeDeleted { get; set; }

		///// <summary>
		///// Vlastnost říká, že se automaticky načtou všechny prvky kolekce v okamžiku prvního přístupu ke kolekci (zavolá se na kolekci metoda LoadAll).
		///// </summary>
		//public bool? LoadAll { get; set; }

		/// <summary>
		/// Pořadí, v jakém jsou položky kolekce načteny z databáze do paměti. Obsahuje SQL výraz, který se vloží do části ORDER BY. Názvy sloupců musí být vloženy do složených závorek! Příklady: "{Order}", "COALESCE({Order}, {TabulkaID})
		/// </summary>
		public string Sorting { get; set; }

		/// <summary>
		/// Určuje režim klonování prvků kolekce při klonování objektu. Kolekce typu 1:N nepodporují klonování typu Shallow.
		/// - No - prvky se neklonují
		/// - Shallow - připojí se prvky originálu(budou tak sdílené, vhodné jen pro M:N kolekce)
		/// - Deep - připojí se klony prvků originálu.
		/// </summary>
		public string CloneMode { get; set; }
		
		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>()
			.AddIfNotDefault($"Collection_{memberInfo.Name}_IncludeDeleted", IncludeDeleted)
			//.AddIfNotDefault($"Collection_{memberInfo.Name}_LoadAll", LoadAll)
			.AddIfNotDefault($"Collection_{memberInfo.Name}_Sorting", Sorting)
			.AddIfNotDefault($"Collection_{memberInfo.Name}_CloneMode", CloneMode);
	}
}