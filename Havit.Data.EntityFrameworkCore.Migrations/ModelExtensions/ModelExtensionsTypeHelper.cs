using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	internal static class ModelExtensionsTypeHelper
	{
		public static IEnumerable<Type> GetModelExtenders(Assembly assembly) => assembly.GetTypes().Where(t => t.GetInterface(nameof(IModelExtender)) != null);
	}
}