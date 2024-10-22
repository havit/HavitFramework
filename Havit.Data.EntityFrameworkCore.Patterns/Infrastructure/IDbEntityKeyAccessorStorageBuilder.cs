namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Sestaví (I)DbEntityKeyAccessorStorage.
/// </summary>
public interface IDbEntityKeyAccessorStorageBuilder
{
	/// <summary>
	/// Sestaví (I)DbEntityKeyAccessorStorage.
	/// </summary>
	IDbEntityKeyAccessorStorage Build();
}