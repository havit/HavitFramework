﻿using System;
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
	public static class DependencyInjectionWebFormsHelper
	{
		#region Fields
		private static IWindsorContainer _resolver;
		private static readonly ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();
		#endregion

		/// <summary>
		/// Nastaví resolver (container) pro použití v Havit.CastleWindsor.WebForms.
		/// Resolver je držen ve statickém fieldu až do konce životního cyklu aplikace.
		/// </summary>
		/// <remarks>
		/// Metoda ReleseResolver() zatím neřešena, asi není praktický scénář využití.
		/// </remarks>
		public static void SetResolver(IWindsorContainer container)
		{
			if (_resolver != null)
			{
				throw new InvalidOperationException("Resolver je již nastaven!");
			}
			_resolver = container;
		}

		/// <summary>
		/// Initializes the control (including child controls).
		/// </summary>
		/// <param name="control">Control to be initialized.</param>
		public static bool InitializeControl(Control control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control"); // TODO: nameof(control));
			}

			bool anyInstanceDependency = InitializeControlInstance(control);

			InitializeChildControls(control);

			return anyInstanceDependency;
		}

		/// <summary>
		/// Initializes the controls and hooks the Unload. (doesn't care about children)
		/// </summary>
		internal static bool InitializeControlInstance(Control control)
		{
			bool anyInstanceDependency = InitializeInstance(control);

			if (anyInstanceDependency)
			{
				control.Unload += (s, e) => { ReleaseDependencies(control); };
			}
			return anyInstanceDependency;
		}

		/// <summary>
		/// Initializes child controls.
		/// </summary>
		internal static bool InitializeChildControls(Control control)
		{
			Control[] childControls = GetChildControls(control);
			bool anyResolvedDependency = false;

			foreach (Control childControl in childControls)
			{
				anyResolvedDependency = InitializeControlInstance(childControl) || anyResolvedDependency;
			}

			return anyResolvedDependency;
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		private static Control[] GetChildControls(Control control)
		{
			// UserControls jsou vždycky "moje", začínající Havit mají reprezentovat controly, které jsou ve WebBase. Do této podmínky spadnou i controly HFW, což je relativně zbytečné
			// možná optimalizace do budoucna je namísto "Havit." vzít jen ty, které NEJSOU z assembly Havit.Web ani System.Web. Pokud Havit.Web nezíská závislost na Castle Windsoru.
			Func<Control, IEnumerable<Control>> getAllControls = null;
			getAllControls = c => c.Controls.Cast<Control>().SelectMany(getAllControls).Concat(new[] { c });

			return getAllControls(control)
				.Where(c => (c != control) && ((c is UserControl) || c.GetType().FullName.StartsWith("Havit.")))
				.ToArray();
		}

		/// <summary>
		/// Initializes the instance (Only the instance itself without child controls!).
		/// </summary>
		internal static bool InitializeInstance(object control)
		{
			PropertyInfo[] props = GetInjectableProperties(control);

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

			return props.Length > 0;
		}

		/// <summary>
		/// Get a collection of injectable properties for the control
		/// </summary>
		private static PropertyInfo[] GetInjectableProperties(object control)
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
		internal static void ReleaseDependencies(object control)
		{
			PropertyInfo[] props = GetInjectableProperties(control);
			foreach (PropertyInfo propertyInfo in props)
			{
				object dependencyInstance = propertyInfo.GetValue(control);
				Type enumerableType = GetEnumerableType(propertyInfo.PropertyType);

				if (enumerableType != null)
				{
					IEnumerable dependencyInstanceEnumerable = (IEnumerable)dependencyInstance;
					foreach (var dependencyInstanceItem in dependencyInstanceEnumerable)
					{
						_resolver.Release(dependencyInstanceItem);
					}
				}
				else if (dependencyInstance != null)
				{
					_resolver.Release(dependencyInstance);
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