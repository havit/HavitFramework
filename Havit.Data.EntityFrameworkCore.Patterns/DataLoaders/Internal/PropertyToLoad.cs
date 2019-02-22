using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Informace o vlastnosti, která má byt DataLoaderem naètena.
	/// </summary>
	public class PropertyToLoad
	{
		/// <summary>
		/// Název naèítané vlastnosti (po pøípadné substituci).
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Název naèítané vlastnosti (pøed pøípadnou substitucí).
		/// </summary>
		public string OriginalPropertyName { get; set; }

		/// <summary>
		/// Typ, jeho vlastnost je naèítána.
		/// </summary>
		public Type SourceType { get; set; }

		/// <summary>
		/// Typ naèítané vlastnosti  (po pøípadné substituci).
		/// V pøípadì kolekcí jde o kolekci prvkù (napø. pro LoginAccount.Roles bude obsahovat List&lt;Role&gt;.
		/// </summary>
		public Type TargetType { get; set; }

		/// <summary>
		/// Typ prvku naèítané kolekce.
		/// </summary>
		public Type CollectionItemType { get; set; }

		/// <summary>
		/// Indikuje, zda je naèítána kolekce. V opaèném pøípadì je naèítána reference.
		/// </summary>
		public bool IsCollection => CollectionItemType != null;
	}
}