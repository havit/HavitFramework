using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;

[Keyless]
public class KeylessClass
{
	public string Value { get; set; }
}
