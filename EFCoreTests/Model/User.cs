namespace Havit.EFCoreTests.Model;

public class User
{
	public Guid Id { get; set; }
	public string Username { get; set; } = new Guid().ToString();

	public override string ToString()
	{
		return "User.Id=" + Id;
	}
}
