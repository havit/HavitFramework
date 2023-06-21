using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;

[Keyless]
public class KeylessClass
{
	public string Value { get; set; }
}
