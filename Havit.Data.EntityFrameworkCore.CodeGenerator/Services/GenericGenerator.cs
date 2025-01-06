namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class GenericGenerator<TModel>
{
	private readonly IModelSource<TModel> _modelSource;
	private readonly ITemplateFactory<TModel> _templateFactory;
	private readonly IFileNamingService<TModel> _fileNamingService;
	private readonly ICodeWriter _codeWriter;
	private readonly OverwriteBahavior _overwriteBahavior;

	public GenericGenerator(IModelSource<TModel> modelSource, ITemplateFactory<TModel> templateFactory, IFileNamingService<TModel> fileNamingService, ICodeWriter codeWriter, OverwriteBahavior overwriteBahavior = OverwriteBahavior.OverwriteWhenFileAlreadyExists)
	{
		_modelSource = modelSource;
		_templateFactory = templateFactory;
		_fileNamingService = fileNamingService;
		_codeWriter = codeWriter;
		_overwriteBahavior = overwriteBahavior;
	}

	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		List<TModel> models = _modelSource.GetModels().ToList();

		await Task.WhenAll(models.Select(async model =>
		{
			ITemplate template = _templateFactory.CreateTemplate(model);

			string content = template.TransformText();
			string filename = _fileNamingService.GetFilename(model);
			await _codeWriter.SaveAsync(filename, content, _overwriteBahavior, cancellationToken);
		}));
	}
}
