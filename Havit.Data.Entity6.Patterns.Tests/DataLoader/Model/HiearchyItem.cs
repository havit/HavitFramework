namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Model;

public class HiearchyItem
{
	public int Id { get; set; }

	public HiearchyItem Parent { get; set; }
	public int? ParentId { get; set; }

	public ICollection<HiearchyItem> Children { get; set; }
}
