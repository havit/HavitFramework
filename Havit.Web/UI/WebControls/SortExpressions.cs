using System;
using Havit.Collections;
using System.Web;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Třída SortExpressions zajišťuje práci s řazením položek.
	/// </summary>
	/// 
	[Serializable]
	public class SortExpressions
	{
		#region SortItems
		/// <summary>
		/// Položky řazení.
		/// </summary>
		public SortItemCollection SortItems
		{
			get
			{
				return sortItems;
			}
		}
		private readonly SortItemCollection sortItems = new SortItemCollection();
		#endregion

		#region ClearSelection
		/// <summary>
		/// Vyprázdní seznam položek řazení.
		/// </summary>
		public void ClearSelection()
		{
			SortItems.Clear();
		}
		#endregion

		#region IndexOf
		/// <summary>
		/// Vyhledá index položky v seznamu položek řazení. Bere se ohled jen na Expression.
		/// </summary>
		protected int IndexOf(string expression)
		{
			for (int i = 0; i < SortItems.Count; i++)
			{
				if (SortItems[i].Expression == expression)
				{
					return i;
				}
			}
			return -1;
		}
		#endregion

		#region RemoveExpression
		/// <summary>
		/// Odstraní položku řazení ze seznamu, pokud existuje.
		/// </summary>
		protected void RemoveExpression(string expression)
		{
			int i = IndexOf(expression);
			if (i >= 0)
			{
				SortItems.RemoveAt(i);
			}
		}
		#endregion

		#region AddSortExpression
		/// <summary>
		/// Rozebere sortExpression a přidá položky řazení na první místo.
		/// Pokud položky pořadí již jsou na začátku seznamu, je jim otočen směr řazení.
		/// </summary>
		/// <param name="sortExpression">
		/// Výraz pro řazení. Seznam oddělený čárkami. Sestupný směr se vyjádří doplněním mezery a DESC.<br/>
		/// Např. "Nazev", "Prijmeni, Jmeno", "Vek DESC", "Vek desc".
		/// </param>
		public void AddSortExpression(string sortExpression)
		{
			string[] expressions = sortExpression.Split(',');

			if (expressions.Length == 0)
			{
				// pokud nic nepřidáváme, končíme.
				return;
			}

			SortItemCollection newItems = new SortItemCollection();
			for (int i = 0; i < expressions.Length; i++)
			{
				string trimmedExpression = expressions[i].Trim();
				if (trimmedExpression.ToLower().EndsWith(" desc"))
				{
					// pokud máme ředit sestupně, vytvoříme položku pro sestupné řazení
					newItems.Add(new SortItem(trimmedExpression.Substring(0, trimmedExpression.Length - 5), SortDirection.Descending));
				}
				else
				{
					// jinak vytvoříme položku pro vzestupné řazení.
					newItems.Add(new SortItem(trimmedExpression, SortDirection.Ascending));
				}
			}

			if (StartsWith(newItems))
			{
				// pokud už položky v seznamu jsou a jsou na začátku, otočíme jim směr řazení.				
				for (int i = 0; i < newItems.Count; i++)
				{
					sortItems[i].Direction = (sortItems[i].Direction == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
				}
			}
			else
			{
				// jinak je přidáme na začátek seznamu (jsou-li v seznamu dále, vyhodíme je a dáme na začátek)
				for (int i = 0; i < newItems.Count; i++)
				{
					RemoveExpression(newItems[i].Expression);
					SortItems.Insert(i, newItems[i]);
				}
			}
		}
		#endregion

		#region StartsWith
		/// <summary>
		/// Vrací true, pokud kolekce začíná stejnými položkami, jako jsou zde uvedené.
		/// Na směr řazení se bere ohled v tom smyslu, že směr řazení všech stávající položek musí být stejný 
		/// jako směr řazení nových položek nebo musí být směr řazení na všech položkách opačný.
		/// </summary>
		protected bool StartsWith(SortItemCollection referencingItems)
		{
			if (referencingItems.Count == 0)
			{
				return true;
			}

			if (referencingItems.Count > sortItems.Count)
			{
				return false;
			}

			bool sameDirection = referencingItems[0].Direction == sortItems[0].Direction;

			for (int i = 0; i < referencingItems.Count; i++)
			{
				if ((referencingItems[i].Expression != sortItems[i].Expression) // nejsou stejné expression
				 || ((referencingItems[i].Direction == sortItems[i].Direction) ^ sameDirection)) // nebo není stejný směr řazení
				{
					return false;
				}
			}

			return true;
		}
		#endregion

		///// <summary>
		///// Přidá položky řazení do seznamu na první pozice (první položka kolekce bude na začátku).
		///// Pokud již položky v seznamu existují na prvních pozicích změní směr řazení.
		///// Pokud existuje na jiné pozici, odstraní položku ze seznamu
		///// a vloží ji na první pozici (tj. zvýší se "priorita" a řadí se vzestupně.
		///// </summary>
		//public void Add(string[] expression)
		//{
		//    // řadíme vzestupně
		//    SortDirection newDirection = SortDirection.Ascending;

		//    if (SortItems.Count >= expression.Length)
		//    {
		//        // řadíme sestupně, ale dále to většinou zpět změníme
		//        newDirection = SortDirection.Descending;

		//        // projdeme všechny položky
		//        for (int i = 0; i < expression.Length; i++)
		//        {
		//            // a testujeme, jestli jsou na začátku seznamu a řazené vzestupně
		//            // pokud ano, neděláme nic (zůstane sestupné řazení
		//            // pokud ne, změníme řazení a končíme test
		//            if (expression[i] != SortItems[i].Expression || SortItems[i].Direction != SortDirection.Ascending)
		//            {
		//                newDirection = SortDirection.Ascending;
		//                break;
		//            }
		//        }
		//    }

		//    // přidáme postupně položky řazení (odzadu, protože poslední přidaná bude mít nejvyšší prioritu)
		//    for (int i = expression.Length - 1; i >= 0; i--)
		//        Add(expression[i], newDirection);
		//}

		///// <summary>
		///// Přidá položku řazení do seznamu na první pozici. 
		///// Pokud již v seznamu existuje, je původní položka odstraněna.
		///// </summary>
		///// <param name="expression"></param>
		///// <param name="sortDirection"></param>
		//protected void Add(string expression, SortDirection sortDirection)
		//{
		//    RemoveExpression(expression);
		//    SortItems.Insert(0, new SortItem(expression, sortDirection));
		//}

		///// <summary>
		///// Prozkoumá expression od uživatele a rozebere ji na jednotlivé položky řazení.
		///// </summary>
		//public static string[] ParseSortExpression(string expression)
		//{
		//    string[] sortExpressionItems = expression.Split(',');

		//    // odstranime pripadne mezery
		//    for (int i = 0; i < sortExpressionItems.Length; i++)
		//        sortExpressionItems[i] = sortExpressionItems[i].Trim();

		//    return sortExpressionItems;
		//}
	}
}