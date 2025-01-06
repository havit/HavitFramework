using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses;

public class MetadataGenerator(
	IMetadataProject _metadataProject,
	IModelProject _modelProject,
	DbContext _dbContext,
	CodeGeneratorConfiguration _configuration,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		MetadataClassFileNamingService fileNamingService = new MetadataClassFileNamingService(_metadataProject);
		MetadataClassModelSource modelSource = new MetadataClassModelSource(_dbContext, _metadataProject, _modelProject, _configuration);

		await _genericGenerator.GenerateAsync(modelSource, metadataClass => new MetadataClassTemplate(metadataClass), fileNamingService, cancellationToken: cancellationToken);
	}
}
