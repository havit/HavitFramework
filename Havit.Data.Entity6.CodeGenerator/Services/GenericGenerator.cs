using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Havit.Data.Entity.CodeGenerator.Services;

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

	public void Generate()
	{
		List<TModel> models = modelSource.GetModels().ToList();

#if DEBUG
		models.ForEach(model =>
#else
		Parallel.ForEach(models, model =>
#endif
		{
			ITemplate template = templateFactory.CreateTemplate(model);

			string content = template.TransformText();
			string filename = fileNamingService.GetFilename(model);
			codeWriter.Save(filename, content, canOverwriteExistingFile);
		});
	}
}
