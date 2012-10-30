using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Operátor pro porovnání hodnot.
	/// </summary>
	public enum ComparisonOperator
	{
		/// <summary>
		/// Rovnost.
		/// </summary>
		Equals, 
		
		/// <summary>
		/// Nerovnost.
		/// </summary>
		NotEquals,
		
		/// <summary>
		/// Menší.
		/// </summary>
		Lower, 

		/// <summary>
		/// Menší nebo rovno.
		/// </summary>
		LowerOrEquals,
		
		/// <summary>
		/// Větší.
		/// </summary>
		Greater,
		
		/// <summary>
		/// Větší nebo rovno.
		/// </summary>
		GreaterOrEquals
	}
}
