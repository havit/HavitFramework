using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Extension metody k <see cref="ModelBuilder" />.
	/// </summary>
    public static class ModelBuilderExtensions
    {
	    /// <summary>
	    /// Zaregistruje veřejné modelové třídy z předané assembly.
	    /// Registrovány jsou veřejné třídy, avšak třídy s atributem <see cref="NotMappedAttribute" />, <see cref="ComplexTypeAttribute" />, <see cref="OwnedAttribute" /> jsou přeskočeny (nejsou nijak registrovány, ani nastaveny jako ignorované).
	    /// </summary>
	    public static void RegisterModelFromAssembly(this ModelBuilder modelBuilder, Assembly assembly, string namespaceName = null)
	    {
		    Contract.Requires(modelBuilder != null);
			Contract.Requires(assembly != null);

		    List<Type> assemblyTypes = assembly.GetTypes()
			    .Where(type => type.IsPublic && type.IsClass && !(type.IsAbstract && type.IsSealed) /* pokrývá statické třídy, viz např. https://stackoverflow.com/questions/4145072/how-to-tell-if-a-type-is-a-static-class */)
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
				else if (assemblyType.GetCustomAttributes<OwnedAttribute>().Any())
				{
					// NOOP
				}
				else
			    {
				    modelBuilder.Entity(assemblyType);
			    }				
		    }			
	    }

		/// <summary>
		/// Z dané assembly (a daného namespace) zaregistruje všechny konfigurace entit (třídy implementující <see cref="IEntityTypeConfiguration{T}" />).
		/// </summary>
	    public static void ApplyConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly assembly, string namespaceName = null)
	    {
		    MethodInfo applyConfigurationGenericMethod = typeof(ModelBuilder)
			    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
			    .FirstOrDefault(m => m.Name == nameof(ModelBuilder.ApplyConfiguration) && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

		    var applicableTypesWithConfigurations = assembly
			    .GetTypes()
			    .Where(type => String.IsNullOrEmpty(namespaceName) || (type.Namespace?.StartsWith(namespaceName) ?? false)) // anonymní typy v unit testech mají namespace nullový
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
