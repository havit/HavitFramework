using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

[Owned]
public class OwnedType
{
	public string Name { get; set; }
}
