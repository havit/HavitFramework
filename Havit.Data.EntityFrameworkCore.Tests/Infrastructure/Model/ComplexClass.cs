using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;

[ComplexType]
public class ComplexClass
{
	public string Value { get; set; }
}
