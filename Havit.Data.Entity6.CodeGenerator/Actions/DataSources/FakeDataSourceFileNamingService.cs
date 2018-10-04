using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources
{
	public class FakeDataSourceFileNamingService : FileNamingServiceBase<FakeDataSourceModel>
	{
		public FakeDataSourceFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override string GetClassName(FakeDataSourceModel model)
		{
			return model.FakeDataSourceClassName;
		}

		protected override string GetNamespaceName(FakeDataSourceModel model)
		{
			return model.NamespaceName;
		}
	}
}
