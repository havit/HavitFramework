using System;
using System.IO;

namespace Havit.Services.Azure.FileStorage.Internal;

/// <summary>
/// MemoryStream, který při uzavření provede definovanou akci.
/// </summary>
internal class BeforeCloseActionableMemoryStream : MemoryStream
{
	private readonly Action<MemoryStream> beforeClosingAction;

	public BeforeCloseActionableMemoryStream(Action<MemoryStream> beforeClosingAction)
	{
		this.beforeClosingAction = beforeClosingAction;
	}

	public override void Close()
	{
		beforeClosingAction(this);
		base.Close();
	}
}
