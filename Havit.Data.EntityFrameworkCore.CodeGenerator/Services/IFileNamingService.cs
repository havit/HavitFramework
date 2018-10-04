namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services
{
	public interface IFileNamingService<TModel>
	{
		string GetFilename(TModel model);
	}
}