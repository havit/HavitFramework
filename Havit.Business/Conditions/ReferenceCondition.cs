using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	public static class ReferenceCondition
	{
		public static ICondition CreateEquals(Property property, int? ID)
		{
			if (ID == null || ID < 0)
				return NullCondition.CreateIsNull(property);
			else
				return NumberCondition.CreateEquals(property, ID.Value);
		}

		public static ICondition CreateEquals(Property property, BusinessObjectBase businessObject)
		{
			if (businessObject.IsNew)
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");

			return CreateEquals(property, businessObject.ID);
		}
		
	}
}
