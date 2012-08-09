using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínky, které nic netestují.
	/// Nyní je taková podmínka reprezentována hodnotou null.
	/// </summary>
	public static class EmptyCondition
	{
		#region Create
		/// <summary>
		/// Vytvoří podmínku reprezentující prázdnou podmínku (nic není testováno). Nyní vrací null.
		/// </summary>
		/// <returns></returns>
		public static Condition Create()
		{
			return null;
		} 
		#endregion
	}
}
