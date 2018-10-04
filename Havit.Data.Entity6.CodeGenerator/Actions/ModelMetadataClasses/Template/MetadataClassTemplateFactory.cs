using Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Template
{
	public class MetadataClassTemplateFactory : ITemplateFactory<MetadataClass>
	{
		public ITemplate CreateTemplate(MetadataClass model)
		{
			return new MetadataClassTemplate(model);
		}
	}

}
