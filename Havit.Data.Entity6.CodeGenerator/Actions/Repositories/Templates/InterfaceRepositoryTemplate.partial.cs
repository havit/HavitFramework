using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Templates
{
	public partial class InterfaceRepositoryTemplate : ITemplate
	{
		protected RepositoryModel Model { get; private set; }

		public InterfaceRepositoryTemplate(RepositoryModel model)
		{
			this.Model = model;
		}
	}
}
