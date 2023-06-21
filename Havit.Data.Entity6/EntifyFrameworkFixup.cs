using System.Diagnostics.CodeAnalysis;

namespace Havit.Data.Entity;

internal class EntifyFrameworkFixup
{
	[SuppressMessage("SonarLint", "S1848", Justification = "Zde je účelné získat hodnotu, přestože nebude nikdy použita.")]
	internal EntifyFrameworkFixup()
	{
		// WORKAROUND:
		// aby se EntityFramework.SqlServer.dll dostalo do cílového projektu, musíme šahnout na nějaký typ z této assembly
		new System.Data.Entity.SqlServer.SqlAzureExecutionStrategy();
	}
}
