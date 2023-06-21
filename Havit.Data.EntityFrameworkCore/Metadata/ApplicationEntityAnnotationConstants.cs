using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata;

/// <summary>
/// Konstanty k označení entit jako aplikační entita.
/// </summary>
public static class ApplicationEntityAnnotationConstants
{
	/// <summary>
	/// Název anotace pro explicitní označení, zda je entity aplikační entitou. Určeno pro vyjmutí některých entit z "firemního standardnu". Např. pro entity IdentityServeru, atp.
	/// </summary>
	public const string IsApplicationEntityAnnotationName = "IsApplicationEntity";
}
