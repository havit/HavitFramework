using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Havit.Reflection;

/// <summary>
/// Class with static methods for simple reflection operations.
/// </summary>
/// <remarks>
/// <para>
/// Memory behavior: resolved <see cref="PropertyInfo"/> lookups are cached in process-wide static dictionaries
/// keyed by <c>(type, propertyName, bindingFlags)</c>. The cache is never evicted - entries live for the lifetime
/// of the process (AppDomain). Negative results (a property not found on a type) are cached as well, so a name that
/// is genuinely missing on a type is not looked up repeatedly.
/// </para>
/// <para>
/// This is safe and intentional because <c>propertyName</c> is expected to come from a closed, bounded set
/// (property names known at compile time, column/binding definitions, ...), not from arbitrary or user-supplied
/// input. Under that assumption the number of distinct cache keys is finite (bounded by the set of types and the
/// fixed set of property names used), so the cache reaches a steady size and does not grow without limit.
/// Passing an unbounded stream of distinct <c>propertyName</c> values (e.g. raw user input) would make the cache
/// grow indefinitely and must be avoided.
/// </para>
/// </remarks>
public static class Reflector
{
	// PropertyInfo lookups are cached per (type, name, bindingFlags) - GetProperty (especially with IgnoreCase) is expensive and these helpers are often called in loops.
	// Misses (null) are cached too: a property genuinely missing on a type stays missing.
	// The cache is never evicted; this is fine because propertyName comes from a closed set (see the class <remarks>), so the number of distinct keys is bounded.
	private static readonly ConcurrentDictionary<(Type Type, string PropertyName, BindingFlags BindingFlags), PropertyInfo> propertyCache = new ConcurrentDictionary<(Type, string, BindingFlags), PropertyInfo>();
	private static readonly ConcurrentDictionary<(Type Type, string PropertyName, BindingFlags BindingFlags), PropertyInfo> propertyIncludingBaseTypesCache = new ConcurrentDictionary<(Type, string, BindingFlags), PropertyInfo>();

	// The properties are looked up by name, including the non-public ones - the target type must keep all of them when trimming.
	private const DynamicallyAccessedMemberTypes PropertyMemberTypes = DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties;

	// The overloads working with the runtime type of the target cannot be expressed by [DynamicallyAccessedMembers]:
	// the type is not known at the call site and, on top of that, the lookup walks up the whole type hierarchy
	// ([DynamicallyAccessedMembers] does not flow to the base types).
	private const string HierarchyLookupIsNotTrimCompatibleMessage = "The property is looked up by name on the runtime type of the target and on its base types (including non-public properties). Those members might be removed when trimming. Use the overload taking targetType, or make sure the properties are preserved.";

	/// <summary>
	/// Gets the value of a property, even if it is marked as protected, internal, or private.
	/// The property is searched only on the specified type (targetType).
	/// </summary>
	/// <param name="target">The object from which the property should be obtained.</param>
	/// <param name="targetType">The type from which the property should be obtained (can also be the parent type of the target).</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The value of the property, or null if it is not found.</returns>
	public static object GetPropertyValue(Object target, [DynamicallyAccessedMembers(PropertyMemberTypes)] Type targetType, String propertyName)
	{
		return GetPropertyValue(
			target,
			targetType,
			propertyName,
			BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
	}

	/// <summary>
	/// Gets the value of a property, even if it is marked as protected, internal, or private.
	/// The property is searched through the whole type hierarchy of the target object (including private properties declared on base types).
	/// </summary>
	/// <param name="target">The object from which the property should be obtained.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The value of the property, or null if it is not found.</returns>
	[RequiresUnreferencedCode(HierarchyLookupIsNotTrimCompatibleMessage)]
	public static object GetPropertyValue(Object target, String propertyName)
	{
		// We walk the hierarchy ourselves - GetProperty does not return private members declared on base types
		// and BindingFlags.FlattenHierarchy has no effect for instance members.
		PropertyInfo property = GetPropertyIncludingBaseTypes(
			target.GetType(),
			propertyName,
			BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		return (property != null)
			? property.GetValue(target, index: null)
			: null;
	}

