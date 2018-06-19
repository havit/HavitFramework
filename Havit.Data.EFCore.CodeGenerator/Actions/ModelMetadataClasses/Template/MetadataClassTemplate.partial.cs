using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Template
{
	public partial class MetadataClassTemplate : ITemplate
	{
		protected MetadataClass Model { get; private set; }

		public MetadataClassTemplate(MetadataClass model)
		{
			this.Model = model;
		}
	}
}
