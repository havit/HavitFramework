using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// Určuje režim klonování prvků kolekce při klonování objektu. Kolekce typu 1:N nepodporují klonování typu Shallow.
	/// </summary>
	public enum CloneMode
	{
		/// <summary>
		/// Prvky se neklonují.
		/// </summary>
		No,

		/// <summary>
		/// Připojí se prvky originálu (budou tak sdílené, vhodné jen pro M:N kolekce).
		/// </summary>
		Shallow,

		/// <summary>
		/// Připojí se klony prvků originálu.
		/// </summary>
		Deep
	}
}
