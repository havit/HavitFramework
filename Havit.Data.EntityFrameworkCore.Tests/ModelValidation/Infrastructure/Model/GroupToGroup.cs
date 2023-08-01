namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class GroupToGroup
{
	public int ParentGroupId { get; set; }
	public Group ParentGroup { get; set; }

	public int ChildGroupId { get; set; }
	public Group ChildGroup { get; set; }
}
