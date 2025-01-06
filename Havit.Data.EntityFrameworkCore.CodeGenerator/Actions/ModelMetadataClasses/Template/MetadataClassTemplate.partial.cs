using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Template;

public partial class MetadataClassTemplate : ITemplate
{
	protected MetadataClass Model { get; private set; }

	public MetadataClassTemplate(MetadataClass model)
	{
		Model = model;
	}
}
