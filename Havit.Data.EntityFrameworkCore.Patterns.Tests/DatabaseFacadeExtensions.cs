using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests
{
	/// <summary>
	/// Extension metody k DatabaseFacade.
	/// </summary>
    internal static class DatabaseFacadeExtensions
    {
		/// <summary>
		/// Dropne a založí znovu databázi.
		/// Neobsahuje intrukci pro spuštění migrací, protože ta v InMemory database vyhazuje výjimku.
		/// </summary>
	    public static void DropCreate(this DatabaseFacade database)
	    {
		    database.EnsureDeleted();
		    database.EnsureCreated();
	    }
    }
}
