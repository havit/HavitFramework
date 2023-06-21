using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.ConstraintBuilders;

public static class ConstraintBuilder
{
	public static void CreateConstraints(Table table)
	{
		DateConstraintBuilder.CreateConstraints(table);
	}
}
