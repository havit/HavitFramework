
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IGenericGenerator
{
	Task GenerateAsync<TModel>(IModelSource<TModel> modelSource, ITemplateFactory<TModel> templateFactory, IFileNamingService<TModel> fileNamingService, OverwriteBahavior overwriteBahavior = OverwriteBahavior.OverwriteWhenFileAlreadyExists, CancellationToken cancellationToken = default);
}