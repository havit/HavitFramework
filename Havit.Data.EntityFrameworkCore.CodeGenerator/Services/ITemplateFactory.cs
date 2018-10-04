namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services
{
	public interface ITemplateFactory<TModel>
	{
		ITemplate CreateTemplate(TModel model);
	}
}
