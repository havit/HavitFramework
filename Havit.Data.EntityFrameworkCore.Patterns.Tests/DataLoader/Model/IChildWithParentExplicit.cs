namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;

/// <summary>
/// Interface implementovaný entitou Child explicitně (bez stejnojmenné public vlastnosti).
/// Slouží k testu, že DataLoader pro explicitně implementovanou vlastnost vyhodí popisnou výjimku.
/// </summary>
public interface IChildWithParentExplicit
{
	Master ParentExplicit { get; }
}
