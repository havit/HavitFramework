namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IModelErrorsProvider
{
	List<string> GetErrors();
}