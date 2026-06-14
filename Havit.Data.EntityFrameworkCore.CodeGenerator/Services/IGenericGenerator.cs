
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IGenericGenerator
{
	Task GenerateAsync<TModel>(IModelSource<TModel> modelSource, Func<TModel, ITemplate> templateFactory, IFileNamingService<TModel> fileNamingService, OverwriteBehavior overwriteBehavior = OverwriteBehavior.OverwriteWhenFileAlreadyExists, CancellationToken cancellationToken = default);
}