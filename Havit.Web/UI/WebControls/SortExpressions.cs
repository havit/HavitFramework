using System;
using Havit.Collections;
using System.Web;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Tøída SortExpressions zajišuje práci s øazením poloek.
	/// </summary>
	/// 
	[Serializable]
	public class SortExpressions
	{
		/// <summary>
		/// Poloky øazení.
		/// </summary>
		public SortItemCollection SortItems
		{
			get
			{
				return sortItems;
			}
		}
		private SortItemCollection sortItems = new SortItemCollection();

		/// <summary>
		/// Vyprázdní seznam poloek øazení.
		/// </summary>
		public void ClearSelection()
		{
			SortItems.Clear();
		}

		/// <summary>
		/// Vyhledá index poloky v seznamu poloek øazení. Bere se ohled jen na Expression.
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

		/// <summary>
		/// Odstraní poloku øazení ze seznamu, pokud existuje.
		/// </summary>
		/// <param name="expression"></param>
		protected void RemoveExpression(string expression)
		{
			int i = IndexOf(expression);
			if (i >= 0)
			{
				SortItems.RemoveAt(i);
			}
		}

		/// <summary>
		/// Rozebere sortExpression a pøidá poloky øazení na první místo.
		/// Pokud poloky poøadí ji jsou na zaèátku seznamu, je jim otoèen smìr øazení.
		/// </summary>
		/// <param name="sortExpression">
		/// Vıraz pro øazení. Seznam oddìlenı èárkami. Sestupnı smìr se vyjádøí doplnìním mezery a DESC.<br/>
		/// Napø. "Nazev", "Prijmeni, Jmeno", "Vek DESC", "Vek desc".
		/// </param>
		public void AddSortExpression(string sortExpression)
		{
			string[] expressions = sortExpression.Split(',');

			if (expressions.Length == 0)
			{
				// pokud nic nepøidáváme, konèíme.
				return;
			}

			SortItemCollection newItems = new SortItemCollection();
			for (int i = 0; i < expressions.Length; i++)
			{
				string trimmedExpression = expressions[i].Trim();
				if (trimmedExpression.ToLower().EndsWith(" desc"))
				{
					// pokud máme øedit sestupnì, vytvoøíme poloku pro sestupné øazení
					newItems.Add(new SortItem(trimmedExpression.Substring(0, trimmedExpression.Length - 5), SortDirection.Descending));
				}
				else
				{
					// jinak vytvoøíme poloku pro vzestupné øazení.
					newItems.Add(new SortItem(trimmedExpression, SortDirection.Ascending));
				}
			}

			if (StartsWith(newItems))
			{
				// pokud u poloky v seznamu jsou a jsou na zaèátku, otoèíme jim smìr øazení.				
				for (int i = 0; i < newItems.Count; i++)
				{
					sortItems[i].Direction = (sortItems[i].Direction == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
				}
			}
			else
			{
				// jinak je pøidáme na zaèátek seznamu (jsou-li v seznamu dále, vyhodíme je a dáme na zaèátek)
				for (int i = 0; i < newItems.Count; i++)
				{
					RemoveExpression(newItems[i].Expression);
					SortItems.Insert(i, newItems[i]);
				}
			}
		}

		/// <summary>
		/// Vrací true, pokud kolekce zaèíná stejnımi polokami, jako jsou zde uvedené.
		/// Na smìr øazení se bere ohled v tom smyslu, e smìr øazení všech stávající poloek musí bıt stejnı 
		/// jako smìr øazení novıch poloek nebo musí bıt smìr øazení na všech polokách opaènı.
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
				 || ((referencingItems[i].Direction == sortItems[i].Direction) ^ sameDirection)) // nebo není stejnı smìr øazení
				{
					return false;
				}
			}

			return true;
		}


		///// <summary>
		///// Pøidá poloky øazení do seznamu na první pozice (první poloka kolekce bude na zaèátku).
		///// Pokud ji poloky v seznamu existují na prvních pozicích zmìní smìr øazení.
		///// Pokud existuje na jiné pozici, odstraní poloku ze seznamu
		///// a vloí ji na první pozici (tj. zvıší se "priorita" a øadí se vzestupnì.
		///// </summary>
		//public void Add(string[] expression)
		//{
		//    // øadíme vzestupnì
		//    SortDirection newDirection = SortDirection.Ascending;

		//    if (SortItems.Count >= expression.Length)
		//    {
		//        // øadíme sestupnì, ale dále to vìtšinou zpìt zmìníme
		//        newDirection = SortDirection.Descending;

		//        // projdeme všechny poloky
		//        for (int i = 0; i < expression.Length; i++)
		//        {
		//            // a testujeme, jestli jsou na zaèátku seznamu a øazené vzestupnì
		//            // pokud ano, nedìláme nic (zùstane sestupné øazení
		//            // pokud ne, zmìníme øazení a konèíme test
		//            if (expression[i] != SortItems[i].Expression || SortItems[i].Direction != SortDirection.Ascending)
		//            {
		//                newDirection = SortDirection.Ascending;
		//                break;
		//            }
		//        }
		//    }

		//    // pøidáme postupnì poloky øazení (odzadu, protoe poslední pøidaná bude mít nejvyšší prioritu)
		//    for (int i = expression.Length - 1; i >= 0; i--)
		//        Add(expression[i], newDirection);
		//}

		///// <summary>
		///// Pøidá poloku øazení do seznamu na první pozici. 
		///// Pokud ji v seznamu existuje, je pùvodní poloka odstranìna.
		///// </summary>
		///// <param name="expression"></param>
		///// <param name="sortDirection"></param>
		//protected void Add(string expression, SortDirection sortDirection)
		//{
		//    RemoveExpression(expression);
		//    SortItems.Insert(0, new SortItem(expression, sortDirection));
		//}

		///// <summary>
		///// Prozkoumá expression od uivatele a rozebere ji na jednotlivé poloky øazení.
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