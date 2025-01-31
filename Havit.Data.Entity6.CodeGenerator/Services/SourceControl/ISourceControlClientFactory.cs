﻿namespace Havit.Data.Entity.CodeGenerator.Services.SourceControl;

public interface ISourceControlClientFactory
{
	ISourceControlClient Create(string path);

	void Release(ISourceControlClient sourceControl);
}
