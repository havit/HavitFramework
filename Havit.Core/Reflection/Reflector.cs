using System.Reflection;

namespace Havit.Reflection;

/// <summary>
/// Class with static methods for simple reflection operations.
/// </summary>
public static class Reflector
{
	/// <summary>
	/// Gets the value of a property, even if it is marked as protected, internal, or private.
	/// The property is searched only on the specified type (targetType).
	/// </summary>
	/// <param name="target">The object from which the property should be obtained.</param>
	/// <param name="targetType">The type from which the property should be obtained (can also be the parent type of the target).</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The value of the property, or null if it is not found.</returns>
	public static object GetPropertyValue(Object target, Type targetType, String propertyName)
	{
		return GetPropertyValue(
			target,
			targetType,
			propertyName,
			BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
	}

	/// <summary>
	/// Gets the value of a property, even if it is marked as protected, internal, or private.
	/// </summary>
	/// <param name="target">The object from which the property should be obtained.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The value of the property, or null if it is not found.</returns>
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

	/// <summary>
	/// Sets the value of a property, even if it is marked as protected, internal, or private.
	/// If the property cannot be found, it throws an InvalidOperationException.
	/// </summary>
	/// <param name="target">The object from which the property should be obtained.</param>
	/// <param name="targetType">The type from which the property should be obtained (can also be the parent type of the target).</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="value">The value to be set.</param>
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
	/// Sets the value of a property, even if it is marked as protected, internal, or private.
	/// The property is searched only on the specified type (targetType).
	/// If the property cannot be found, it throws an InvalidOperationException.
	/// </summary>
	/// <param name="target">The object from which the property should be obtained.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="value">The value to be set.</param>
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
			throw new InvalidOperationException(String.Format("The property {0} was not found in the class {1}.", propertyName, targetType.FullName));
		}
		property.SetValue(target, value, null);
	}
}
