using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Odstraní podtržítka z názvů všech vlastností (sloupců) všech tabulek (včetně vazbových M:N).
	/// </summary>
	public class NoUnderscopesInPropertiesNamingConvention : IStoreModelConvention<EdmProperty>
	{
		/// <summary>
		/// Aplikuje konvenci na model.
		/// </summary>
		public void Apply(EdmProperty member, DbModel model)
		{
			member.Name = member.Name.Replace("_", "");
		}
	}
}
