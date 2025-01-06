namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IModelSource<TModel>
{
	List<TModel> GetModels();
}
