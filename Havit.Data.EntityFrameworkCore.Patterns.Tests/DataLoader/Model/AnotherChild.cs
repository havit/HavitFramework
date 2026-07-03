namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;

/// <summary>
/// Druhá entita implementující IChildWithParent.
/// Slouží k testu, že DataLoader při načítání přes interface odmítne entity různých typů v jednom volání.
/// </summary>
public class AnotherChild : IChildWithParent
{
	public int Id { get; set; }

	// Reference
	public Master Parent { get; set; }
	public int? ParentId { get; set; }
}
