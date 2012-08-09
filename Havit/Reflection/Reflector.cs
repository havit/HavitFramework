using System;
using System.Reflection;

namespace Havit.Reflection
{
	/// <summary>
	/// Tøída se statickými metodami pro jednoduché operace reflexe.
	/// </summary>
	public sealed class Reflector
	{
		#region GetPropertyValue
		/// <summary>
		/// Získá hodnotu property, i kdyby byla oznaèená jako protected, internal, nebo private.
		/// </summary>
		/// <param name="target">Objekt, z kterého má být property získána.</param>
		/// <param name="targetType">Typ z kterého má být property získána (mùže být i rodièovským typem targetu).</param>
		/// <param name="propertyName">Jméno property.</param>
		/// <returns>Hodnota property, nebo null, není-li nalezena.</returns>
		public static object GetPropertyValue(Object target, Type targetType, String propertyName) 
		{
			PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic );
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

		#region private constructor
		/// <summary>
		/// private constructor, aby nebylo možno vytvoøit instanci tøídy
		/// </summary>
		private Reflector()	{}
		#endregion
	}
}
