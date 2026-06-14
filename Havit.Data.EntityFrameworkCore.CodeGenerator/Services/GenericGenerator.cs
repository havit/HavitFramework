namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class GenericGenerator : IGenericGenerator
{
	private readonly ICodeWriter _codeWriter;

	public GenericGenerator(ICodeWriter codeWriter)
	{
		_codeWriter = codeWriter;
	}

	public async Task GenerateAsync<TModel>(IModelSource<TModel> modelSource, Func<TModel, ITemplate> templateFactory, IFileNamingService<TModel> fileNamingService, OverwriteBehavior overwriteBehavior = OverwriteBehavior.OverwriteWhenFileAlreadyExists, CancellationToken cancellationToken = default)
	{
		List<TModel> models = modelSource.GetModels();

		await Task.WhenAll(models.Select(async model =>
		{
			ITemplate template = templateFactory(model);

			string content = template.TransformText();
			string filename = fileNamingService.GetFilename(model);
			await _codeWriter.SaveAsync(filename, content, overwriteBehavior, cancellationToken);
		}));
	}
}
