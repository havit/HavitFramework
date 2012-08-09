using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínku testující referenèní hodnotu (cizí klíè).
	/// </summary>
	public static class ReferenceCondition
	{
		/// <summary>
		/// Vytvoøí podmínku na rovnost.
		/// </summary>
		public static ICondition CreateEquals(Property property, int? ID)
		{
			if (ID == null || ID < 0)
				return NullCondition.CreateIsNull(property);
			else
				return NumberCondition.CreateEquals(property, ID.Value);
		}

		/// <summary>
		/// Vytvoøí podmínku na rovnost.
		/// </summary>
		public static ICondition CreateEquals(Property property, BusinessObjectBase businessObject)
		{
			if (businessObject.IsNew)
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");

			return CreateEquals(property, businessObject.ID);
		}
		
	}
}
