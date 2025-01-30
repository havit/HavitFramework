namespace Havit.Collections;

/// <summary>
/// Extension methods for SortDirection.
/// </summary>
public static class SortDirectionExtensions
{
	/// <summary>
	/// Returns the opposite sorting direction.
	/// </summary>
	public static SortDirection Reverse(this SortDirection source)
	{
		return source switch
		{
			SortDirection.Ascending => SortDirection.Descending,
			SortDirection.Descending => SortDirection.Ascending,
			_ => throw new InvalidOperationException(source.ToString())
		};
	}
}
