using System;

namespace Havit;

/// <summary>
/// Indicates range of date - start date and end date.
/// </summary>
#pragma warning disable SA1313 // Parameter must begin with lower-case letter
public readonly record struct DateTimeRange(DateTime? StartDate, DateTime? EndDate);
#pragma warning restore SA1313 // Parameter must begin with lower-case letter