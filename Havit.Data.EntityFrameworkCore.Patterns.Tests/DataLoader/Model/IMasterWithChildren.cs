using Havit.Model.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;

/// <summary>
/// Interface implementovaný implicitně entitou Master.
/// Slouží k testům načítání kolekcí DataLoaderem přes entity typované interface.
/// </summary>
public interface IMasterWithChildren
{
	FilteringCollection<Child> Children { get; }
}
