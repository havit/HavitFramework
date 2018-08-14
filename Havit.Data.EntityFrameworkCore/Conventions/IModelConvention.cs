using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.Entity.Conventions
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
