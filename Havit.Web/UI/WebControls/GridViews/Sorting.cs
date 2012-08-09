using System;
using Havit.Collections;
using Havit.Business.Query;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Tøída Sorting zajišuje práci s øazením poloek.
	/// </summary>
	/// 
	[Serializable]
	public class Sorting
	#warning Neumí rozumnì sestupné øazení
	{
		private SortItemCollection sortItems = new SortItemCollection();
		internal SortItemCollection SortItems
		{
			get
			{
				return sortItems;
			}
		}

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
				if (SortItems[i].Expression == expression)
					return i;
			return -1;
		}

		/// <summary>
		/// Odstraní poloku øazení ze seznamu, pokud existuje.
		/// </summary>
		/// <param name="expression"></param>
		protected void RemoveField(string expression)
		{
			int i = IndexOf(expression);
			if (i >= 0)
				SortItems.RemoveAt(i);
		}

		/// <summary>
		/// Pøidá poloku øazení do seznamu na první pozici.
		/// Pokud ji v seznamu existuje, tak pokud existuje na první pozici (tj. na pozici 0),
		/// zmìní smìr øazení. Pokud existuje na jiné pozici, odstraní poloku ze seznamu
		/// a vloí ji na první pozici (tj. zvıší se "priorita" a øadí se vzestupnì.
		/// </summary>
		/// <param name="expression"></param>
		public void Add(string expression)
		{
			int i = IndexOf(expression);
			if (i == 0)
				Add(expression, (SortItems[0].Direction == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending);
			else
				Add(expression, SortDirection.Ascending);
		}

        /// <summary>
        /// Pøidá poloky øazení do seznamu na první pozice (první poloka kolekce bude na zaèátku).
        /// Pokud ji poloky v seznamu existují na prvních pozicích zmìní smìr øazení.
        /// Pokud existuje na jiné pozici, odstraní poloku ze seznamu
        /// a vloí ji na první pozici (tj. zvıší se "priorita" a øadí se vzestupnì.
        /// </summary>
		public void Add(string[] expression)
        {
            // øadíme vzestupnì
			SortDirection newDirection = SortDirection.Ascending;

			if (SortItems.Count >= expression.Length)
            {
                // øadíme sestupnì, ale dále to vìtšinou zpìt zmìníme
				newDirection = SortDirection.Descending;

                // projdeme všechny poloky
				for (int i = 0; i < expression.Length; i++)
                {
                    // a testujeme, jestli jsou na zaèátku seznamu a øazené vzestupnì
                    // pokud ano, nedìláme nic (zùstane sestupné øazení
                    // pokud ne, zmìníme øazení a konèíme test
					if (expression[i] != SortItems[i].Expression || SortItems[i].Direction != SortDirection.Ascending)
                    {
						newDirection = SortDirection.Ascending;
                        break;
                    }
                }
            }

            // pøidáme postupnì poloky øazení (odzadu, protoe poslední pøidaná bude mít nejvyšší prioritu)
			for (int i = expression.Length - 1; i >= 0; i--)
				Add(expression[i], newDirection);
        }

        /// <summary>
        /// Pøidá poloku øazení do seznamu na první pozici. 
        /// Pokud ji v seznamu existuje, je pùvodní poloka odstranìna.
        /// </summary>
		/// <param name="expression"></param>
        /// <param name="sortDirection"></param>
		protected void Add(string expression, SortDirection sortDirection)
		{
			RemoveField(expression);
			SortItems.Insert(0, new SortItem(expression, sortDirection));
		}

		/// <summary>
		/// Prozkoumá expression od uivatele a rozebere ji na jednotlivé poloky øazení.
		/// </summary>
		public static string[] ParseSortExpression(string expression)
		{
			string[] sortExpressionItems = expression.Split(',');

			// odstranime pripadne mezery
			for (int i = 0; i < sortExpressionItems.Length; i++)
				sortExpressionItems[i] = sortExpressionItems[i].Trim();

			return sortExpressionItems;
		}
	}
}