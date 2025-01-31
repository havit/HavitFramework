using System.ComponentModel.DataAnnotations;

namespace Havit.EFCoreTests.Model;

public class Address
{
	public int Id { get; set; }

	[MaxLength(200)]
	public string Street { get; set; }

	[MaxLength(100)]
	public string City { get; set; }

	[MaxLength(10)]
	public string ZipCode { get; set; }
}
