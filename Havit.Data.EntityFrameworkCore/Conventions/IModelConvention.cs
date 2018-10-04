using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Konvence aplikovaná na model (model builder).
	/// </summary>
    public interface IModelConvention
    {
		/// <summary>
		/// Aplikuje konvenci.
		/// </summary>
	    void Apply(ModelBuilder modelBuilder);
    }
}
