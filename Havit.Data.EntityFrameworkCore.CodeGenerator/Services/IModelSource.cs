using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IModelSource<TModel>
{
	IEnumerable<TModel> GetModels();
}
