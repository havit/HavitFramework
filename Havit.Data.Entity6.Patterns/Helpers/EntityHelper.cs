using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.Helpers
{
	internal static class EntityHelper
	{
		internal static int GetEntityId(object entity)
		{
			return ((dynamic)entity).Id;
		}

	}
}
