using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.ConstraintBuilders
{
	public static class ConstraintBuilder
	{
		#region CreateConstraints
		public static void CreateConstraints(Table table)
		{
			DateConstraintBuilder.CreateConstraints(table);
		}
		#endregion
	}
}
