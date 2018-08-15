using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template
{
	public partial class InterfaceDataSourceTemplate : ITemplate
	{
		protected InterfaceDataSourceModel Model { get; private set; }

		public InterfaceDataSourceTemplate(InterfaceDataSourceModel model)
		{
			this.Model = model;
		}
	}
}
