using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// Provides extension methods for <see cref="IncrementalValueProvider{T}"/>.
/// </summary>
public static class IncrementalValueProviderExtensions
{
	/// <summary>
	/// Concatenates two IncrementalValueProvider instances.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the arrays.</typeparam>
	/// <param name="provider1">The first provider.</param>
	/// <param name="provider2">The second provider.</param>
	/// <returns>A new IncrementalValueProvider that contains the concatenated elements of the input providers.</returns>
	public static IncrementalValueProvider<ImmutableArray<T>> Concat<T>(this IncrementalValueProvider<ImmutableArray<T>> provider1, IncrementalValueProvider<ImmutableArray<T>> provider2)
	{
		return provider1.Combine(provider2).Select((item, _) => item.Left.Concat(item.Right)).SelectMany((item, _) => item).Collect();
	}
}
