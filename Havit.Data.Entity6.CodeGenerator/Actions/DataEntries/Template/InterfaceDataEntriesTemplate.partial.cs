using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Template
{
	public partial class InterfaceDataEntriesTemplate : ITemplate
	{
		protected DataEntriesModel Model { get; private set; }

		public InterfaceDataEntriesTemplate(DataEntriesModel model)
		{
			this.Model = model;
		}
	}
}
