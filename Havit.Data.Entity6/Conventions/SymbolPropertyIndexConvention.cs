using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using Havit.Data.Entity.Internal;
using Havit.Data.Entity.ModelConfiguration.Edm;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Zajišťuje existenci indexu na sloupci Symbol, pokud existuje.	
	/// </summary>
	public class SymbolPropertyIndexConvention : IStoreModelConvention<EdmProperty>
	{
		/// <summary>
		/// Aplikuje konvenci na model.
		/// </summary>
		public void Apply(EdmProperty member, DbModel model)
		{
			if (member.Name == "Symbol")
			{
				if (!member.DeclaringType.IsConventionSuppressed(typeof(SymbolPropertyIndexConvention)) && !member.IsConventionSuppressed(typeof(SymbolPropertyIndexConvention)))
				{
					IndexHelper.AddIndex(member, true);
				}
			}
		}
    }
}
