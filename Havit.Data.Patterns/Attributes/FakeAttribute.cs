using System;

namespace Havit.Data.Patterns.Attributes;

/// <summary>
/// Označuje třídy, které jsou Fake implementací.
/// Pomáhá určovat třídy, které nemají být registrovány do IoC containeru.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class FakeAttribute : Attribute
{
}
