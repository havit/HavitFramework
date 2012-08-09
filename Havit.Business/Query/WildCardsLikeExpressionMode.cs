using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Režim WildCardsLikeExpression.
	/// </summary>
	public enum WildCardsLikeExpressionMode
	{
		/// <summary>
		/// Vyhledává text, který začíná zadaným výrazem. Toto je výchozí chování.
		/// </summary>
		StartsWith,

		/// <summary>
		/// Vyhledává text, který obsahuje daný text.
		/// </summary>
		Contains
	}
}
