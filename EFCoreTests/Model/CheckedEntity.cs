using System.ComponentModel.DataAnnotations;

namespace Havit.EFCoreTests.Model;

public class CheckedEntity
{
	public int Id { get; set; }
	public string Value { get; set; }

	public int? AddressId { get; set; }
	public Address Address { get; set; }

	[ConcurrencyCheck]
	public int Version { get; set; }
}
