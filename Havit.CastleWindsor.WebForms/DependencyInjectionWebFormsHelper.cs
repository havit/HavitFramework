using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Castle.Windsor;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Pomocná třída pro resolve závislostí injektováním do existujícího objektu.
	/// Svým způsobem funguje jako ServiceLocator pro WindsorContainer.
	/// </summary>
	internal static class DependencyInjectionWebFormsHelper
	{
		#region Fields
		private static readonly ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();
		#endregion

		/// <summary>
		/// Initializes the instance (Only the instance itself without child controls!).
		/// </summary>
		internal static bool InitializeInstance(object control)
		{
			IWindsorContainer resolver = WindsorContainerAdapter.GetWindsorContainer();
			PropertyInfo[] props = GetInjectableProperties(control);

			// inject the values to properties
			foreach (PropertyInfo prop in props)
			{
				IEnumerable<InjectOverrideAttribute> overrideAttribs = prop.GetCustomAttributes(typeof(InjectOverrideAttribute), false).Cast<InjectOverrideAttribute>();
				Dictionary<string, object> resolvedSubdependencies = overrideAttribs
					.ToDictionary(x => x.PropertyName, x => resolver.Resolve(x.DependencyKey, x.DependencyServiceType, null));
				try
				{
					object value;
					Type enumerableType = GetEnumerableType(prop.PropertyType);
					if (enumerableType != null)
					{
						value = resolvedSubdependencies.Count > 0 ? resolver.ResolveAll(enumerableType, resolvedSubdependencies) : resolver.ResolveAll(enumerableType);
					}
					else
					{
						value = resolvedSubdependencies.Count > 0 ? resolver.Resolve(prop.PropertyType, resolvedSubdependencies) : resolver.Resolve(prop.PropertyType);
					}
					prop.SetValue(control, value, null);
				}
				catch (Exception e)
				{
					throw new ApplicationException("Error in resolving dependency " + prop.Name + ".", e);
				}
			}

			return props.Length > 0;
		}

		/// <summary>
		/// Get a collection of injectable properties for the control
		/// </summary>
		private static PropertyInfo[] GetInjectableProperties(object instance)
		{
			PropertyInfo[] props = cachedProperties.GetOrAdd(instance.GetType(), type =>
			{
				PropertyInfo[] nonPublicInstanceProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
					.Where(p => p.GetCustomAttributes(typeof(InjectAttribute), false).Length == 1).ToArray();

				if (nonPublicInstanceProperties.Length > 0)
				{
					throw new NotSupportedException(String.Format("InjectAttribute cannot be used on a non public property. It is used on property {0} in {1}.",
						nonPublicInstanceProperties.First().Name, nonPublicInstanceProperties.First().DeclaringType.FullName));
				}

				PropertyInfo[] staticProperties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
					.Where(p => p.GetCustomAttributes(typeof(InjectAttribute), false).Length == 1).ToArray();

				if (staticProperties.Length > 0)
				{
					throw new NotSupportedException(String.Format("InjectAttribute cannot be used on a static property. It is used on property {0} in {1}.",
						staticProperties.First().Name, staticProperties.First().DeclaringType.FullName));
				}

				return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.Where(p => p.GetCustomAttributes(typeof(InjectAttribute), false).Length == 1).ToArray();
			});
			return props;
		}

		/// <summary>
		/// Releases all dependencies of the instance (which is being released)
		/// </summary>
		internal static void ReleaseDependencies(object control)
		{
			IWindsorContainer resolver = WindsorContainerAdapter.GetWindsorContainer();
			PropertyInfo[] props = GetInjectableProperties(control);

			foreach (PropertyInfo propertyInfo in props)
			{
				object dependencyInstance = propertyInfo.GetValue(control);
				Type enumerableType = GetEnumerableType(propertyInfo.PropertyType);

				if ((enumerableType != null) && (dependencyInstance != null))
				{
					IEnumerable dependencyInstanceEnumerable = (IEnumerable)dependencyInstance;
					foreach (var dependencyInstanceItem in dependencyInstanceEnumerable)
					{
						resolver.Release(dependencyInstanceItem);
					}
				}
				else if (dependencyInstance != null)
				{
					resolver.Release(dependencyInstance);
				}

				propertyInfo.SetValue(control, null);
			}
		}

		/// <summary>
		/// Gets enumerated type if interface of input type is IEnumerable&lt;TEntity&gt; or an array.
		/// </summary>
		private static Type GetEnumerableType(Type type)
		{
			if ((type != null) && type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
			{
				return type.GetGenericArguments().FirstOrDefault();
			}

			if ((type != null) && type.IsArray && type.HasElementType && (type.GetArrayRank() == 1))
			{
				return type.GetElementType();
			}

			return null;
		}
	}
}