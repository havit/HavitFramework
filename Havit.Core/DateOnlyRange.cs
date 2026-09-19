#if NET6_0_OR_GREATER
namespace Havit;

/// <summary>
/// Indicates a range of calendar dates - start date and end date.
/// </summary>
/// <param name="StartDate">The start date, or <see langword="null"/> for an open start.</param>
/// <param name="EndDate">The end date, or <see langword="null"/> for an open end.</param>
/// <remarks>
/// Included in the net8.0 and net10.0 package targets; not available on net48 or netstandard2.0.
/// Conditional compilation follows native <see cref="DateOnly"/> availability (.NET 6 or later).
/// The default value is an empty range with both endpoints null.
/// Endpoints are preserved without validation or normalization; the start date may be after the end date.
/// Calendar dates have no time-zone or UTC conversion behavior.
/// </remarks>
#pragma warning disable SA1313 // Parameter must begin with lower-case letter
public readonly record struct DateOnlyRange(DateOnly? StartDate, DateOnly? EndDate);
#pragma warning restore SA1313 // Parameter must begin with lower-case letter
#endif
