using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Markovací rozšíření IDbContextu.
	/// Určeno pro možnost zaregistrovat IDbContext s transientním lifetime v ServiceCollection.
	/// </summary>
	public interface IDbContextTransient : IDbContext
	{
	}
}
