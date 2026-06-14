using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses;

public class MetadataGenerator(
	IMetadataProject _metadataProject,
	MetadataClassModelSource _metadataClassModelSource,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		MetadataClassFileNamingService fileNamingService = new MetadataClassFileNamingService(_metadataProject);

		await _genericGenerator.GenerateAsync(_metadataClassModelSource, metadataClass => new MetadataClassTemplate(metadataClass), fileNamingService, cancellationToken: cancellationToken);
	}
}
