namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class GenericGenerator : IGenericGenerator
{
	private readonly ICodeWriter _codeWriter;

	public GenericGenerator(ICodeWriter codeWriter, OverwriteBahavior overwriteBahavior = OverwriteBahavior.OverwriteWhenFileAlreadyExists)
	{
		_codeWriter = codeWriter;
	}

	public async Task GenerateAsync<TModel>(IModelSource<TModel> modelSource, ITemplateFactory<TModel> templateFactory, IFileNamingService<TModel> fileNamingService, OverwriteBahavior overwriteBahavior = OverwriteBahavior.OverwriteWhenFileAlreadyExists, CancellationToken cancellationToken = default)
	{
		List<TModel> models = modelSource.GetModels();

		await Task.WhenAll(models.Select(async model =>
		{
			ITemplate template = templateFactory.CreateTemplate(model);

			string content = template.TransformText();
			string filename = fileNamingService.GetFilename(model);
			await _codeWriter.SaveAsync(filename, content, overwriteBahavior, cancellationToken);
		}));
	}
}
