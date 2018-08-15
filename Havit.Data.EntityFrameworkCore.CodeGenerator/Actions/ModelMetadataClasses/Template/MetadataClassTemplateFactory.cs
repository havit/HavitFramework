using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Template
{
	public class MetadataClassTemplateFactory : ITemplateFactory<MetadataClass>
	{
		public ITemplate CreateTemplate(MetadataClass model)
		{
			return new MetadataClassTemplate(model);
		}
	}

}
