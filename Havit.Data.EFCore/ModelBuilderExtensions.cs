using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Havit.Data.EFCore
{
    public static class ModelBuilderExtensions
    {
	    /// <summary>
	    /// Zaregistruje veřejné modelové třídy z předané assembly.
	    /// Registrovány jsou veřejné třídy, avšak třídy s atributem NotMappedAttribute a ComplexTypeAttribute jsou ignorovány (nejsou registrovány).
	    /// </summary>
	    public static void RegisterModelFromAssembly(this ModelBuilder modelBuilder, Assembly assembly, string namespaceName = null)
	    {
		    // TODO JK: Unit test

//			TODO JK: Havit -> .NET Standard
//		    Contract.Requires(modelBuilder != null);
//			Contract.Requires(assembly != null);

		    List<Type> assemblyTypes = assembly.GetTypes()
			    .Where(type => type.IsPublic && type.IsClass)
			    .Where(type => String.IsNullOrEmpty(namespaceName) || type.Namespace.StartsWith(namespaceName))
			    .ToList();

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
// TODO JK: OwnedAttribute (a další)? + dokumentace
			    else
			    {
				    modelBuilder.Entity(assemblyType);
			    }				
		    }			
	    }

		// TODO JK: Komentář
	    public static void ApplyConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly assembly, string namespaceName = null)
	    {
		    // TODO JK: Unit test

		    MethodInfo applyConfigurationGenericMethod = typeof(ModelBuilder).GetMethod(nameof(ModelBuilder.ApplyConfiguration), BindingFlags.Instance | BindingFlags.Public);

		    var applicableTypesWithConfigurations = assembly
			    .GetTypes()
			    .Where(type => String.IsNullOrEmpty(namespaceName) || (type.Namespace?.StartsWith(namespaceName) ?? false)) // anonymní typy v unit testech mají namespace nullový
			    // TODO JK: type.IsPublic ?
			    .Where(type => type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters)
			    // ke každému typu přidáme všechny interfaces, pokud interfaces nemá, nebude ve výstupu
			    .SelectMany(type => type.GetInterfaces(), (type, iface) => new { Type = type, Interface = iface })
			    // if type implements interface IEntityTypeConfiguration<SomeEntity>
			    .Where(item => item.Interface.IsConstructedGenericType && (item.Interface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
			    // přejmenování Interface -> EntityTypeConfigurationInterface
			    .Select(item => new { Type = item.Type, EntityTypeConfigurationInterface = item.Interface })
			    .ToList();

		    foreach (var typeWithInterface in applicableTypesWithConfigurations)
		    {
			    // z generické ApplyConfiguration<> methody vyrobíme konkrétní ApplyConfiguration<SomeEntity>
			    MethodInfo applyConfigurationMethod = applyConfigurationGenericMethod.MakeGenericMethod(typeWithInterface.EntityTypeConfigurationInterface.GenericTypeArguments[0]);
			    applyConfigurationMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(typeWithInterface.Type) });
		    }
	    }
    }
}
