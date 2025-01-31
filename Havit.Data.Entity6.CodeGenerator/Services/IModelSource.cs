﻿namespace Havit.Data.Entity.CodeGenerator.Services;

public interface IModelSource<TModel>
{
	IEnumerable<TModel> GetModels();
}
