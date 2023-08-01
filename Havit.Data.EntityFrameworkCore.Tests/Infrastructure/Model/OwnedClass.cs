using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;

[Owned]
public class OwnedClass
{
	public string Value { get; set; }
}
