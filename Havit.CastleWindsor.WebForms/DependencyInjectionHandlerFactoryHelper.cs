using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Windsor;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Pomocná třída pro resolve závislostí injektováním do existujícího objektu
	/// </summary>
	public static class DependencyInjectionHandlerFactoryHelper
	{
		#region Fields
		private static IWindsorContainer _resolver;
		#endregion

		/// <summary>
		/// Nastaví resolver do vnitřku třídy
		/// </summary>
		public static void SetResolver(IWindsorContainer container)
		{
			if (_resolver != null)
			{
				throw new ApplicationException("Resolver je již nastaven!");
			}
			_resolver = container;
		}

		/// <summary>
		/// Initializes the instance.
		/// </summary>
		internal static void InitializeInstance(object control, ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties)
		{
			PropertyInfo[] props = GetInjectableProperties(control, cachedProperties);

			// inject the values to properties
			foreach (PropertyInfo prop in props)
			{
				IEnumerable<InjectOverrideAttribute> overrideAttribs = prop.GetCustomAttributes(typeof(InjectOverrideAttribute), false).Cast<InjectOverrideAttribute>();
				Dictionary<string, object> resolvedSubdependencies = overrideAttribs
					.ToDictionary(x => x.PropertyName, x => _resolver.Resolve(x.DependencyKey, x.DependencyServiceType, null));
				try
				{
					object value;
					Type enumerableType = GetEnumerableType(prop.PropertyType);
					if (enumerableType != null)
					{
						value = resolvedSubdependencies.Count > 0 ? _resolver.ResolveAll(enumerableType, resolvedSubdependencies) : _resolver.ResolveAll(enumerableType);
					}
					else
					{
						value = resolvedSubdependencies.Count > 0 ? _resolver.Resolve(prop.PropertyType, resolvedSubdependencies) : _resolver.Resolve(prop.PropertyType);
					}
					prop.SetValue(control, value, null);
				}
				catch (Exception e)
				{
					throw new ApplicationException("Error in resolving dependency " + prop.Name + ".", e);
				}
			}
		}

		/// <summary>
		/// Get a collection of injectable properties for the type
		/// </summary>
		private static PropertyInfo[] GetInjectableProperties(object control, ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties)
		{
			PropertyInfo[] props = cachedProperties.GetOrAdd(control.GetType(), type =>
			{
				return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.Where(p => p.GetCustomAttributes(typeof(InjectAttribute), false).Length == 1).ToArray();
			});
			return props;
		}

		/// <summary>
		/// Releases all dependencies of the instance (which is being released)
		/// </summary>
		internal static void ReleaseDependencies(object control, ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties)
		{
			PropertyInfo[] props = GetInjectableProperties(control, cachedProperties);
			foreach (PropertyInfo propertyInfo in props)
			{
				object dependencyInstance = propertyInfo.GetValue(control);
				if (dependencyInstance != null)
				{
					_resolver.Release(dependencyInstance);
				}
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