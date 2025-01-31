using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Havit.Data.Entity.Conventions;

/// <summary>
/// Všem sloupcům, které jsou typu string, nastaví povinnost hodnoty. Sloupce jsou tedy not-null.
/// </summary>
public class RequiredStringPropertiesConvention : IConceptualModelConvention<EdmProperty>
{
	/// <summary>
	/// Aplikuje konvenci na model.
	/// </summary>
	public void Apply(EdmProperty item, DbModel model)
	{
		if ((item.PrimitiveType != null) && (item.PrimitiveType.ClrEquivalentType == typeof(string)))
		{
			item.Nullable = false;
		}
	}
}
