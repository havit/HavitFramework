using System;

namespace Havit;

/// <summary>
/// Indicates range of date - start date and end date.
/// </summary>
public readonly record struct DateTimeRange(DateTime? StartDate, DateTime? EndDate);
