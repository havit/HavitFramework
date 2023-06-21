namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model;

/// <summary>
/// Location of a person.
/// </summary>
public class PersonLocation
{
	/// <summary>
	/// Primary key of <see cref="PersonLocation"/> entity.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Navigation property for <see cref="Person"/> entity.
	/// </summary>
	public Person Person { get; set; }
	/// <summary>
	/// Primary key of <see cref="Person"/> entity.
	/// </summary>
	public int PersonId { get; set; }

	/// <summary>
	/// Navigation property for <see cref="Location"/> entity.
	/// </summary>
	public Location Location { get; set; }
	/// <summary>
	/// Primary key <see cref="Location"/> entity.
	/// </summary>
	public int LocationId { get; set; }
}