namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IModelSourceErrorsProvider
{
	IEnumerable<string> GetModelErrors();
}
