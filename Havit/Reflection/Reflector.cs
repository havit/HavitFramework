using System;
using System.Reflection;

namespace Havit.Reflection
{
	/// <summary>
	/// Třída se statickými metodami pro jednoduché operace reflexe.
	/// </summary>
	public static class Reflector
	{
		#region GetPropertyValue
		/// <summary>
		/// Získá hodnotu property, i kdyby byla označená jako protected, internal, nebo private.
		/// Vlastnost je hledána jen na zadaném typu (targetType).
		/// </summary>
		/// <param name="target">Objekt, z kterého má být property získána.</param>
		/// <param name="targetType">Typ z kterého má být property získána (může být i rodičovským typem targetu).</param>
		/// <param name="propertyName">Jméno property.</param>
		/// <returns>Hodnota property, nebo null, není-li nalezena.</returns>
		public static object GetPropertyValue(Object target, Type targetType, String propertyName) 
		{
			return GetPropertyValue(
				target,
				targetType,
				propertyName,
				BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}

		/// <summary>
		/// Získá hodnotu property, i kdyby byla označená jako protected, internal, nebo private.
		/// </summary>
		/// <param name="target">Objekt, z kterého má být property získána.</param>
		/// <param name="propertyName">Jméno property.</param>
		/// <returns>Hodnota property, nebo null, není-li nalezena.</returns>
		public static object GetPropertyValue(Object target, String propertyName)
		{
			return GetPropertyValue(
				target,
				target.GetType(),
				propertyName,
				BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}

		private static object GetPropertyValue(Object target, Type targetType, String propertyName, BindingFlags bindingFlags)
		{
			PropertyInfo property = targetType.GetProperty(propertyName, bindingFlags);
			if (property != null)
			{
				return property.GetValue(target, null);
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region SetPropertyValue
		/// <summary>
		/// Nastaví hodnotu property, i kdyby byla označená jako protected, internal, nebo private.
		/// Pokud se nepodaří vlastnost nalézt, vyvolá výjimku InvalidOperationException.
		/// </summary>
		/// <param name="target">Objekt, z kterého má být property získána.</param>
		/// <param name="targetType">Typ z kterého má být property získána (může být i rodičovským typem targetu).</param>
		/// <param name="propertyName">Jméno property.</param>
		/// <param name="value">Hodnota, která má být nastavena.</param>
		public static void SetPropertyValue(Object target, Type targetType, String propertyName, object value)
		{
			SetPropertyValue(
				target,
				targetType,
				propertyName,
				BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				value);
		}

		/// <summary>
		/// Nastaví hodnotu property, i kdyby byla označená jako protected, internal, nebo private.
		/// Vlastnost je hledána jen na zadaném typu (targetType).
		/// Pokud se nepodaří vlastnost nalézt, vyvolá výjimku InvalidOperationException.
		/// </summary>
		/// <param name="target">Objekt, z kterého má být property získána.</param>
		/// <param name="propertyName">Jméno property.</param>
		/// <param name="value">Hodnota, která má být nastavena.</param>
		public static void SetPropertyValue(Object target, String propertyName, object value)
		{
			SetPropertyValue(
				target,
				target.GetType(),
				propertyName, 
				BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
				value);
		}

		private static void SetPropertyValue(Object target, Type targetType, String propertyName, BindingFlags bindingFlags, object value)
		{
			PropertyInfo property = targetType.GetProperty(propertyName, bindingFlags);
			if (property == null)
			{
				throw new InvalidOperationException(String.Format("Vlastnost {0} nebyla v třídě {1} nalezena.", propertyName, targetType.FullName));
			}
			property.SetValue(target, value, null);
		}
		
		#endregion
	}
}
