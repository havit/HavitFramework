using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Informace o vlastnosti, která má byt DataLoaderem načtena.
	/// </summary>
	public class PropertyToLoad
	{
		/// <summary>
		/// Název načítané vlasntosti.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Typ, jehož vlastnost je načítána.
		/// </summary>
		public Type SourceType { get; set; }

		/// <summary>
		/// Typ načítané vlasnosti.
		/// V případě kolekcí jde o kolekci prvků (např. pro LoginAccount.Roles bude obsahovat List&lt;Role&gt;.
		/// </summary>
		public Type TargetType { get; set; }

		/// <summary>
		/// Typ prvku načítané kolekce.
		/// </summary>
		public Type CollectionItemType { get; set; }

		/// <summary>
		/// Indikuje, zda je načítána kolekce. V opačném případě je načítána reference.
		/// </summary>
		public bool IsCollection => CollectionItemType != null;
	}
}