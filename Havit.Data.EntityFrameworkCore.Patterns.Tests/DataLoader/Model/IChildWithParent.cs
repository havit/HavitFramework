namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;

/// <summary>
/// Interface implementovaný implicitně entitami Child a AnotherChild.
/// Slouží k testům načítání referencí DataLoaderem přes entity typované interface.
/// </summary>
public interface IChildWithParent
{
	Master Parent { get; set; }
}
