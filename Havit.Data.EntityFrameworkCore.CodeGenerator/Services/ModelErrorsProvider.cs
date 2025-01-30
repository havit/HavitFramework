
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class ModelErrorsProvider : IModelErrorsProvider
{
	private readonly IEnumerable<IModelSourceErrorsProvider> _modelErrorsProviders;

	public ModelErrorsProvider(IEnumerable<IModelSourceErrorsProvider> modelErrorsProviders)
	{
		_modelErrorsProviders = modelErrorsProviders;
	}

	public List<string> GetErrors()
	{
		return _modelErrorsProviders.SelectMany(x => x.GetModelErrors()).ToList();
	}
}
