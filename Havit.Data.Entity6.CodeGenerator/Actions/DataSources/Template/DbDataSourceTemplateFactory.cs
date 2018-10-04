using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Template
{
	public class DbDataSourceTemplateFactory : ITemplateFactory<DbDataSourceModel>
	{
		public ITemplate CreateTemplate(DbDataSourceModel model)
		{
			return new DbDataSourceTemplate(model);
		}
	}

}
