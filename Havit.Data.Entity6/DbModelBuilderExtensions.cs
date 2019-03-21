using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity
{
	/// <summary>
	/// Extension metody ke třídě DbModelBuilder.
	/// </summary>
	public static class DbModelBuilderExtensions
	{
		/// <summary>
		/// Zaregistruje veřejné modelové třídy z předané assembly.
		/// Registrovány jsou veřejné třídy, avšak třídy s atributem NotMappedAttribute a ComplexTypeAttribute jsou ignorovány (nejsou registrovány).
		/// </summary>
		public static void RegisterModelFromAssembly(this DbModelBuilder modelBuilder, Assembly assembly)
		{
			Contract.Requires<ArgumentNullException>(modelBuilder != null, nameof(modelBuilder));
			Contract.Requires<ArgumentNullException>(assembly != null, nameof(modelBuilder));

			List<Type> assemblyTypes = assembly.GetTypes().Where(assemblyType => assemblyType.IsPublic && assemblyType.IsClass).ToList();
			foreach (Type assemblyType in assemblyTypes)
			{
				if (assemblyType.GetCustomAttributes<NotMappedAttribute>().Any())
				{
					// NOOP
				}
				else if (assemblyType.GetCustomAttributes<ComplexTypeAttribute>().Any())
				{
					// NOOP
				}
				else
				{
					modelBuilder.RegisterEntityType(assemblyType);
				}				
			}
		}
	}
}