	private static object GetPropertyValue(Object target, [DynamicallyAccessedMembers(PropertyMemberTypes)] Type targetType, String propertyName, BindingFlags bindingFlags)
	{
		PropertyInfo property = GetCachedProperty(targetType, propertyName, bindingFlags);
		if (property != null)
		{
			return property.GetValue(target, index: null);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Sets the value of a property, even if it is marked as protected, internal, or private.
	/// The property is searched only on the specified type (targetType).
	/// If the property cannot be found, it throws an InvalidOperationException.
	/// </summary>
	/// <param name="target">The object on which the property should be set.</param>
	/// <param name="targetType">The type on which the property should be searched (can also be the parent type of the target).</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="value">The value to be set.</param>
	public static void SetPropertyValue(Object target, [DynamicallyAccessedMembers(PropertyMemberTypes)] Type targetType, String propertyName, object value)
	{
		PropertyInfo property = GetCachedProperty(targetType, propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		if (property == null)
		{
			throw new InvalidOperationException(String.Format("The property {0} was not found in the class {1}.", propertyName, targetType.FullName));
		}
		property.SetValue(target, value, index: null);
	}

	/// <summary>
	/// Sets the value of a property, even if it is marked as protected, internal, or private.
	/// The property is searched through the whole type hierarchy of the target object (including private properties declared on base types).
	/// If the property cannot be found, it throws an InvalidOperationException.
	/// </summary>
	/// <param name="target">The object on which the property should be set.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="value">The value to be set.</param>
	[RequiresUnreferencedCode(HierarchyLookupIsNotTrimCompatibleMessage)]
	public static void SetPropertyValue(Object target, String propertyName, object value)
	{
		// We walk the hierarchy ourselves - GetProperty does not return private members declared on base types
		// and BindingFlags.FlattenHierarchy has no effect for instance members.
		PropertyInfo property = GetPropertyIncludingBaseTypes(
			target.GetType(),
			propertyName,
			BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		if (property == null)
		{
			throw new InvalidOperationException(String.Format("The property {0} was not found in the class {1}.", propertyName, target.GetType().FullName));
		}
		property.SetValue(target, value, index: null);
	}

	/// <summary>
	/// Finds a property by walking up the type hierarchy.
	/// Unlike a single <see cref="Type.GetProperty(string, BindingFlags)" /> call, this also finds private properties declared on base types.
	/// The most derived declaration wins (so a property hiding a base one via <c>new</c> does not cause an ambiguous match).
	/// </summary>
	[RequiresUnreferencedCode(HierarchyLookupIsNotTrimCompatibleMessage)]
	private static PropertyInfo GetPropertyIncludingBaseTypes(Type type, String propertyName, BindingFlags bindingFlags)
	{
		if (propertyIncludingBaseTypesCache.TryGetValue((type, propertyName, bindingFlags), out PropertyInfo cached))
		{
			return cached;
		}

		PropertyInfo result = null;
		Type currentType = type;
		while (currentType != null)
		{
			PropertyInfo property = currentType.GetProperty(propertyName, bindingFlags | BindingFlags.DeclaredOnly);
			if (property != null)
			{
				result = property;
				break;
			}
			currentType = currentType.BaseType;
		}

		propertyIncludingBaseTypesCache[(type, propertyName, bindingFlags)] = result;
		return result;
	}

	private static PropertyInfo GetCachedProperty([DynamicallyAccessedMembers(PropertyMemberTypes)] Type type, String propertyName, BindingFlags bindingFlags)
	{
		// Deliberately not GetOrAdd(key, valueFactory): the type would travel to the value factory inside the cache key
		// and its [DynamicallyAccessedMembers] annotation would be lost (the trimmer cannot flow annotations through a delegate).
		// Resolving the same property twice in a race is harmless (the result is identical) - GetOrAdd gives no stronger guarantee either.
		var key = (type, propertyName, bindingFlags);
		if (propertyCache.TryGetValue(key, out PropertyInfo cached))
		{
			return cached;
		}

		PropertyInfo result = type.GetProperty(propertyName, bindingFlags);
		propertyCache[key] = result;
		return result;
	}
}
