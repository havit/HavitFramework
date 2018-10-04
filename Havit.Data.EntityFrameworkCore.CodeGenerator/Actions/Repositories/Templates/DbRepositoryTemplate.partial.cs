using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates
{
	public partial class DbRepositoryTemplate : ITemplate
	{
		protected RepositoryModel Model { get; private set; }

		public DbRepositoryTemplate(RepositoryModel model)
		{
			this.Model = model;
		}
	}
}
