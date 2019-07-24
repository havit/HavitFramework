namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// Základný interface pre Model Extensions.
	/// </summary>
	public interface IModelExtension
	{
		/// <summary>
		/// Názov databázového objektu, ktorý je spravovaný touto <see cref="IModelExtension"/>.
		/// </summary>
        string ObjectName { get; }
	}
}