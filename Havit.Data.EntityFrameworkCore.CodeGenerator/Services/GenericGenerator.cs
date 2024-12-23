namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class GenericGenerator<TModel>
{
	private readonly IModelSource<TModel> modelSource;
	private readonly ITemplateFactory<TModel> templateFactory;
	private readonly IFileNamingService<TModel> fileNamingService;
	private readonly CodeWriter codeWriter;

	private readonly bool canOverwriteExistingFile;

	public GenericGenerator(IModelSource<TModel> modelSource, ITemplateFactory<TModel> templateFactory, IFileNamingService<TModel> fileNamingService, CodeWriter codeWriter, bool canOverwriteExistingFile = true)
	{
		this.modelSource = modelSource;
		this.templateFactory = templateFactory;
		this.fileNamingService = fileNamingService;
		this.codeWriter = codeWriter;
		this.canOverwriteExistingFile = canOverwriteExistingFile;
	}

	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		List<TModel> models = modelSource.GetModels().ToList();

		await Task.WhenAll(models.Select(async model =>
		{
			ITemplate template = templateFactory.CreateTemplate(model);

			string content = template.TransformText();
			string filename = fileNamingService.GetFilename(model);
			await codeWriter.SaveAsync(filename, content, canOverwriteExistingFile, cancellationToken);
		}));
	}
}
