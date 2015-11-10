using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Zajišťuje pojmenování sloupců cizích klíčů tak, aby neobsahovaly přidené podtržítko.
	/// Řeší cizí klíče jak běžné vazby 1:N, jak cizí klíče ve vztahových tabulkách pro M:N.
	/// </summary>
	public class ForeignKeyNamingConvention : IStoreModelConvention<AssociationType>
	{
		public void Apply(AssociationType association, DbModel model)
		{
			if (association.IsForeignKey)
			{
				var constraint = association.Constraint;
				for (int i = 0; i < constraint.FromProperties.Count; i++)
				{
					string fromName = constraint.FromProperties[i].Name;
					string toName = constraint.ToProperties[i].Name;

					if (toName.EndsWith("_" + fromName))
					{
						constraint.ToProperties[i].Name = toName.Substring(0, toName.Length - fromName.Length - 1 /* podtržítko */) + fromName;
					}
					if (fromName.EndsWith("_" + toName))
					{
						constraint.FromProperties[i].Name = fromName.Substring(0, fromName.Length - toName.Length - 1 /* podtržítko */) + toName;
					}
				}
			}
		}
	}
}
