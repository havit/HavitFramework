using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template
{
	public class DbDataSourceTemplateFactory : ITemplateFactory<DbDataSourceModel>
	{
		public ITemplate CreateTemplate(DbDataSourceModel model)
		{
			return new DbDataSourceTemplate(model);
		}
	}

}
